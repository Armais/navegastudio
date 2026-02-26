using System.ComponentModel.DataAnnotations;

namespace NavegaStudio.Areas.Escrow.Models;

public class CreateEscrowRequest
{
    [Required(ErrorMessage = "Seller address is required")]
    [RegularExpression(@"^0x[0-9a-fA-F]{40}$", ErrorMessage = "Invalid Ethereum address")]
    public string SellerAddress { get; set; } = "";

    [Required(ErrorMessage = "Arbiter address is required")]
    [RegularExpression(@"^0x[0-9a-fA-F]{40}$", ErrorMessage = "Invalid Ethereum address")]
    public string ArbiterAddress { get; set; } = "";

    [Required(ErrorMessage = "Amount is required")]
    [Range(0.0001, 1000, ErrorMessage = "Amount must be between 0.0001 and 1000 ETH")]
    public decimal Amount { get; set; }

    [Range(0, 1000, ErrorMessage = "Arbiter fee must be between 0 and 1000 basis points (0-10%)")]
    public int ArbiterFeeBps { get; set; } = 100; // 1% default

    [Required(ErrorMessage = "Description is required")]
    [StringLength(500, MinimumLength = 10, ErrorMessage = "Description must be 10-500 characters")]
    public string Description { get; set; } = "";

    /// <summary>Buyer address (set from connected wallet or demo)</summary>
    [Required(ErrorMessage = "Buyer address is required")]
    [RegularExpression(@"^0x[0-9a-fA-F]{40}$", ErrorMessage = "Invalid Ethereum address")]
    public string BuyerAddress { get; set; } = "";
}
