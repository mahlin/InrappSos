using System;
using System.Collections.Generic;
using System.IO;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using InrappSos.ApplicationService.Helpers;

namespace ErrorLogNotifier
{
    public class NotificationManager
    {
        MailHelper _mailHelper;
        private string _mailSender;
        private string _mailReciever;

        public NotificationManager()
        {
            _mailHelper = new MailHelper();
            _mailSender = ConfigurationManager.AppSettings["MailSender"];
            _mailReciever = ConfigurationManager.AppSettings["MailReciever"];
        }


        public void Notify()
        {
            string yesterday = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

            string _errorLogPath = ConfigurationManager.AppSettings["errorLogFilePath"];
            DirectoryInfo dir = new DirectoryInfo(_errorLogPath);
            List<FileInfo> filesInFolder = dir.GetFiles().OrderByDescending(p => p.CreationTime).ToList();
            var fullPath = Path.Combine(_errorLogPath + yesterday + "_error.txt");

            //Check if errorlog for yesterday exists. If so - email it to administrator-adress
            if (File.Exists(fullPath))
            {
                var mailRecipients = new List<string>();
                mailRecipients.Add(_mailReciever);
                Console.WriteLine("The file exists. Sending email.");
                string subject = "Fellogg för SFTP-filer " + yesterday;
                string body = "Hej! </br>";
                body += "Fel inträffade under gårdagens hantering av filer inlämnade via SFTP. Se bifogad fellogg. <br>";

                MailMessage msg = new MailMessage();
                var recipientsStr = mailRecipients.Aggregate((a, b) => a + ", " + b);

                foreach (var address in mailRecipients)
                {
                    msg.To.Add(address);
                }

                MailAddress fromMail = new MailAddress(_mailSender);
                msg.From = fromMail;

                msg.Subject = subject;
                msg.Body = body;
                msg.Attachments.Add(new Attachment(fullPath));

                _mailHelper.SendEmail(msg);
            }

        }
    }
}
