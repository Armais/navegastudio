using NavegaStudio.Models;

namespace NavegaStudio.Services;

public interface IEmailService
{
    Task<bool> SendContactNotificationAsync(ContactRequest request);
    Task<bool> SendContactConfirmationAsync(ContactRequest request);
}
