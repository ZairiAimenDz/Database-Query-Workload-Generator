using DatabaseWorkloadQueryGenerator.Application.Interfaces.Notifications;
using DatabaseWorkloadQueryGenerator.Application.Wrappers;
using DatabaseWorkloadQueryGenerator.Core.Constants;
using FluentEmail.Core;
using FluentEmail.Smtp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseWorkloadQueryGenerator.Infrastructure.Services.Notifications
{
    public class GeneralEmailSender : IGeneralEmailSender
    {
        public GeneralEmailSender()
        { }

        public async Task<bool> SendEmail(MailData mailData, EmailSettings _settings, CancellationToken ct, bool WithWrapper = false)
        {
            try
            {
                // Configure FluentEmail with SMTP settings
                var smtp = new SmtpSender(() => new SmtpClient(_settings.SmtpServer)
                {
                    EnableSsl = _settings.EnableSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Port = _settings.SmtpPort,
                    Credentials = new NetworkCredential(_settings.SmtpUsername, _settings.SmtpPassword)
                });

                Email.DefaultSender = smtp;

                // Prepare email body
                string emailBody = mailData.Body;

                if (WithWrapper)
                {
                    string emailTemplate = "";
                    using (StreamReader SourceReader = System.IO.File.OpenText("wwwroot/Templates/email_wrapper.html"))
                    {
                        emailTemplate = await SourceReader.ReadToEndAsync();
                    }
                    emailBody = emailTemplate.Replace("{{title}}", mailData.Subject)
                                           .Replace("{{content}}", mailData.Body);
                }

                // Build the email using FluentEmail
                var email = Email
                    .From(_settings.FromEmail, _settings.FromName)
                    .Subject(mailData.Subject)
                    .Body(emailBody, true); // true indicates HTML body

                // Add recipients
                foreach (string mailAddress in mailData.To)
                {
                    email.To(mailAddress);
                }

                // Add Reply-To if specified
                if (!string.IsNullOrEmpty(mailData.ReplyTo))
                {
                    email.ReplyTo(mailData.ReplyTo, mailData.ReplyToName);
                }

                // Add BCC recipients
                if (mailData.Bcc != null)
                {
                    foreach (string mailAddress in mailData.Bcc.Where(x => !string.IsNullOrWhiteSpace(x)))
                    {
                        email.BCC(mailAddress.Trim());
                    }
                }

                // Add CC recipients
                if (mailData.Cc != null)
                {
                    foreach (string mailAddress in mailData.Cc.Where(x => !string.IsNullOrWhiteSpace(x)))
                    {
                        email.CC(mailAddress.Trim());
                    }
                }

                // Send the email
                var result = await email.SendAsync(ct);

                return result.Successful;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
