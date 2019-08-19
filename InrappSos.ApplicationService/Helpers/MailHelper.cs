using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace InrappSos.ApplicationService.Helpers
{
    public class MailHelper
    {
        private static SmtpClient _smtpClient;
        private static bool _mailLogEnabled = bool.Parse(ConfigurationManager.AppSettings["MailLogEnabled"]);
        private static string _mailLogPath = ConfigurationManager.AppSettings["SFTPWatcherMailLogFile"];
        private static readonly object _mailLogLock = new object();

        public MailHelper()
        {
            _smtpClient = new SmtpClient(ConfigurationManager.AppSettings["MailServer"]);
            if (_mailLogEnabled)
            {
                _mailLogPath = _mailLogPath.Replace(".txt", "_" + DateTime.Now.ToString("yyyyMMdd") + ".txt");
            }
        }


        public void SendEmail(string subject, string bodytext, List<string> mailRecipients, string mailSender)
        {

            string currentDate = DateTime.Now.ToString("yyyyMMdd");
            var recipientsStr = mailRecipients.Aggregate((a, b) => a + ", " + b);

            MailMessage msg = new MailMessage();

            foreach (var address in mailRecipients)
            {
                msg.To.Add(address);
            }
            MailAddress fromMail = new MailAddress(mailSender);
            msg.From = fromMail;

            msg.Subject = subject;
            msg.Body = bodytext;

            SendEmail(msg);

           // _smtpClient.Send(msg);

            // _smtpClient.SendAsync(msg, "notification");
            //Wait try to prevent to spam the server if many emails.. did run in to async problem else during testing.
            //System.Threading.Thread.Sleep(8000);

            //Logga att mailet skickats
            //Log("Mail till : " + recipientsStr + ". Rubrik: " + subject);

            //_smtpClient.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);
        }


        public void SendEmail(MailMessage msg)
        {
            //MailAddress fromMail = new MailAddress(_mailSender);
            //msg.From = fromMail;

            msg.BodyEncoding = System.Text.Encoding.UTF8;
            msg.IsBodyHtml = true;

            _smtpClient.Send(msg);

            //Logga att mailet skickats
            Log("Mail till : " + msg.To + ". Rubrik: " + msg.Subject);
        }

        // <summary>
        // Logs a message to either the console or a file
        // </summary>
        // <param name="message">The message</param>
        private static void Log(string message)
        {

            if (_mailLogEnabled)
            {
                lock (_mailLogLock)
                {
                    try
                    {
                        File.AppendAllText(_mailLogPath, DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss]") + " " + message + Environment.NewLine);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Couldn't log '" + message + "' to file log '" + _mailLogPath + "'" + Environment.NewLine + ex.ToString());
                    }
                }
            }
        }
    }
}
