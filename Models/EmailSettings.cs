namespace NavegaStudio.Models;

public class EmailSettings
{
    public string SmtpHost { get; set; } = "smtp.gmail.com";
    public int SmtpPort { get; set; } = 587;
    public string SenderEmail { get; set; } = string.Empty;
    public string SenderName { get; set; } = "NavegaStudio";
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string RecipientEmail { get; set; } = string.Empty;
    public bool SendConfirmation { get; set; } = true;
}
