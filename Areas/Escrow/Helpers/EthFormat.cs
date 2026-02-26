namespace NavegaStudio.Areas.Escrow.Helpers;

public static class EthFormat
{
    /// <summary>Format a decimal ETH value removing trailing zeros (e.g. 1.5000 -> "1.5")</summary>
    public static string FormatEth(decimal value)
    {
        return Math.Round(value, 8).ToString("G29");
    }

    /// <summary>Format a decimal ETH value with " ETH" suffix (e.g. 1.5000 -> "1.5 ETH")</summary>
    public static string FormatEthWithUnit(decimal value)
    {
        return $"{FormatEth(value)} ETH";
    }
}
