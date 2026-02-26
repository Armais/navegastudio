namespace NavegaStudio.Areas.Escrow.Models;

public class EscrowViewModel
{
    public EscrowTransaction Escrow { get; set; } = null!;

    public string CurrentUserAddress { get; set; } = "";

    public bool IsBuyer => string.Equals(CurrentUserAddress, Escrow.Buyer, StringComparison.OrdinalIgnoreCase);
    public bool IsSeller => string.Equals(CurrentUserAddress, Escrow.Seller, StringComparison.OrdinalIgnoreCase);
    public bool IsArbiter => string.Equals(CurrentUserAddress, Escrow.Arbiter, StringComparison.OrdinalIgnoreCase);
    public bool IsParty => IsBuyer || IsSeller || IsArbiter;

    // State-based action availability
    public bool CanShip => IsSeller && Escrow.State == EscrowState.Funded;
    public bool CanConfirm => IsBuyer && Escrow.State == EscrowState.Shipped;
    public bool CanDispute => (IsBuyer || IsSeller) && (Escrow.State == EscrowState.Funded || Escrow.State == EscrowState.Shipped);
    public bool CanResolve => IsArbiter && Escrow.State == EscrowState.Disputed;
    public bool CanCancel => IsBuyer && Escrow.State == EscrowState.Funded;

    public string StateBadgeClass => Escrow.State switch
    {
        EscrowState.Created => "bg-secondary",
        EscrowState.Funded => "bg-primary",
        EscrowState.Shipped => "bg-warning text-dark",
        EscrowState.Completed => "bg-success",
        EscrowState.Disputed => "bg-danger",
        EscrowState.Resolved => "bg-info",
        EscrowState.Cancelled => "bg-dark",
        _ => "bg-secondary"
    };

    public decimal ArbiterFeePercent => Escrow.ArbiterFeeBps / 100m;
    public decimal ArbiterFeeEth => Escrow.CalculateArbiterFee();
    public decimal SellerPayout => Escrow.CalculateSellerPayout();
}

public class DashboardViewModel
{
    public List<EscrowTransaction> MyEscrows { get; set; } = new();
    public string CurrentUserAddress { get; set; } = "";
    public bool IsDemoMode { get; set; }
    public CreateEscrowRequest CreateForm { get; set; } = new();
}
