using Microsoft.AspNetCore.Identity;

namespace DatabaseWorkloadQueryGenerator.Core.Entities.Users;

public class ApplicationUser : IdentityUser<Guid>
{
    /// <summary>
    /// User's Registration Date
    /// </summary>
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The Confirmation Code Sent By Email For Custom Email Confirmation
    /// </summary>
    public string EmailConfirmationCode { get; set; } = string.Empty;
    /// <summary>
    /// The Password Reset Code Sent By Email For Custom Password Reset
    /// </summary>
    public string PasswordResetCode { get; set; } = string.Empty;
    /// <summary>
    /// The Confirmation Code Sent By SMS For Custom Phone Confirmation
    /// </summary>
    public string PhoneConfirmationCode { get; set; } = string.Empty;
}