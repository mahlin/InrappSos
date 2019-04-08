using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using InrappSos.ApplicationService;
using InrappSos.ApplicationService.Helpers;
using InrappSos.ApplicationService.Interface;
using InrappSos.DataAccess;
using InrappSos.DomainModel;


namespace SFTPFileHandler
{
    public class SFTPWatcher
    {
        private readonly IPortalSosService _portalService;
        private int _timeToWaitForCompleteDelivery;
        FilesHelper filesHelper;
        private static SmtpClient _smtpClient;
        private string _mailSender;
        private static bool _mailLogEnabled =bool.Parse(ConfigurationManager.AppSettings["MailLogEnabled"]);
        private static string _mailLogPath = ConfigurationManager.AppSettings["SFTPWatcherMailLogFile"];
        private static readonly object _mailLogLock = new object();


        private string StorageRoot
        {
            get { return Path.Combine(ConfigurationManager.AppSettings["uploadFolder"]); }
        }

        public SFTPWatcher()
        {
            _portalService =
                new PortalSosService(new PortalSosRepository(new InrappSosDbContext()));
            _timeToWaitForCompleteDelivery = Convert.ToInt32(ConfigurationManager.AppSettings["TimeSpan"]);
            DateTime currentTime = DateTime.Now;
            filesHelper = new FilesHelper(StorageRoot);
            _smtpClient = new SmtpClient(ConfigurationManager.AppSettings["MailServer"]);
            _mailSender = ConfigurationManager.AppSettings["MailSender"];
            _mailLogPath = _mailLogPath.Replace(".txt", "_" + DateTime.Now.ToString("yyyyMMdd") + ".txt");
        }

        public void CheckFiles()
        {
            string _fileareaPath = ConfigurationManager.AppSettings["FileAreaPath"];
            //var relevantFileNameStarts = _portalService.HamtaGodkandaFilnamnsStarter();

            var directoriesInFileArea = Directory.GetDirectories(_fileareaPath);

            //Check if registered ftpaccount before handling files
            foreach (var folder in directoriesInFileArea)
            {
                var folderName = GetFolderNameFromPath(folder);
                //folderName equals sftpAccountName
                var ftpAccount = _portalService.HamtaFtpKontoByName(folderName);

                if (ftpAccount != null)
                {
                    DirectoryInfo dir = new DirectoryInfo(folder);

                    //Get info about registers relevant for current sftpaccount
                    var  delregisterInfoList = _portalService.HamtaRelevantaDelregisterForSFTPKonto(ftpAccount);
                    List<FileInfo> filesInFolder = dir.GetFiles().OrderByDescending(p => p.CreationTime).ToList();

                    if (filesInFolder.Count > 0)
                    {
                       foreach (var delregInfo in delregisterInfoList)
                       {
                           var okFileCodes = new List<string>();
                            if (delregInfo.RapporterarPerEnhet)
                            {
                                foreach (var orgenhet in delregInfo.Orgenheter)
                                {
                                    okFileCodes.Add(orgenhet.Value);
                                }
                            }
                            else
                            {
                                okFileCodes.Add(GetOrgCodeForOrg(ftpAccount.OrganisationsId, delregInfo));
                            }
                            var okFilesForSubDirList = new List<FileInfo>();
                            var period = String.Empty;
                            var unitCode = String.Empty;
                            var okFile = false;
                            //If relevant file exists in folder, save file to list
                            foreach (var filkrav in delregInfo.Filkrav)
                            {
                                foreach (var forvantadfil in filkrav.ForvantadeFiler)
                                {
                                    Regex expression = new Regex(forvantadfil.Regexp, RegexOptions.IgnoreCase);
                                    foreach (var file in filesInFolder)
                                    {
                                        Match match = expression.Match(file.Name);
                                        //If correct filename, check fileCode and period
                                        if (match.Success)
                                        {
                                            var fileCodeInFileName = match.Groups[1].Value;
                                            //TODO - för alla register? Special för PAR?
                                            if (okFileCodes.Contains(fileCodeInFileName))
                                            {
                                                if (delregInfo.RapporterarPerEnhet)
                                                {
                                                    //Get orgunitid
                                                    unitCode = _portalService.HamtaOrganisationsenhetMedFilkod(fileCodeInFileName, ftpAccount.OrganisationsId).Enhetskod;
                                                }
                                                okFile = true;
                                                var periodInFileName = match.Groups[2].Value;
                                                if (!_portalService.HamtaGiltigaPerioderForDelregister(delregInfo.Id).Contains(periodInFileName))
                                                {
                                                    okFile = false;
                                                }
                                                else
                                                {
                                                    period = periodInFileName;
                                                }
                                            }
                                            
                                            if (okFile)
                                                okFilesForSubDirList.Add(file);
                                        }
                                    }
                                }
                                if (okFilesForSubDirList.Count > 0)
                                {
                                    if (CompleteDelivery(filkrav, okFilesForSubDirList))
                                    {
                                        try
                                        {
                                            var resultList = new List<ViewDataUploadFilesResult>();
                                            //If complete delivery of approved files, tag and upload
                                            filesHelper.UploadSFTPFilesAndShowResults(okFilesForSubDirList, resultList, ftpAccount, delregInfo.Id, unitCode, period, delregisterInfoList);

                                            //Delete files from incoming filearea
                                            foreach (var file in okFilesForSubDirList)
                                            {
                                                file.Delete();
                                            }
                                        }
                                        catch (ApplicationException e)
                                        {
                                            ErrorManager.WriteToErrorLog("SFTPWatcher", "Upload approved files", e.ToString(),
                                                e.HResult, folder);
                                            //TODO - email, ta bort ReadLine
                                            Console.WriteLine("Sending email-alert. Upload files aborted.");
                                            Console.ReadLine();
                                            var mailRecipients = new List<string>();
                                            mailRecipients.Add(_mailSender);
                                            string subject = "SFTP-leverans gick ej att ladda upp.";
                                            string body = "Hej! </br>";
                                            body += "Fel inträffade när leveransen skulle sparas. <br>";
                                            body += e.ToString() +  "<br>";
                                            foreach (var file in okFilesForSubDirList)
                                            {
                                                body += file.Name + "<br> ";
                                            }
                                            body += "SFTPkonto: " + folderName + "</br>";

                                            SendEmail(subject, body, mailRecipients);
                                        }
                                        catch (Exception e)
                                        {
                                            //Todo - send mail?
                                            Console.WriteLine("Sending email-alert. Upload files aborted.");
                                            Console.ReadLine();
                                            var mailRecipients = new List<string>();
                                            mailRecipients.Add(_mailSender);
                                            string subject = "SFTP-leverans gick ej att ladda upp.";
                                            string body = "Hej! </br>";
                                            body += "Fel inträffade när leveransen skulle sparas. <br>";
                                            body += e.ToString() + "<br>";
                                            foreach (var file in okFilesForSubDirList)
                                            {
                                                body += file.Name + "<br> ";
                                            }
                                            body += "SFTPkonto: " + folderName + "</br>";

                                            SendEmail(subject, body, mailRecipients);
                                        }
                                    }
                                    else
                                    {
                                        //If not complete, check if enough time elapsed
                                        var sortedFilesList = okFilesForSubDirList.OrderByDescending(p => p.CreationTime).ToList();
                                        //Check youngest file
                                        var tmp = sortedFilesList[0].CreationTime.AddMinutes(_timeToWaitForCompleteDelivery);
                                        var tmp2 = sortedFilesList[0].CreationTime;

                                        if (sortedFilesList[0].CreationTime.AddMinutes(_timeToWaitForCompleteDelivery) <  DateTime.Now)
                                        {
                                            try
                                            {
                                                //TODO - email, ta bort ReadLine
                                                Console.WriteLine("Sending email. Not complete delivery.");
                                                Console.ReadLine();
                                                var mailRecipients = _portalService.HamtaEpostadresserForSFTPKonto(ftpAccount.Id);
                                                string subject = "SFTP-leverans ej komplett";
                                                string body = "Hej! </br>";
                                                body += "Leveransen är ej komplett. <br>";
                                                foreach (var file in sortedFilesList)
                                                {
                                                    body += file.Name + "<br> ";
                                                }
                                                body += "SFTPkonto: " + folderName + "</br>";
                                                body += "Vid frågor kontakta Socialstyrelsen, e-post: inrapportering@socialstyrelsen.se eller telefon 010 222 2222. <br> ";

                                                SendEmail(subject, body, mailRecipients);

                                                var errorFilesArea = ConfigurationManager.AppSettings["notApprovedFilesFolder"];
                                                String pathOnServer = Path.Combine(errorFilesArea);
                                                //Kopiera filerna till fel-mappen 
                                                foreach (var file in sortedFilesList)
                                                {
                                                    var fullPath = Path.Combine(pathOnServer, Path.GetFileName(file.Name));
                                                    file.CopyTo(fullPath);
                                                }
                                                //Ta sen bort filerna från ursprungliga mappen
                                                foreach (var file in sortedFilesList)
                                                {
                                                    file.Delete();
                                                }
                                            }
                                            catch (Exception e)
                                            {
                                                Console.WriteLine(e);
                                                //Send email
                                                var mailRecipients = new List<string>();
                                                mailRecipients.Add(_mailSender);
                                                ErrorManager.WriteToErrorLog("SFTPWatcher", "Moving not approved files aborted", e.ToString(), e.HResult, folderName);

                                                string subject = "Ett fel inträffade när ej godkända filer skulle flyttas.";
                                                string body = "Hej! </br>";
                                                body += "Fel inträffade när ej godkända filer skulle flyttas. <br>";
                                                body += e.ToString() + "<br>";
                                                foreach (var file in okFilesForSubDirList)
                                                {
                                                    body += file.Name + "<br> ";
                                                }
                                                body += "SFTPkonto: " + folderName + "</br>";

                                                SendEmail(subject, body, mailRecipients);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private string GetOrgCodeForOrg(int orgId, RegisterInfo delregInfo)
        {
            var orgCodeList = new List<string>();
            var org = _portalService.HamtaOrganisation(orgId);
            var orgtypeListForOrg = _portalService.HamtaOrgtyperForOrganisation(orgId);
            var orgCode = String.Empty;

            //Compare organisations orgtypes with orgtypes for current subdir
            foreach (var subDirOrgtype in delregInfo.Organisationstyper)
            {
                foreach (var orgOrgtype in orgtypeListForOrg)
                {
                    if (subDirOrgtype.Value == orgOrgtype.Typnamn)
                    {
                        if (orgOrgtype.Typnamn == "Kommun")
                        {
                            orgCode = org.Kommunkod;
                        }
                        else if (orgOrgtype.Typnamn == "Landsting")
                        {
                            orgCode = org.Landstingskod;
                        }
                        else
                        {
                            orgCode = org.Inrapporteringskod;
                        }
                    }
                }
            }
            return orgCode;
        }

        private string GetFolderNameFromPath(string folderPath)
        {
            var lastSlashPos = folderPath.LastIndexOf("\\");
            var folderName = folderPath.Substring(lastSlashPos + 1);
            return folderName;
        }

        private bool CompleteDelivery(RegisterFilkrav filkrav, List<FileInfo> fileList)
        {
            var complete = false;
            var numberOfFiles = filkrav.AntalFiler;
            var numberOfRequiredFiles = filkrav.AntalObligatoriskaFiler;
            var numberOfNotRequiredFiles = filkrav.AntalEjObligatoriskaFiler;

            var numberOfFilesInList = fileList.Count();
            var numberOfRequriredFilesinList = 0;
            var numberOfNotRequiredFilesinList = 0;

            //TODO - check time

            foreach (var file in fileList)
            {
                foreach (var forvantadFil in filkrav.ForvantadeFiler)
                {
                    Regex expression = new Regex(forvantadFil.Regexp, RegexOptions.IgnoreCase);
                    Match match = expression.Match(file.Name);
                    if (match.Success)
                    {
                        if (forvantadFil.Obligatorisk)
                        {
                            numberOfRequriredFilesinList++;
                        }
                        else
                        {
                            numberOfNotRequiredFiles++;
                        }
                    }
                }
            }

            if (numberOfRequiredFiles == numberOfRequriredFilesinList)
            {
                complete = true;
            }

            return complete;
        }

        public void SendEmail(string subject, string bodytext, List<string> mailRecipients)
        {
            try
            {
                string currentDate = DateTime.Now.ToString("yyyyMMdd");

                MailMessage msg = new MailMessage();
                MailAddress fromMail = new MailAddress(_mailSender);
                msg.From = fromMail;

                foreach (var address in mailRecipients)
                {
                    msg.To.Add(address);
                }

                //TODO för test, använd min epostadress
                msg.To.Add("marie.ahlin@socialstyrelsen.se");

                msg.Subject = subject;
                msg.Body = bodytext;
                msg.BodyEncoding = System.Text.Encoding.UTF8;
                msg.IsBodyHtml = true;
                //client.Send(msg);
                _smtpClient.SendAsync(msg, "notification");
                //Wait try to prevent to spam the server if many emails.. did run in to async problem else during testing.
                System.Threading.Thread.Sleep(8000);

                Log("Mail till : " + mailRecipients + ". Rubrik: " + subject);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("SFTPWatcher", "Send Email", e.ToString(),e.HResult, mailRecipients[0]);
            }

            //_smtpClient.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);
        }

        //private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        //{
        //    // Get the unique identifier for this asynchronous operation.
        //    var token = (string)e.UserState;

        //    if (e.Cancelled)
        //    {
        //        Console.WriteLine(e.Cancelled.ToString());
        //        // Logger.Log(Logger.Level.Info," Send canceled. " +  token);
        //    }
        //    if (e.Error != null)
        //    {
        //        Console.WriteLine(e.Error.ToString());
        //        ErrorManager.WriteToErrorLog("SFTPWatcher", "Error sending email.", e.ToString());
        //        //Logger.Log(Logger.Level.Error, " Send failed. " + e.Error.ToString());
        //    }
        //    else
        //    {
        //        Console.WriteLine("Sent mail successfully.");
        //        // Logger.Log(Logger.Level.Info, " Sent notification successfully.");
        //    }
        //}

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
