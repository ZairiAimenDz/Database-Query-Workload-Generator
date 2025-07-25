using DatabaseWorkloadQueryGenerator.Core.Entities.Users;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseWorkloadQueryGenerator.Infrastructure.Identity;


public class CustomEmailConfirmationCodeProvider : IUserTwoFactorTokenProvider<ApplicationUser>
{
    public Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<ApplicationUser> manager, ApplicationUser user)
    {
        return Task.FromResult(true);
    }

    public async Task<string> GenerateAsync(string purpose, UserManager<ApplicationUser> manager, ApplicationUser user)
    {
        string code = Guid.NewGuid().ToString().Replace("-", "")[..6];

        // Store the email confirmation token in the user's account for later validation
        user.EmailConfirmationCode = code;
        await manager.UpdateAsync(user);

        return code;
    }

    public async Task<bool> ValidateAsync(string purpose, string token, UserManager<ApplicationUser> manager, ApplicationUser user)
    {
        // Retrieve the user's stored email confirmation token
        string storedToken = user.EmailConfirmationCode;
        bool valid = token == storedToken;
        if (valid)
        {
            // Clear the token after validation
            user.EmailConfirmationCode = Guid.NewGuid().ToString().Replace("-", "")[..6];
            await manager.UpdateAsync(user);
        }
        // Compare the provided token to the stored token
        return valid;
    }
}
