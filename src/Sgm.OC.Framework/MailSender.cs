using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Sgm.OC.Framework
{
    public class MailSender
    {

        public void Send(string subject, string fromAddress, string toAddress, string body, string attachentName, byte[] attachmentData = null,bool isBodyHtml = false)
        {
            MemoryStream memoryStream = new MemoryStream(attachmentData);

            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient(Sgm.OC.Framework.Settings.SMTPServerAddress);
            mail.From = new MailAddress(fromAddress);
            mail.IsBodyHtml = isBodyHtml;
            mail.To.Add(toAddress);
            mail.Subject = subject;
            mail.Body = body;

            if (attachmentData != null && attachmentData.Length > 0)
            {
                System.Net.Mail.Attachment attachment = new Attachment(memoryStream, attachentName);
                mail.Attachments.Add(attachment);
            }

            SmtpServer.Port = Sgm.OC.Framework.Settings.SMTPServerPort;
            SmtpServer.Credentials = new System.Net.NetworkCredential(Sgm.OC.Framework.Settings.SMTPServerUsername, Sgm.OC.Framework.Settings.SMTPServerPassword);
            SmtpServer.EnableSsl = Sgm.OC.Framework.Settings.SMTPServerUsesSSL;

            SmtpServer.Send(mail);
        }

        public Task SenAsync(string subject, string fromAddress, string toAddress, string body, string attachmentName, byte[] attachmentData = null)
        {
            return Task.Run(() =>
            {
                Send(subject, fromAddress, toAddress, body, attachmentName, attachmentData);
            });
        }

    }

}
