using MimeKit;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit.Text;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using ManagerCenter.UserManager.Abstractions;
using ManagerCenter.UserManager.Abstractions.UserManagerInterfaces;

namespace ManagerCenter.UserManager.EntityFrameworkCore
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string[] toEmails, string subject, string htmlMessage)
        {
            if (toEmails is null)
            {
                throw new ArgumentNullException(nameof(toEmails));
            }

            if (string.IsNullOrEmpty(subject))
            {
                throw new ArgumentException($"'{nameof(subject)}' cannot be null or empty", nameof(subject));
            }

            if (string.IsNullOrEmpty(htmlMessage))
            {
                throw new ArgumentException($"'{nameof(htmlMessage)}' cannot be null or empty", nameof(htmlMessage));
            }

            var message = new MimeMessage();
			message.From.Add(MailboxAddress.Parse("943866961@qq.com"));
            foreach (var toEmail in toEmails)
            {
				message.To.Add(MailboxAddress.Parse(toEmail));
			}

            message.Subject = subject;
			message.Body = new TextPart(TextFormat.Html)
			{
				Text = htmlMessage
			};

			using (var client = new SmtpClient())
			{
				await client.ConnectAsync("smtp.qq.com", 587, false).ConfigureAwait(false);

				//QQ邮件授权码  dglslhzvyexibebc
				// Note: only needed if the SMTP server requires authentication
				client.Authenticate("943866961@qq.com", "daoyjawyifxabejf");

				await client.SendAsync(message).ConfigureAwait(false);
                await client.DisconnectAsync(true).ConfigureAwait(false);
			}
		}
    }
}
