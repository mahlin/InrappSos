using System;
using System.Collections.Generic;
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
        FilesHelper _filesHelper;
        MailHelper _mailHelper;
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
            _filesHelper = new FilesHelper(StorageRoot);
            _mailHelper = new MailHelper();
            _smtpClient = new SmtpClient(ConfigurationManager.AppSettings["MailServer"]);
            _mailSender = ConfigurationManager.AppSettings["MailSender"];
            _mailLogPath = _mailLogPath.Replace(".txt", "_" + DateTime.Now.ToString("yyyyMMdd") + ".txt");
        }

        public void CheckFolders()
        {
            string _fileareaPath = ConfigurationManager.AppSettings["FileAreaPath"];

            //System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(@"\\\\DESKTOP-Q22BLMK\TestSFTP\some-file-that-exists.txt");
            //int count = dir.GetFiles().Length;
            //var _remoteName = @"\\DESKTOP-Q22BLMK\";
            //FileInfo myFile = new FileInfo(@"\\DESKTOP-Q22BLMK\some-file-that-exists.txt");
            //bool exists = myFile.Exists;
            //FileInfo myFile1 = new FileInfo(@"\\DESKTOP-Q22BLMK\C\TestSFTP\some-file-that-exists.txt");
            //bool exists1 = myFile1.Exists;
            //FileInfo myFile2 = new FileInfo((@"\\DESKTOP-Q22BLMK\C\some-file-that-exists.txt");
            //bool exists2 = myFile2.Exists;

            var directoriesInFileArea = Directory.GetDirectories(_fileareaPath);

            //Check if registered ftpaccount before handling files
            foreach (var folder in directoriesInFileArea)
            {
                CheckFiles(folder);
            }
        }

        private void CheckFiles(string folder)
        {
            var folderName = GetFolderNameFromPath(folder);
            //folderName equals sftpAccountName
            var ftpAccount = _portalService.HamtaFtpKontoByName(folderName);

            if (ftpAccount != null)
            {

                DirectoryInfo dir = new DirectoryInfo(folder);

                //Get info about registers relevant for current sftpaccount
                var delregisterInfoList = _portalService.HamtaRelevantaDelregisterForSFTPKonto(ftpAccount);
                List<FileInfo> filesInFolder = dir.GetFiles().OrderByDescending(p => p.CreationTime).ToList();

                if (filesInFolder.Count > 0)
                {
                    //Check if account has any registered contactperson. Otherwise reject files and write to errorlog
                    var userEmails = _portalService.HamtaEpostadresserForSFTPKonto(ftpAccount.Id);
                    if (userEmails.Any())
                    {
                        var inCorrectFilenamnList = new List<FileInfo>();
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
                                        //om fil redan godkänd behöver den inte mappas
                                        IEnumerable<FileInfo> res = from fileInList in okFilesForSubDirList
                                            where fileInList.Name == file.Name
                                            select file;
                                        if (!res.Any())
                                        {
                                            Match match = expression.Match(file.Name);
                                            //If correct filename, check fileCode and period
                                            if (match.Success)
                                            {
                                                //Remove from errorlist if saved there
                                                inCorrectFilenamnList.Remove(file);
                                                var fileCodeInFileName = match.Groups[1].Value;
                                                //TODO - för alla register? Special för PAR?
                                                if (okFileCodes.Contains(fileCodeInFileName))
                                                {
                                                    if (delregInfo.RapporterarPerEnhet)
                                                    {
                                                        //Get orgunitid
                                                        unitCode = _portalService
                                                            .HamtaOrganisationsenhetMedFilkod(fileCodeInFileName,
                                                                ftpAccount.OrganisationsId).Enhetskod;
                                                    }
                                                    okFile = true;
                                                    var periodInFileName = match.Groups[2].Value;
                                                    if (!_portalService.HamtaGiltigaPerioderForDelregister(delregInfo.Id)
                                                        .Contains(periodInFileName))
                                                    {
                                                        okFile = false;
                                                    }
                                                    else
                                                    {
                                                        period = periodInFileName;
                                                    }
                                                }
                                                if (okFile)
                                                {
                                                    okFilesForSubDirList.Add(file);
                                                }
                                            }
                                            else
                                            {
                                                //save to errorlist if file not already saved there
                                                IEnumerable<FileInfo> result = from fileInList in inCorrectFilenamnList
                                                    where fileInList.Name == file.Name
                                                    select file;
                                                if (!result.Any())
                                                {
                                                    inCorrectFilenamnList.Add(file);
                                                }
                                            }
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
                                            _filesHelper.UploadSFTPFilesAndShowResults(okFilesForSubDirList, resultList, ftpAccount, delregInfo.Id, unitCode, period, delregisterInfoList);

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
                                        }
                                        catch (Exception e)
                                        {
                                            //Todo - send mail?
                                            Console.WriteLine("Sending email-alert. Upload files aborted.");
                                            ErrorManager.WriteToErrorLog("SFTPWatcher", "Upload approved files", e.ToString(),
                                                e.HResult, folder);
                                        }
                                    }
                                    else
                                    {
                                        //If not complete, check if enough time elapsed
                                        var sortedFilesList = okFilesForSubDirList.OrderByDescending(p => p.CreationTime).ToList();
                                        //Check youngest file
                                        if (sortedFilesList[0].CreationTime.AddMinutes(_timeToWaitForCompleteDelivery) < DateTime.Now)
                                        {
                                            //Not complete delivery - email user and remove files
                                            try
                                            {
                                                IncompleteDeliveryHandler(ftpAccount, sortedFilesList, folderName);
                                            }
                                            catch (System.Net.Mail.SmtpException e)
                                            {
                                                Console.WriteLine(e);
                                                //throw new ArgumentException(e.Message);
                                                ErrorManager.WriteToErrorLog("SFTPWatcher", "SendEmail/Incomplete delivery", e.ToString(), e.HResult, folderName);
                                            }
                                            catch (Exception e)
                                            {
                                                Console.WriteLine(e);
                                                ErrorManager.WriteToErrorLog("SFTPWatcher", "Moving not approved files aborted", e.ToString(), e.HResult, folderName);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        //If any files incorrect - email user and move file
                        if (inCorrectFilenamnList.Any())
                        {
                            try
                            {
                                IncorrectFilesHandler(ftpAccount, inCorrectFilenamnList, folderName);
                            }
                            catch (System.Net.Mail.SmtpException e)
                            {
                                Console.WriteLine(e);
                                //throw new ArgumentException(e.Message);
                                ErrorManager.WriteToErrorLog("SFTPWatcher", "SendEmail/Not correct filenamne", e.ToString(), e.HResult, folderName);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                                ErrorManager.WriteToErrorLog("SFTPWatcher", "Not correct filenamne", e.ToString(), e.HResult, folderName);
                            }
                        }

                    }
                    else
                    {
                        //No contactperson registered for account
                        NoRegisteredContactHandler(ftpAccount, folderName, folder);
                    }
                }
            }
        }

        private void NoRegisteredContactHandler(SFTPkonto ftpAccount, string folderName, string folder)
        {
            Console.WriteLine("Sending email. No registered contact.");
            var mailRecipients = new List<string>();
            mailRecipients.Add(_portalService.HamtaOrganisation(ftpAccount.OrganisationsId).EpostAdress);
            DirectoryInfo dir = new DirectoryInfo(folder);
            List<FileInfo> filesInFolder = dir.GetFiles().OrderByDescending(p => p.CreationTime).ToList();

            string subject = "SFTP-kontot saknar registrerad kontaktperson";
            string body = "Hej! <br>";
            body += "Vi har mottagit en filleverans via SFTP för ett konto som saknar registrerad kontaktperson och kan då ej hantera filerna. <br>";
            body += "Registera kontaktperson via www.filip.se och gör sedan om leveransen. <br>";
            body += "Berörda filer: <br>";
            foreach (var file in filesInFolder)
            {
                body += file.Name + "<br> ";
            }

            body += "SFTPkonto: " + folderName + "<br><br>";
            body += "Vid frågor kontakta Socialstyrelsen, e-post: inrapportering@socialstyrelsen.se eller telefon 010 222 2222. <br> ";

            _mailHelper.SendEmail(subject, body, mailRecipients, _mailSender);
            //SendEmail(subject, body, mailRecipients);
            var errorFilesArea = ConfigurationManager.AppSettings["notApprovedFilesFolder"];
            String pathOnServer = Path.Combine(errorFilesArea);
            //Kopiera filerna till fel-mappen 
            foreach (var file in filesInFolder)
            {
                var fullPath = Path.Combine(pathOnServer, Path.GetFileName(file.Name));
                file.CopyTo(fullPath);
            }
            //Ta sen bort filerna från ursprungliga mappen
            foreach (var file in filesInFolder)
            {
                file.Delete();
            }
        }


        private void IncompleteDeliveryHandler(SFTPkonto ftpAccount, List<FileInfo> sortedFilesList, string folderName)
        {
            Console.WriteLine("Sending email. Not complete delivery.");
            var mailRecipients = new List<string>();
            var userEmails = _portalService.HamtaEpostadresserForSFTPKonto(ftpAccount.Id);
            //Om inga epostadresser finns kopplade till kontot, använd organisationens epostadress
            if (userEmails.Any())
            {
                mailRecipients = userEmails;
            }
            else
            {
                mailRecipients.Add(_portalService.HamtaOrganisation(ftpAccount.OrganisationsId).EpostAdress);
            }
            string subject = "SFTP-leverans ej komplett";
            string body = "Hej! <br>";
            body += "Leveransen är ej komplett. <br>";
            foreach (var file in sortedFilesList)
            {
                body += file.Name + "<br> ";
            }
            body += "SFTPkonto: " + folderName + "<br><br>";
            body += "Vid frågor kontakta Socialstyrelsen, e-post: inrapportering@socialstyrelsen.se eller telefon 010 222 2222. <br> ";

            _mailHelper.SendEmail(subject, body, mailRecipients, _mailSender);
            //SendEmail(subject, body, mailRecipients);

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

        private void IncorrectFilesHandler(SFTPkonto ftpAccount, List<FileInfo> incorrectFilesList, string folderName)
        {
            //Incorrect filename - move file and email user
            Console.WriteLine("Sending email. Not correct filenamne.");
            var mailRecipients = new List<string>();
            var userEmails = _portalService.HamtaEpostadresserForSFTPKonto(ftpAccount.Id);
            //Om inga epostadresser finns kopplade till kontot, använd organisationens epostadress
            if (userEmails.Any())
            {
                mailRecipients = userEmails;
            }
            else
            {
                mailRecipients.Add(_portalService.HamtaOrganisation(ftpAccount.OrganisationsId).EpostAdress);
            }
            string subject = "SFTP-leverans - fil med felaktigt filnamn";
            string body = "Hej! <br>";
            body += "Leveransen innehåller fil med felatigt filnamn:  <br>";
            foreach (var incorrectFile in incorrectFilesList)
            {
                body += incorrectFile.Name + "<br> ";
            }
            body += "SFTPkonto: " + folderName + "<br><br>";
            body += "Vid frågor kontakta Socialstyrelsen, e-post: inrapportering@socialstyrelsen.se eller telefon 010 222 2222. <br> ";

            _mailHelper.SendEmail(subject, body, mailRecipients, _mailSender);

            //SendEmail(subject, body, mailRecipients);
            var errorFilesArea = ConfigurationManager.AppSettings["notApprovedFilesFolder"];
            String pathOnServer = Path.Combine(errorFilesArea);
            //Kopiera filerna till det aktuella kontots fel-mapp 
            var fullPathDir = Path.Combine(pathOnServer, folderName);
            Directory.CreateDirectory(fullPathDir);
            foreach (var incorrectFile in incorrectFilesList)
            {
                var pathToCopyTo = Path.Combine(fullPathDir, Path.GetFileName(incorrectFile.Name));
                incorrectFile.CopyTo(pathToCopyTo);
                //Ta sen bort filen från ursprungliga mappen
                incorrectFile.Delete();
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

        //private void SendEmail(string subject, string bodytext, List<string> mailRecipients)
        //{

        //    string currentDate = DateTime.Now.ToString("yyyyMMdd");
        //    var recipientsStr = mailRecipients.Aggregate((a, b) => a + ", " + b);

        //    MailMessage msg = new MailMessage();
        //    MailAddress fromMail = new MailAddress(_mailSender);
        //    msg.From = fromMail;

        //    foreach (var address in mailRecipients)
        //    {
        //        msg.To.Add(address);
        //    }

        //    //TODO för test, använd min epostadress
        //    //msg.To.Add("marie.ahlin@socialstyrelsen.se");

        //    msg.Subject = subject;
        //    msg.Body = bodytext;
        //    msg.BodyEncoding = System.Text.Encoding.UTF8;
        //    msg.IsBodyHtml = true;

        //    _smtpClient.Send(msg);

        //    // _smtpClient.SendAsync(msg, "notification");
        //    //Wait try to prevent to spam the server if many emails.. did run in to async problem else during testing.
        //    //System.Threading.Thread.Sleep(8000);

        //    //Logga att mailet skickats
        //    Log("Mail till : " + recipientsStr + ". Rubrik: " + subject);

        //    //_smtpClient.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);
        //}

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
        //private static void Log(string message)
        //{

        //    if (_mailLogEnabled)
        //    {
        //        lock (_mailLogLock)
        //        {
        //            try
        //            {
        //                File.AppendAllText(_mailLogPath, DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss]") + " " + message + Environment.NewLine);
        //            }
        //            catch (Exception ex)
        //            {
        //                Console.WriteLine("Couldn't log '" + message + "' to file log '" + _mailLogPath + "'" + Environment.NewLine + ex.ToString());
        //            }
        //        }
        //    }
        //}

    }
}
