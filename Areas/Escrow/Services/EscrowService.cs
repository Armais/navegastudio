using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NavegaStudio.Data;
using NavegaStudio.Areas.Escrow.Helpers;
using NavegaStudio.Areas.Escrow.Models;

namespace NavegaStudio.Areas.Escrow.Services;

public class EscrowService : IEscrowService
{
    private readonly EscrowDbContext _db;
    private readonly EthereumSettings _ethSettings;
    private readonly ILogger<EscrowService> _logger;

    public bool IsDemoMode => string.IsNullOrEmpty(_ethSettings.ContractAddress);

    public EscrowService(EscrowDbContext db, IOptions<EthereumSettings> ethSettings, ILogger<EscrowService> logger)
    {
        _db = db;
        _ethSettings = ethSettings.Value;
        _logger = logger;

        if (IsDemoMode)
            _logger.LogInformation("EscrowService running in DEMO mode (no contract configured)");
        else
            _logger.LogInformation("EscrowService connected to contract {Address} on {Chain}",
                _ethSettings.ContractAddress, _ethSettings.ChainName);
    }

    public async Task<EscrowTransaction?> GetEscrowAsync(int id)
    {
        return await _db.Escrows
            .Include(e => e.Events)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<List<EscrowTransaction>> GetUserEscrowsAsync(string address)
    {
        var addr = address.ToLowerInvariant();
        return await _db.Escrows
            .Include(e => e.Events)
            .Where(e => e.Buyer == addr
                     || e.Seller == addr
                     || e.Arbiter == addr)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<EscrowTransaction>> GetAllEscrowsAsync()
    {
        return await _db.Escrows
            .Include(e => e.Events)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    public async Task<EscrowTransaction> CreateEscrowAsync(CreateEscrowRequest request)
    {
        var escrow = new EscrowTransaction
        {
            Buyer = request.BuyerAddress.ToLowerInvariant(),
            Seller = request.SellerAddress.ToLowerInvariant(),
            Arbiter = request.ArbiterAddress.ToLowerInvariant(),
            Amount = request.Amount,
            ArbiterFeeBps = request.ArbiterFeeBps,
            Description = request.Description,
            State = EscrowState.Funded, // Created + Funded in one step (matches contract)
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        escrow.Events.Add(new EscrowEvent
        {
            Action = "Created",
            PerformedBy = escrow.Buyer,
            Details = $"Escrow created for {EthFormat.FormatEthWithUnit(request.Amount)}",
            Timestamp = DateTime.UtcNow
        });
        escrow.Events.Add(new EscrowEvent
        {
            Action = "Funded",
            PerformedBy = escrow.Buyer,
            Details = $"Funded with {EthFormat.FormatEthWithUnit(request.Amount)}",
            Timestamp = DateTime.UtcNow.AddSeconds(1)
        });

        _db.Escrows.Add(escrow);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Demo: Created escrow #{Id} — {Buyer} → {Seller}, {Amount} ETH",
            escrow.Id, escrow.Buyer, escrow.Seller, escrow.Amount);
        return escrow;
    }

    public async Task<EscrowTransaction> FundEscrowAsync(int id, string funderAddress)
    {
        var escrow = await GetOrThrowAsync(id);
        EnsureState(escrow, EscrowState.Created);

        escrow.State = EscrowState.Funded;
        escrow.UpdatedAt = DateTime.UtcNow;
        AddEvent(escrow, "Funded", funderAddress, $"Funded with {EthFormat.FormatEthWithUnit(escrow.Amount)}");

        await _db.SaveChangesAsync();
        return escrow;
    }

    public async Task<EscrowTransaction> ConfirmShipmentAsync(int id, string sellerAddress)
    {
        var escrow = await GetOrThrowAsync(id);
        EnsureState(escrow, EscrowState.Funded);
        EnsureRole(escrow.Seller, sellerAddress, "seller");

        escrow.State = EscrowState.Shipped;
        escrow.UpdatedAt = DateTime.UtcNow;
        AddEvent(escrow, "Shipped", sellerAddress, "Seller confirmed shipment/delivery");

        await _db.SaveChangesAsync();
        _logger.LogInformation("Demo: Escrow #{Id} shipped by {Seller}", id, sellerAddress);
        return escrow;
    }

    public async Task<EscrowTransaction> ConfirmReceiptAsync(int id, string buyerAddress)
    {
        var escrow = await GetOrThrowAsync(id);
        EnsureState(escrow, EscrowState.Shipped);
        EnsureRole(escrow.Buyer, buyerAddress, "buyer");

        escrow.State = EscrowState.Completed;
        escrow.UpdatedAt = DateTime.UtcNow;

        var fee = escrow.CalculateArbiterFee();
        var payout = escrow.CalculateSellerPayout();
        AddEvent(escrow, "Completed", buyerAddress,
            $"Buyer confirmed receipt. Seller receives {EthFormat.FormatEthWithUnit(payout)}, arbiter fee {EthFormat.FormatEthWithUnit(fee)}");

        await _db.SaveChangesAsync();
        _logger.LogInformation("Demo: Escrow #{Id} completed. Seller payout: {Payout} ETH", id, payout);
        return escrow;
    }

    public async Task<EscrowTransaction> RaiseDisputeAsync(int id, string raisedBy, string reason)
    {
        var escrow = await GetOrThrowAsync(id);
        if (escrow.State != EscrowState.Funded && escrow.State != EscrowState.Shipped)
            throw new InvalidOperationException("Cannot dispute in current state");

        if (!IsParty(escrow, raisedBy))
            throw new UnauthorizedAccessException("Only buyer or seller can raise dispute");

        escrow.State = EscrowState.Disputed;
        escrow.DisputeReason = reason;
        escrow.UpdatedAt = DateTime.UtcNow;
        AddEvent(escrow, "Disputed", raisedBy, $"Dispute raised: {reason}");

        await _db.SaveChangesAsync();
        _logger.LogInformation("Demo: Escrow #{Id} disputed by {Address}: {Reason}", id, raisedBy, reason);
        return escrow;
    }

    public async Task<EscrowTransaction> ResolveDisputeAsync(int id, string arbiterAddress, int buyerPercent)
    {
        var escrow = await GetOrThrowAsync(id);
        EnsureState(escrow, EscrowState.Disputed);
        EnsureRole(escrow.Arbiter, arbiterAddress, "arbiter");

        if (buyerPercent < 0 || buyerPercent > 100)
            throw new ArgumentException("Buyer percent must be 0-100");

        escrow.State = EscrowState.Resolved;
        escrow.UpdatedAt = DateTime.UtcNow;

        var buyerPayout = escrow.Amount * buyerPercent / 100m;
        var sellerTotal = escrow.Amount - buyerPayout;
        var fee = escrow.CalculateFeeOn(sellerTotal);
        var sellerPayout = sellerTotal - fee;

        AddEvent(escrow, "Resolved", arbiterAddress,
            $"Dispute resolved: Buyer {buyerPercent}% ({EthFormat.FormatEthWithUnit(buyerPayout)}), Seller ({EthFormat.FormatEthWithUnit(sellerPayout)}), Arbiter fee ({EthFormat.FormatEthWithUnit(fee)})");

        await _db.SaveChangesAsync();
        _logger.LogInformation("Demo: Escrow #{Id} resolved — buyer {BuyerPct}%", id, buyerPercent);
        return escrow;
    }

    public async Task<EscrowTransaction> CancelEscrowAsync(int id, string buyerAddress)
    {
        var escrow = await GetOrThrowAsync(id);
        EnsureState(escrow, EscrowState.Funded);
        EnsureRole(escrow.Buyer, buyerAddress, "buyer");

        escrow.State = EscrowState.Cancelled;
        escrow.UpdatedAt = DateTime.UtcNow;
        AddEvent(escrow, "Cancelled", buyerAddress, $"Escrow cancelled. {EthFormat.FormatEthWithUnit(escrow.Amount)} refunded to buyer");

        await _db.SaveChangesAsync();
        _logger.LogInformation("Demo: Escrow #{Id} cancelled, refund to {Buyer}", id, buyerAddress);
        return escrow;
    }

    // ── Helpers ──────────────────────────────────────────────────

    private async Task<EscrowTransaction> GetOrThrowAsync(int id)
    {
        var escrow = await _db.Escrows
            .Include(e => e.Events)
            .FirstOrDefaultAsync(e => e.Id == id);
        return escrow ?? throw new KeyNotFoundException($"Escrow #{id} not found");
    }

    private static void EnsureState(EscrowTransaction escrow, EscrowState expected)
    {
        if (escrow.State != expected)
            throw new InvalidOperationException($"Expected state {expected}, got {escrow.State}");
    }

    private static void EnsureRole(string expected, string actual, string role)
    {
        if (!string.Equals(expected, actual, StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedAccessException($"Caller is not the {role}");
    }

    private static bool IsParty(EscrowTransaction escrow, string address)
    {
        return string.Equals(escrow.Buyer, address, StringComparison.OrdinalIgnoreCase)
            || string.Equals(escrow.Seller, address, StringComparison.OrdinalIgnoreCase);
    }

    private static void AddEvent(EscrowTransaction escrow, string action, string performedBy, string details)
    {
        escrow.Events.Add(new EscrowEvent
        {
            Action = action,
            PerformedBy = performedBy.ToLowerInvariant(),
            Details = details,
            Timestamp = DateTime.UtcNow
        });
    }
}
