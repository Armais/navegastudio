namespace NavegaStudio.Areas.Escrow.Models;

public class EthereumSettings
{
    public string RpcUrl { get; set; } = "";
    public string ContractAddress { get; set; } = "";
    public int ChainId { get; set; } = 11155111; // Sepolia
    public string ChainName { get; set; } = "Sepolia";
}
