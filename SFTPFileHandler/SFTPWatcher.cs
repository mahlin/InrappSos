using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
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
        }

        public void CheckFolders()
        {
            string _fileareaPath;

            //Om utvecklingsmiljön
            if (ConfigurationManager.AppSettings["Env"] == "Utv")
            {
                _fileareaPath = ConfigurationManager.AppSettings["UtvFileAreaPath"];
                var directoriesInFileArea = Directory.GetDirectories(_fileareaPath);

                foreach (var folder in directoriesInFileArea)
                {
                    CheckFiles(folder);
                }
            }
            else //KT,AT,Prod
            {
                 _fileareaPath = ConfigurationManager.AppSettings["NetworkPath"];
                var credentials = new NetworkCredential(@ConfigurationManager.AppSettings["NetworkUser"], ConfigurationManager.AppSettings["NetworkPwd"]);
                try
                {
                    using (new NetworkConnection(_fileareaPath, credentials))
                    {
                        var directoriesInFileArea = Directory.GetDirectories(_fileareaPath);
                        foreach (var folder in directoriesInFileArea)
                        {
                            CheckFiles(folder);
                        }

                        //foreach (var dir in dirList)
                        //{
                        //    var lastSlashPos = dir.LastIndexOf("\\");
                        //    var folderName = dir.Substring(lastSlashPos + 1);
                        //    Console.WriteLine(folderName);
                        //}
                    }
                }

                catch (Win32Exception e)
                {
                    ErrorManager.WriteToErrorLog("SFTPWatcher", "NetworkConnection", e.ToString(),
                        e.HResult);
                    Console.WriteLine(e.Message);
                    Console.ReadLine();
                }
                catch (Exception ex)
                {
                    ErrorManager.WriteToErrorLog("SFTPWatcher", "NetworkConnection", ex.ToString(),
                        ex.HResult);
                    Console.WriteLine(ex.Message);
                    Console.ReadLine();
                }
            }
            

            //var directoriesInFileArea = Directory.GetDirectories(_fileareaPath);

            ////Check if registered ftpaccount before handling files
            //foreach (var folder in directoriesInFileArea)
            //{
            //    CheckFiles(folder);
            //}
        }

        private void CheckFiles(string folder)
        {
            var folderName = GetFolderNameFromPath(folder);
            //folderName equals sftpAccountName
            var ftpAccount = _portalService.HamtaFtpKontoByName(folderName);

            //Check if registered ftpaccount before handling files
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
            body += "Registera kontaktperson via https://filip.socialstyrelsen.se och gör sedan om leveransen. <br>";
            body += "Berörda filer: <br>";
            foreach (var file in filesInFolder)
            {
                body += file.Name + "<br> ";
            }

            body += "SFTPkonto: " + folderName + "<br><br>";
            body += "Vid frågor kontakta Socialstyrelsen, e-post: inrapportering@socialstyrelsen.se eller telefon 075-247 45 40 under våra telefontider måndag 13-15, tisdag 9-11, torsdag 13.15. <br> ";

            _mailHelper.SendEmail(subject, body, mailRecipients, _mailSender);
            //SendEmail(subject, body, mailRecipients);
            var errorFilesArea = ConfigurationManager.AppSettings["notApprovedFilesFolder"];
            String pathOnServer = Path.Combine(errorFilesArea);
            var fullPathDir = Path.Combine(pathOnServer, folderName);

            //Kopiera filerna till fel-mappen 
            foreach (var file in filesInFolder)
            {
                //Kopiera filen till det aktuella kontots fel-mapp 
                WriteFileToErrorFolder(fullPathDir, file);
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
            body += "Vid frågor kontakta Socialstyrelsen, e-post: inrapportering@socialstyrelsen.se eller telefon 075-247 45 40 under våra telefontider måndag 13-15, tisdag 9-11, torsdag 13.15. <br> ";

            _mailHelper.SendEmail(subject, body, mailRecipients, _mailSender);

            var errorFilesArea = ConfigurationManager.AppSettings["notApprovedFilesFolder"];
            String pathOnServer = Path.Combine(errorFilesArea);
            var fullPathDir = Path.Combine(pathOnServer, folderName);

            //Kopiera filerna till fel-mappen 
            foreach (var file in sortedFilesList)
            {
                //Kopiera filen till det aktuella kontots fel-mapp 
                WriteFileToErrorFolder(fullPathDir, file);
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
            Console.WriteLine("Sending email. Not correct filename.");
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
            body += "Leveransen innehåller fil med felaktigt filnamn:  <br>";
            foreach (var incorrectFile in incorrectFilesList)
            {
                body += incorrectFile.Name + "<br> ";
            }
            body += "SFTPkonto: " + folderName + "<br><br>";
            body += "Vid frågor kontakta Socialstyrelsen, e-post: inrapportering@socialstyrelsen.se eller telefon 075-247 45 40 under våra telefontider måndag 13-15, tisdag 9-11, torsdag 13.15. <br> ";

            _mailHelper.SendEmail(subject, body, mailRecipients, _mailSender);

            //SendEmail(subject, body, mailRecipients);
            var errorFilesArea = ConfigurationManager.AppSettings["notApprovedFilesFolder"];
            String pathOnServer = Path.Combine(errorFilesArea);
            var fullPathDir = Path.Combine(pathOnServer, folderName);
            foreach (var incorrectFile in incorrectFilesList)
            {
                //Kopiera filen till det aktuella kontots fel-mapp 
                WriteFileToErrorFolder(fullPathDir, incorrectFile);
                //Ta sen bort filen från ursprungliga mappen
                incorrectFile.Delete();
            }
        }


        private void WriteFileToErrorFolder(string fullPathDir, FileInfo incorrectFile)
        {
            Directory.CreateDirectory(fullPathDir);
            //Tag file with date and time so it becomes unique in folder
            var tag = DateTime.Now.ToString("_yyyyMMdd" + "T" + "HHmmss");
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(incorrectFile.Name);
            var extension = Path.GetExtension(incorrectFile.Name);
            var pathToCopyTo = Path.Combine(fullPathDir, Path.GetFileName(fileNameWithoutExtension + tag + extension));
            incorrectFile.CopyTo(pathToCopyTo);
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

    }
}
