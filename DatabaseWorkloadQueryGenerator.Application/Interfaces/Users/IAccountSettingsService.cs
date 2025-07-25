using DatabaseWorkloadQueryGenerator.Application.Models.UserEntities;
using DatabaseWorkloadQueryGenerator.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseWorkloadQueryGenerator.Application.Interfaces.Users
{

    public interface IAccountSettingsService
    {
        Task<Response> SendPasswordResetCode(Guid UserId);
        Task<Response> SendEmailConfirmationLink(string email, string CallBackUrl);
        Task<Response> SendPhoneConfirmationCode(Guid UserId);
        Task<Response> SendPasswordResetLink(string email, string CallbackUrl);
        Task<Response> SendEmailResetCode(Guid UserId, SendEmailResetCodeDTO Email);
        Task<Response> SendPhoneNewCode(Guid UserId, SendPhoneCodeDTO Phone);
        Task<Response> ResetPasswordWithCode(Guid UserId, PasswordChangeWithCodeDTO PasswordChange);
        Task<Response> ChangePasswordWithOldPass(Guid UserId, PasswordResetWithOldPassDTO PasswordChange);
        Task<Response> ChangeEmailWithCode(Guid UserId, EmailChangeWithCodeDTO EmailChange);
        Task<Response> ChangePhoneWithCode(Guid UserId, PhoneChangeWithCodeDTO PhoneChange);
        Task<Response> DeleteUserData(Guid UserId, DataDeletionConfirmationDTO DeletionConfirmation);
    }
}
