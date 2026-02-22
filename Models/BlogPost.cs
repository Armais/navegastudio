namespace NavegaStudio.Models;

public class BlogPost
{
    public string Slug { get; set; } = string.Empty;
    public string TitleEs { get; set; } = string.Empty;
    public string TitleEn { get; set; } = string.Empty;
    public string SummaryEs { get; set; } = string.Empty;
    public string SummaryEn { get; set; } = string.Empty;
    public string ContentEs { get; set; } = string.Empty;
    public string ContentEn { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public List<string> Tags { get; set; } = new();

    public string GetTitle(string culture) => culture.StartsWith("es") ? TitleEs : TitleEn;
    public string GetSummary(string culture) => culture.StartsWith("es") ? SummaryEs : SummaryEn;
    public string GetContent(string culture) => culture.StartsWith("es") ? ContentEs : ContentEn;
}
