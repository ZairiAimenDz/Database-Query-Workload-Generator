using DatabaseWorkloadQueryGenerator.Core.Entities.Users;
using Microsoft.AspNetCore.Identity;

namespace DatabaseWorkloadQueryGenerator.Infrastructure.Identity;

public class CustomResetTokenProvider : IUserTwoFactorTokenProvider<ApplicationUser>
{
    public Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<ApplicationUser> manager, ApplicationUser user)
    {
        return Task.FromResult(true);
    }

    public async Task<string> GenerateAsync(string purpose, UserManager<ApplicationUser> manager, ApplicationUser user)
    {
        string code = Guid.NewGuid().ToString().Replace("-", "")[..6];

        // Save the code to the user object
        user.PasswordResetCode = code;
        await manager.UpdateAsync(user);

        return code;
    }

    public async Task<bool> ValidateAsync(string purpose, string token, UserManager<ApplicationUser> manager, ApplicationUser user)
    {
        bool isValid = user.PasswordResetCode == token;

        if (isValid)
        {
            // Clear the token after validation
            user.PasswordResetCode = Guid.NewGuid().ToString().Replace("-", "")[..6];
            await manager.UpdateAsync(user);
        }

        return isValid;
    }
}