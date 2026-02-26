using System.ComponentModel.DataAnnotations;

namespace NavegaStudio.Areas.Escrow.Models;

public enum EscrowState
{
    Created,
    Funded,
    Shipped,
    Completed,
    Disputed,
    Resolved,
    Cancelled
}

public class EscrowTransaction
{
    [Key]
    public int Id { get; set; }

    /// <summary>On-chain escrow ID (null in demo mode)</summary>
    public uint? OnChainId { get; set; }

    [Required]
    public string Buyer { get; set; } = "";

    [Required]
    public string Seller { get; set; } = "";

    [Required]
    public string Arbiter { get; set; } = "";

    /// <summary>Amount in ETH</summary>
    public decimal Amount { get; set; }

    /// <summary>Arbiter fee in basis points (100 = 1%)</summary>
    public int ArbiterFeeBps { get; set; }

    public string Description { get; set; } = "";

    public EscrowState State { get; set; } = EscrowState.Created;

    public string? DisputeReason { get; set; }

    public string? TxHash { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public List<EscrowEvent> Events { get; set; } = new();

    // ── Fee calculations ──────────────────────────────────────

    public decimal CalculateArbiterFee() => Amount * ArbiterFeeBps / 10000m;

    public decimal CalculateSellerPayout() => Amount - CalculateArbiterFee();

    /// <summary>Calculate arbiter fee on a sub-amount (used in dispute resolution)</summary>
    public decimal CalculateFeeOn(decimal subAmount) => subAmount * ArbiterFeeBps / 10000m;
}
