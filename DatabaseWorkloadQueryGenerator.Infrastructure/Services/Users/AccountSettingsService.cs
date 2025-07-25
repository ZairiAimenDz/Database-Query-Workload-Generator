using DatabaseWorkloadQueryGenerator.Application.Interfaces.Notifications;
using DatabaseWorkloadQueryGenerator.Application.Interfaces.Users;
using DatabaseWorkloadQueryGenerator.Application.Models.UserEntities;
using DatabaseWorkloadQueryGenerator.Application.Wrappers;
using DatabaseWorkloadQueryGenerator.Core.Constants;
using DatabaseWorkloadQueryGenerator.Core.Entities.Users;
using DatabaseWorkloadQueryGenerator.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseWorkloadQueryGenerator.Infrastructure.Services.Users
{

    public class AccountSettingsService : IAccountSettingsService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOptions<EmailSettings> _options;
        private readonly IGeneralEmailSender _sender;

        public AccountSettingsService(ApplicationDbContext context, IMemoryCache cache, UserManager<ApplicationUser> userManager, IOptions<EmailSettings> options, IGeneralEmailSender sender)
        {
            this._context = context;
            this._cache = cache;
            this._userManager = userManager;
            this._options = options;
            this._sender = sender;
        }

        public async Task<Response> ChangeEmailWithCode(Guid UserId, EmailChangeWithCodeDTO EmailChange)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(UserId.ToString());
            if (user == null)
            {
                return Response.Fail("User Not Found");
            }
            IdentityResult result = await _userManager.ChangeEmailAsync(user, EmailChange.NewEmail, EmailChange.Token);

            if (result.Succeeded)
            {
                await _userManager.SetUserNameAsync(user, EmailChange.NewEmail);
                await _userManager.UpdateNormalizedUserNameAsync(user);
                _cache.Remove($"user:{UserId}");
                _cache.Set($"user:{UserId}", user);
            }

            return result.Succeeded
                ? Response.Success(true)
                : Response.Fail(result.Errors.Select(x => x.Description).ToList());
        }

        public async Task<Response> ChangePasswordWithOldPass(Guid UserId, PasswordResetWithOldPassDTO PasswordChange)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(UserId.ToString());
            if (user == null)
            {
                return Response.Fail("User Not Found");
            }
            IdentityResult result = await _userManager.ChangePasswordAsync(user, PasswordChange.OldPassword, PasswordChange.NewPassword);
            return result.Succeeded
                ? Response.Success(true)
                : Response.Fail(result.Errors.Select(x => x.Description).ToList());
        }

        public async Task<Response> ChangePhoneWithCode(Guid UserId, PhoneChangeWithCodeDTO PhoneChange)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(UserId.ToString());
            if (user == null)
            {
                return Response.Fail("User Not Found");
            }
            IdentityResult result = await _userManager.ChangePhoneNumberAsync(user, PhoneChange.PhoneNumber, PhoneChange.Token);

            if (result.Succeeded)
            {
                _cache.Remove($"user:{UserId}");
                _cache.Set($"user:{UserId}", user);
            }

            return result.Succeeded
                ? Response.Success()
                : Response.Fail([.. result.Errors.Select(x => x.Description)]);
        }

        public async Task<Response> DeleteUserData(Guid UserId, DataDeletionConfirmationDTO DeletionConfirmation)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(UserId.ToString());
            if (user == null)
            {
                return Response.Success(false);
            }
            if (await _userManager.CheckPasswordAsync(user, DeletionConfirmation.Password))
            {
                // TODO : Cancel The Subscription
                IdentityResult result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    return Response.Fail(result.Errors.Select(x => x.Description).ToList());
                }
                return Response.Success(true);
            }
            else
            {
                return Response.Fail("Password is incorrect");
            }
        }

        public async Task<Response> ResetPasswordWithCode(Guid UserId, PasswordChangeWithCodeDTO PasswordChange)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(UserId.ToString());
            if (user == null)
            {
                return Response.Fail("User Not Found");
            }
            IdentityResult result = await _userManager.ResetPasswordAsync(user, PasswordChange.Token, PasswordChange.NewPassword);
            return result.Succeeded
                ? Response.Success(true)
                : Response.Fail(result.Errors.Select(x => x.Description).ToList());
        }

        public async Task<Response> SendEmailConfirmationLink(string email, string CallBackUrl)
        {
            bool sending = await _sender.SendEmail(new([email], "Email Confirmation", $"Please Confirm Your Email By Clicking <a href='{CallBackUrl}'>Here</a>"), _options.Value, CancellationToken.None, true);
            return Response.Success(sending);
        }

        public async Task<Response> SendEmailResetCode(Guid UserId, SendEmailResetCodeDTO Email)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(UserId.ToString());
            if (user == null)
            {
                return Response.Fail("User Not Found");
            }
            string token = await _userManager.GenerateChangeEmailTokenAsync(user, Email.NewEmail);
            bool sending = await _sender.SendEmail(new([Email.NewEmail], "Email Change Confirmation", $"Your Email Change Confirmation Code is : {token}"), _options.Value, CancellationToken.None, true);
            return Response.Success(sending);
        }

        public async Task<Response> SendPasswordResetCode(Guid UserId)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(UserId.ToString());
            if (user == null)
            {
                return Response.Fail("User Not Found");
            }
            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            bool sending = await _sender.SendEmail(new([user.Email], "Password Reset", $"Your Password Reset Code is : {token}"), _options.Value, CancellationToken.None, true);
            return Response.Success(sending);
        }

        public async Task<Response> SendPasswordResetLink(string email, string CallbackUrl)
        {
            bool sending = await _sender.SendEmail(new([email], "Password Reset", $"Please Reset Your Password By Clicking <a href='{CallbackUrl}'>Here</a>"), _options.Value, CancellationToken.None, true);
            return Response.Success(sending);
        }

        public Task<Response> SendPhoneConfirmationCode(Guid UserId)
        {
            // Not implemented yet
            return Task.FromResult(Response.Fail("Not Implemented"));
        }

        public async Task<Response> SendPhoneNewCode(Guid UserId, SendPhoneCodeDTO Phone)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(UserId.ToString());
            if (user == null)
            {
                return Response.Fail("User Not Found");
            }
            string token = await _userManager.GenerateChangePhoneNumberTokenAsync(user, Phone.PhoneNumber);
            // TODO : Replace With Real SMS Sending
            Console.WriteLine(token);
            return Response.Success(token);
        }

    }
}