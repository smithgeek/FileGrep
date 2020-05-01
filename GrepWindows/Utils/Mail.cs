using System;
using System.Collections.Generic;
using System.Net.Mail;
using Smithgeek.Extensions;

namespace Smithgeek.Mail
{
    public static class Mail
    {
        public static void sendMail(String from, String to, String subject, String body, String smtpServer)
        {
            List<String> address = new List<string>();
            address.Add(to);
            sendMail(from, address, subject, body, smtpServer);
        }

        public static void sendMail(String from, List<String> to, String subject, String body, String smtpServer)
        {
            if (!from.isEmpty() && !to.isEmpty() && !smtpServer.isEmpty())
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(from);
                foreach (String address in to)
                {
                    mail.To.Add(address);
                }
                mail.Subject = subject;
                mail.Body = body;
                SmtpClient smtp = new SmtpClient(smtpServer);
                smtp.Send(mail);
            }
        }
    }
}