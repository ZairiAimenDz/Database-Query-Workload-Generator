using DatabaseWorkloadQueryGenerator.Application.Wrappers;
using DatabaseWorkloadQueryGenerator.Core.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseWorkloadQueryGenerator.Application.Interfaces.Notifications
{
    public interface IGeneralEmailSender
    {
        Task<bool> SendEmail(MailData mail, EmailSettings email, CancellationToken ct, bool WithWrapper = false);
    }
}
