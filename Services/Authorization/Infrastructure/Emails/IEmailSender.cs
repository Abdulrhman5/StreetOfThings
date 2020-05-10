using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Emails
{
    public interface IEmailSender
    {
        Task SendEmailByEmailAsync(string email, Email emailToSend);
    }

    public class Email
    {
        public string Subject { get; set; }

        public string HtmlBody { get; set; }

        public string TextBody { get; set; }
    }
}
