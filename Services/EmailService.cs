using System.Net;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using NavegaStudio.Models;

namespace NavegaStudio.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<bool> SendContactNotificationAsync(ContactRequest request)
    {
        if (string.IsNullOrEmpty(_settings.Password))
        {
            _logger.LogWarning("Email not configured (Password empty). Skipping contact notification for {Email}", request.Email);
            return false;
        }

        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
            message.To.Add(MailboxAddress.Parse(_settings.RecipientEmail));
            message.ReplyTo.Add(new MailboxAddress(request.Name, request.Email));
            message.Subject = $"[NavegaStudio Contact] {request.Subject}";

            var encodedName = WebUtility.HtmlEncode(request.Name);
            var encodedEmail = WebUtility.HtmlEncode(request.Email);
            var encodedSubject = WebUtility.HtmlEncode(request.Subject);
            var encodedMessage = WebUtility.HtmlEncode(request.Message);
            var dateUtc = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm UTC");

            var html = $@"
<div style=""font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif; max-width: 600px; margin: 0 auto; background-color: #0a0f1e; color: #e2e8f0; border-radius: 8px; overflow: hidden;"">
    <div style=""background: linear-gradient(135deg, #3b82f6, #1d4ed8); padding: 24px 32px;"">
        <h1 style=""margin: 0; font-size: 20px; color: #ffffff;"">New Contact Form Submission</h1>
    </div>
    <div style=""padding: 32px;"">
        <table style=""width: 100%; border-collapse: collapse;"">
            <tr>
                <td style=""padding: 8px 0; color: #94a3b8; width: 100px;"">Name</td>
                <td style=""padding: 8px 0; color: #f1f5f9; font-weight: 600;"">{encodedName}</td>
            </tr>
            <tr>
                <td style=""padding: 8px 0; color: #94a3b8;"">Email</td>
                <td style=""padding: 8px 0;""><a href=""mailto:{encodedEmail}"" style=""color: #3b82f6;"">{encodedEmail}</a></td>
            </tr>
            <tr>
                <td style=""padding: 8px 0; color: #94a3b8;"">Subject</td>
                <td style=""padding: 8px 0; color: #f1f5f9;"">{encodedSubject}</td>
            </tr>
            <tr>
                <td style=""padding: 8px 0; color: #94a3b8;"">Date</td>
                <td style=""padding: 8px 0; color: #f1f5f9;"">{dateUtc}</td>
            </tr>
        </table>
        <hr style=""border: none; border-top: 1px solid #1e293b; margin: 20px 0;"" />
        <div style=""background-color: #111827; border-radius: 6px; padding: 20px; white-space: pre-wrap; color: #e2e8f0; line-height: 1.6;"">{encodedMessage}</div>
        <p style=""margin-top: 24px; color: #64748b; font-size: 13px;"">Reply directly to this email to respond to the client.</p>
    </div>
</div>";

            var text = $@"New Contact Form Submission
---
Name: {request.Name}
Email: {request.Email}
Subject: {request.Subject}
Date: {dateUtc}
---
{request.Message}
---
Reply directly to this email to respond to the client.";

            var body = new BodyBuilder
            {
                HtmlBody = html,
                TextBody = text
            };
            message.Body = body.ToMessageBody();

            await SendAsync(message);
            _logger.LogInformation("Contact notification sent for {Name} ({Email}), subject: {Subject}", request.Name, request.Email, request.Subject);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send contact notification for {Email}", request.Email);
            return false;
        }
    }

    public async Task<bool> SendContactConfirmationAsync(ContactRequest request)
    {
        if (string.IsNullOrEmpty(_settings.Password))
        {
            _logger.LogWarning("Email not configured (Password empty). Skipping confirmation to {Email}", request.Email);
            return false;
        }

        if (!_settings.SendConfirmation)
            return false;

        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
            message.To.Add(new MailboxAddress(request.Name, request.Email));
            message.Subject = "Thank you for contacting NavegaStudio";

            var encodedName = WebUtility.HtmlEncode(request.Name);
            var encodedSubject = WebUtility.HtmlEncode(request.Subject);

            var html = $@"
<div style=""font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif; max-width: 600px; margin: 0 auto; background-color: #0a0f1e; color: #e2e8f0; border-radius: 8px; overflow: hidden;"">
    <div style=""background: linear-gradient(135deg, #3b82f6, #1d4ed8); padding: 24px 32px;"">
        <h1 style=""margin: 0; font-size: 20px; color: #ffffff;"">NavegaStudio</h1>
    </div>
    <div style=""padding: 32px;"">
        <p style=""color: #f1f5f9; font-size: 16px; line-height: 1.6;"">Hi {encodedName},</p>
        <p style=""color: #e2e8f0; line-height: 1.6;"">Thank you for reaching out regarding <strong style=""color: #f1f5f9;"">""{encodedSubject}""</strong>.</p>
        <p style=""color: #e2e8f0; line-height: 1.6;"">I've received your message and will get back to you within <strong style=""color: #3b82f6;"">24 hours</strong>.</p>
        <hr style=""border: none; border-top: 1px solid #1e293b; margin: 24px 0;"" />
        <p style=""color: #64748b; font-size: 13px; line-height: 1.5;"">
            Best regards,<br />
            <strong style=""color: #f1f5f9;"">NavegaStudio</strong><br />
            High-Performance Financial Applications<br />
            <a href=""https://navegastudio.es"" style=""color: #3b82f6;"">navegastudio.es</a>
        </p>
    </div>
</div>";

            var text = $@"Hi {request.Name},

Thank you for reaching out regarding ""{request.Subject}"".

I've received your message and will get back to you within 24 hours.

Best regards,
NavegaStudio
High-Performance Financial Applications
https://navegastudio.es";

            var body = new BodyBuilder
            {
                HtmlBody = html,
                TextBody = text
            };
            message.Body = body.ToMessageBody();

            await SendAsync(message);
            _logger.LogInformation("Contact confirmation sent to {Email}", request.Email);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send confirmation to {Email}", request.Email);
            return false;
        }
    }

    private async Task SendAsync(MimeMessage message)
    {
        using var client = new SmtpClient();
        await client.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_settings.Username, _settings.Password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
