using NavegaStudio.Areas.Escrow.Models;

namespace NavegaStudio.Areas.Escrow.Services;

public interface IEscrowService
{
    bool IsDemoMode { get; }
    Task<EscrowTransaction?> GetEscrowAsync(int id);
    Task<List<EscrowTransaction>> GetUserEscrowsAsync(string address);
    Task<List<EscrowTransaction>> GetAllEscrowsAsync();
    Task<EscrowTransaction> CreateEscrowAsync(CreateEscrowRequest request);
    Task<EscrowTransaction> FundEscrowAsync(int id, string funderAddress);
    Task<EscrowTransaction> ConfirmShipmentAsync(int id, string sellerAddress);
    Task<EscrowTransaction> ConfirmReceiptAsync(int id, string buyerAddress);
    Task<EscrowTransaction> RaiseDisputeAsync(int id, string raisedBy, string reason);
    Task<EscrowTransaction> ResolveDisputeAsync(int id, string arbiterAddress, int buyerPercent);
    Task<EscrowTransaction> CancelEscrowAsync(int id, string buyerAddress);
}
