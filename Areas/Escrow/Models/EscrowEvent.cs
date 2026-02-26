using System.ComponentModel.DataAnnotations;

namespace NavegaStudio.Areas.Escrow.Models;

public class EscrowEvent
{
    [Key]
    public int Id { get; set; }

    public int EscrowTransactionId { get; set; }

    public string Action { get; set; } = "";

    public string PerformedBy { get; set; } = "";

    public string? Details { get; set; }

    public string? TxHash { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // Navigation
    public EscrowTransaction? EscrowTransaction { get; set; }
}
