using System.ComponentModel.DataAnnotations;

namespace NavegaStudio.Areas.Escrow.Models;

public class ActionRequest
{
    [Required(ErrorMessage = "Address is required")]
    [RegularExpression(@"^0x[0-9a-fA-F]{40}$", ErrorMessage = "Invalid Ethereum address")]
    public string Address { get; set; } = "";
}

public class DisputeRequest
{
    [Required(ErrorMessage = "Address is required")]
    [RegularExpression(@"^0x[0-9a-fA-F]{40}$", ErrorMessage = "Invalid Ethereum address")]
    public string Address { get; set; } = "";

    [Required(ErrorMessage = "Reason is required")]
    [StringLength(500, MinimumLength = 5, ErrorMessage = "Reason must be 5-500 characters")]
    public string Reason { get; set; } = "";
}

public class ResolveRequest
{
    [Required(ErrorMessage = "Address is required")]
    [RegularExpression(@"^0x[0-9a-fA-F]{40}$", ErrorMessage = "Invalid Ethereum address")]
    public string Address { get; set; } = "";

    [Range(0, 100, ErrorMessage = "Buyer percent must be between 0 and 100")]
    public int BuyerPercent { get; set; }
}
