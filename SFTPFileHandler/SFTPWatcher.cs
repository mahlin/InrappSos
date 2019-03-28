using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
                        var okFileCodes = _portalService.HamtaGiltigaFilkoderForSFTPKonto(ftpAccount.Id);

                        foreach (var delregInfo in delregisterInfoList)
                        {
                            var okFilesForSubDirList = new List<FileInfo>();
                            var period = String.Empty;
                            var fileCode = String.Empty;
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
                                                fileCode = fileCodeInFileName;
                                                okFile = true;
                                            }
                                            else
                                            {
                                                okFile = false;
                                            }
                                            var periodInFileName = match.Groups[2].Value;
                                            if (_portalService.HamtaGiltigaPerioderForDelregister(delregInfo.Id).Contains(periodInFileName))
                                            {
                                                period = periodInFileName;
                                                okFile = true;
                                            }
                                            else
                                            {
                                                okFile = false;
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
                                            filesHelper.UploadSFTPFilesAndShowResults(okFilesForSubDirList, resultList, ftpAccount, delregInfo.Id, fileCode, period, delregisterInfoList);

                                            //Delete files from incoming filearea
                                            foreach (var file in okFilesForSubDirList)
                                            {
                                                file.Delete();
                                            }
                                        }
                                        catch (ApplicationException e)
                                        {
                                            //Todo - send mail?
                                            ErrorManager.WriteToErrorLog("SFTPWatcher", "Upload approved files", e.ToString(),
                                                e.HResult, folder);
                                        }
                                        catch (Exception e)
                                        {
                                            //Todo - send mail?
                                            ErrorManager.WriteToErrorLog("SFTPWatcher", "Upload approved files", e.ToString(),
                                                e.HResult, folder);
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
                                            //TODO - email
                                            Console.WriteLine("Sending email. Not complete delivery.");
                                            Console.ReadLine();
                                            //TODO - move files to other area
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
            }
        }

        public static List<String> GetAllFiles(String folderPath)
        {
            return Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly).ToList();
        }

        private string GetFilenameFromPath(string filePath)
        {
            var lastSlashPos = filePath.LastIndexOf("\\");
            var fileName = filePath.Substring(lastSlashPos + 1);
            return fileName;
        }

        private string GetFolderNameFromPath(string folderPath)
        {
            var lastSlashPos = folderPath.LastIndexOf("\\");
            var folderName = folderPath.Substring(lastSlashPos + 1);
            return folderName;
        }

        public string GetFilenameStart(string fileName)
        {
            var firstUnderscorePos = fileName.IndexOf("_");
            var fileNameStart = fileName.Substring(0, firstUnderscorePos);
            return fileNameStart;
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






        ////TODO - dummyobject i väntan på databastabell
        //public class FtpAccoutnt
        //{
        //    public int Id { get; set; }
        //    public int OrganisationsId { get; set; }
        //    public string Name { get; set; }

        //}


    }
}
