using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Emails
{
    class EmailSender : IEmailSender
    {
        private ILogger<EmailSender> _logger;

        IConfiguration _configurations;
        public EmailSender(ILogger<EmailSender> logger, IConfiguration configurations)
        {
            _logger = logger;
            _configurations = configurations;
        }

        public async Task SendEmailByEmailAsync(string email, Email emailToSend)
        {
            try
            {
                #region formatter
                string text = emailToSend.Subject + emailToSend.TextBody;
                string html = emailToSend.HtmlBody;
                #endregion

                var emailToSendFrom = _configurations["EmailConfigs:Email"];
                var password = _configurations["EmailConfigs:Password"];
                var host = _configurations["EmailConfigs:Host"];
                var port = _configurations["EmailConfigs:Port"];

                MailMessage msg = new MailMessage
                {
                    From = new MailAddress(emailToSendFrom),
                    Subject = emailToSend.Subject,
                };
                msg.To.Add(new MailAddress(email));

                if (!string.IsNullOrEmpty(emailToSend.TextBody))
                {
                    msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
                }

                if (!string.IsNullOrEmpty(emailToSend.HtmlBody))
                {
                    msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));
                    msg.IsBodyHtml = true;
                }
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(emailToSendFrom, password);

                SmtpClient smtpClient = new SmtpClient(host, Convert.ToInt32(port));
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = credentials;
                smtpClient.EnableSsl = true;
                await smtpClient.SendMailAsync(msg);
                _logger.LogInformation($"A message was sent successfully to {email}",
                    $"The message subject was: {emailToSend.Subject}",
                    $"The message html body was: {emailToSend.HtmlBody}",
                    $"The message text body was: {emailToSend.TextBody}");
            }
            catch (Exception e)
            {
                _logger.LogError($"There were an error while sending an email to: {email}",
                    $"The email subject was: {emailToSend.Subject}",
                    $"The email Html body was: {emailToSend.HtmlBody}",
                    $"The email Text body was: {emailToSend.TextBody}",
                    $"The Exception details",
                    e.ToString());
            }
        }

    }
}
