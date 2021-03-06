﻿using InrappSos.DataAccess;
using InrappSos.ApplicationService.DTOModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Numerics;
using InrappSos.ApplicationService.Interface;
using InrappSos.DomainModel;
using System.Text.RegularExpressions;
using System.Net;

namespace InrappSos.ApplicationService.Helpers
{
    public class FilesHelper
    {
        private readonly IPortalSosRepository _portalSosRepository;
        private readonly IPortalSosService _portalSosService;
        private readonly InrappSosDbContext db = new InrappSosDbContext();
        MailHelper _mailHelper;
        String DeleteURL = null;
        String DeleteType = null;
        String StorageRoot = null;
        String UrlBase = null;
        String tempPath = null;
        //ex:"~/Files/something/";
        String serverMapPath = null;

        public FilesHelper()
        {
            _portalSosRepository = new PortalSosRepository(db);
            _portalSosService = new PortalSosService(_portalSosRepository);
            _mailHelper = new MailHelper();
        }

        public FilesHelper(String DeleteURL, String DeleteType, String StorageRoot, String UrlBase, String tempPath, String serverMapPath)
        {
            this.DeleteURL = DeleteURL;
            this.DeleteType = DeleteType;
            this.StorageRoot = StorageRoot;
            this.UrlBase = UrlBase;
            this.tempPath = tempPath;
            this.serverMapPath = serverMapPath;
            _portalSosRepository = new PortalSosRepository(db);
            _portalSosService = new PortalSosService(_portalSosRepository);
            _mailHelper = new MailHelper();
        }

        public FilesHelper(String StorageRoot)
        {
            this.StorageRoot = StorageRoot;
            _portalSosRepository = new PortalSosRepository(db);
            _portalSosService = new PortalSosService(_portalSosRepository);
            _mailHelper = new MailHelper();
        }

        public void UploadAndShowResults(HttpContextBase ContentBase, List<ViewDataUploadFilesResult> resultList, string userId, string userName, string orgKod, int selectedRegisterId, string selectedUnitId, string selectedPeriod, List<RegisterInfo> registerList)
        {
            var httpRequest = ContentBase.Request;
            //System.Diagnostics.Debug.WriteLine(Directory.Exists(tempPath));

            //Kolla vilket register filen/filerna hör till och skapa mapp om det behövs
            var slussmapp = registerList.Where(x => x.Id == selectedRegisterId).Select(x => x.Slussmapp).Single();

            var fileName = httpRequest.Files[0].FileName;
            var periodInFileName = String.Empty;

            //Period tas från filnamnet pga problem med selectedPeriod från klienten 4 april 2018
            //Använd regular expression istf hårdkodning - 20180917
            var filkravList = _portalSosRepository.GetFileRequirementsAndExpectedFilesForSubDirectory(selectedRegisterId);

            foreach (var filkrav in filkravList)
            {
                //hämta regexper
                var forvantadFilList = filkrav.AdmForvantadfil;
                //kontrollera om inkommande fil matchar regexp
                foreach (var forvFil in forvantadFilList)
                {
                    Regex expression = new Regex(forvFil.Regexp, RegexOptions.IgnoreCase);
                    Match match = expression.Match(fileName);
                    if (match.Success)
                    {
                        periodInFileName = match.Groups[2].Value;
                        //periodInFileName = match.Groups["period"].Value;
                    }
                }
            }


            //var period = GetPeriodFromFilename(httpRequest.Files[0]);
            if (periodInFileName == "")
            {
                throw new Exception("Felaktig period i filnamnet, " + httpRequest.Files[0].FileName);
            }
            else if (!_portalSosService.HamtaGiltigaPerioderForDelregister(selectedRegisterId).Contains(periodInFileName)) //Kontrollera om vald period är ok 
            {
                throw new Exception("Period i filnamnet inte inom godkänt intervall. " + httpRequest.Files[0].FileName);
            }

            //Hämta forvantadlevid beroende på vald period
            var forvantadLevId = _portalSosRepository.GetExpextedDeliveryIdForSubDirAndPeriod(selectedRegisterId, periodInFileName);
            //var forvantadLevId = registerList.Where(x => x.Id == selectedRegisterId).Select(x => x.ForvantadLevransId).Single();
            StorageRoot = StorageRoot + slussmapp + "\\";
            String fullPath = Path.Combine(StorageRoot);
            Directory.CreateDirectory(fullPath);

            //hämta ett leveransId och skapa hashAddOn till filnamnet
            var orgId = _portalSosRepository.GetUserOrganisationId(userId);
            var orgenhetsId = 0;
            //Om leverans för stadsdelar, hämta organisationsenhetsid
            if (!String.IsNullOrWhiteSpace(selectedUnitId))
            {
                orgenhetsId = _portalSosRepository.GetOrganisationsenhetsId(selectedUnitId, orgId);
            }

            var levId = _portalSosRepository.GetNewLeveransId(userId, userName, orgId, selectedRegisterId, orgenhetsId, forvantadLevId, "Levererad",0);
            var hash = GetHashAddOn(orgKod, levId);
            var headers = httpRequest.Headers;

            if (string.IsNullOrEmpty(headers["X-File-Name"]))
            {
                var registerId = _portalSosRepository.GetSubDirectoryById(selectedRegisterId).RegisterId;
                var register = _portalSosRepository.GetDirectoryById(registerId);
                UploadWholeFile(ContentBase, resultList, hash, levId, selectedUnitId, register.Kortnamn);
            }

            //Om inga filer kunde sparas, rensa levid
            if (!resultList.Any())
            {
                _portalSosRepository.DeleteDelivery(levId);
            }
            //Om PAR-filer, skapa statusfil och ladda upp
            else
            {
                var registerId = _portalSosRepository.GetSubDirectoryById(selectedRegisterId).RegisterId;
                var register = _portalSosRepository.GetDirectoryById(registerId);
                if (register.Kortnamn == "PAR")
                {
                    //Skapa statusfil
                    CreateAndUploadPARStatusFile(levId, resultList);
                }

            }

            //TODO - Test EncryptDecrypt
            //var krypteradUtfil = "C:\\Socialstyrelsen\\KrypteringTest\\krypteradUtfil.txt";
            //var dekrypteradUtfil = "C:\\Socialstyrelsen\\KrypteringTest\\dekrypteradUtfil.txt";
            //var storKrypteradUtfil = "C:\\Socialstyrelsen\\KrypteringTest\\storKrypteradUtfil.txt";
            //var storDekrypteradUtfil = "C:\\Socialstyrelsen\\KrypteringTest\\storDekrypteradUtfil.txt";

            //EncryptDecrypt.AES_Encrypt("C:\\Socialstyrelsen\\KrypteringTest\\testfil.txt", krypteradUtfil);
            //EncryptDecrypt.AES_Decrypt("C:\\Socialstyrelsen\\KrypteringTest\\krypteradUtfil.txt", dekrypteradUtfil);
            //EncryptDecrypt.AES_Encrypt("C:\\Socialstyrelsen\\KrypteringTest\\Ekb_0330_201707_20170815T1011.txt", storKrypteradUtfil);            //EncryptDecrypt.AES_Decrypt("C:\\Socialstyrelsen\\KrypteringTest\\storKrypteradUtfil.txt", storDekrypteradUtfil);

        }

        public void UploadFileDropFilesAndShowResults(HttpContextBase ContentBase, List<ViewDataUploadFilesResult> resultList, int selectedCaseId, string userId, string userName)
        {
            var httpRequest = ContentBase.Request;

            //Kolla vilken ärendetyp filen/filerna hör till och skapa mapp om det behövs
            var arende = _portalSosRepository.GetCase(selectedCaseId);
            var slussmapp = _portalSosRepository.GetCaseType(arende.ArendetypId).Slussmapp;

            //Byt ut ev slashar(/\) i ärendenumret mot bindestreck (-)
            var fixedCasenumber = arende.Arendenr.Replace('/', '_').Replace('\\', '_').Replace('.', '_').Replace('?', '_').Replace('*', '_').Replace('"', '_');

            StorageRoot = StorageRoot + slussmapp + "\\";
            //StorageRoot = StorageRoot + slussmapp + "\\" + fixedCasenumber + "\\";
            String fullPath = Path.Combine(StorageRoot);
            Directory.CreateDirectory(fullPath);

            var headers = httpRequest.Headers;

            //Kontrollera om chunked upload
            if (string.IsNullOrEmpty(headers["X-File-Name"]))
            {
                UploadWholeFiledropFile(ContentBase, resultList, userId, userName, selectedCaseId, fixedCasenumber);
            }
            else
            {
                UploadPartialFile(headers["X-File-Name"], ContentBase, resultList); //Ej implementerat/testa
            }
            NotifyCaseManager(resultList, arende, userName);
            NotifyFileUploader(resultList, arende, userName);

        }


        public void UploadTemplateFileAndShowResults(HttpContextBase ContentBase, List<ViewDataUploadFilesResult> resultList, string userId, string userName)
        {
            //TODO - test
            var tmp = ContentBase.Request.Url.Host;

            var fileareaPath = ConfigurationManager.AppSettings["TemplatesNetworkPath"];

            //Om utvecklingsmiljön
            if (ConfigurationManager.AppSettings["Env"] == "Utv")
            {
                var httpRequest = ContentBase.Request;

                //Spara filerna
                for (int i = 0; i < httpRequest.Files.Count; i++)
                {
                    var file = httpRequest.Files[i];
                    Directory.CreateDirectory(fileareaPath);
                    var fullPath = Path.Combine(fileareaPath, Path.GetFileName(file.FileName));
                    file.SaveAs(fullPath);
                }
            }
            else  //KT,AT,Prod
            {
                var credentials = new NetworkCredential(@ConfigurationManager.AppSettings["NetworkUser"], ConfigurationManager.AppSettings["NetworkPwd"]);
                try
                {
                    using (new NetworkConnection(fileareaPath, credentials))
                    {
                        var httpRequest = ContentBase.Request;

                        //Spara filerna
                        for (int i = 0; i < httpRequest.Files.Count; i++)
                        {
                            var file = httpRequest.Files[i];
                            Directory.CreateDirectory(fileareaPath);
                            var fullPath = Path.Combine(fileareaPath, Path.GetFileName(file.FileName));
                            file.SaveAs(fullPath);
                        }
                    }
                }
                catch (Win32Exception e)
                {
                    ErrorManager.WriteToErrorLog("FilesHelper", "UploadTemplateFileAndShowResults", e.ToString(),
                        e.HResult);
                    Console.WriteLine(e.Message);
                    Console.ReadLine();
                }
                catch (Exception ex)
                {
                    ErrorManager.WriteToErrorLog("FilesHelper", "UploadTemplateFileAndShowResults", ex.ToString(),
                        ex.HResult);
                    Console.WriteLine(ex.Message);
                    Console.ReadLine();
                }
            }
           
        }

        public void UploadSFTPFilesAndShowResults(List<FileInfo> fileList, List<ViewDataUploadFilesResult> resultList, SFTPkonto ftpAccount, int selectedRegisterId, string selectedUnitId, string selectedPeriod, List<RegisterInfo> registerList)
        {

            //Kolla vilket register filen/filerna hör till och skapa mapp om det behövs
            var slussmapp = registerList.Where(x => x.Id == selectedRegisterId).Select(x => x.Slussmapp).Single();
            var user = _portalSosRepository.GetFirstUserForSFTPAccount(ftpAccount.Id);
            //Get orgcode
            var orgCode = String.Empty;
            var orgtypeListForOrg = _portalSosService.HamtaOrgtyperForOrganisation(ftpAccount.OrganisationsId);
            var orgtypeListForSubDir = _portalSosService.HamtaOrgtyperForDelregister(selectedRegisterId);

            //Compare organisations orgtypes with orgtypes for current subdir
            foreach (var subDirOrgtype in orgtypeListForSubDir)
            {
                foreach (var orgOrgtype in orgtypeListForOrg)
                {
                    if (subDirOrgtype.Typnamn == orgOrgtype.Typnamn)
                    {
                        if (orgOrgtype.Typnamn == "Kommun")
                        {
                            orgCode = _portalSosRepository.GetKommunkodForOrganisation(ftpAccount.OrganisationsId); 
                        }
                        else if (orgOrgtype.Typnamn == "Landsting")
                        {
                            orgCode = _portalSosRepository.GetLandstingskodForOrganisation(ftpAccount.OrganisationsId);
                        }
                        else
                        {
                            orgCode = _portalSosRepository.GetInrapporteringskodForOrganisation(ftpAccount.OrganisationsId);
                        }
                    }
                }
            }

            //var fileName = fileList[0].Name;
            //var filkravList = _portalSosRepository.GetFileRequirementsAndExpectedFilesForSubDirectory(selectedRegisterId);

            //Hämta forvantadlevid beroende på vald period
            var forvantadLevId = _portalSosRepository.GetExpextedDeliveryIdForSubDirAndPeriod(selectedRegisterId, selectedPeriod);
            //var forvantadLevId = registerList.Where(x => x.Id == selectedRegisterId).Select(x => x.ForvantadLevransId).Single();
            StorageRoot = StorageRoot + slussmapp + "\\";
            String fullPath = Path.Combine(StorageRoot);
            Directory.CreateDirectory(fullPath);

            var orgenhetsId = 0;
            //Om leverans för stadsdelar/organisationsenheter, hämta organisationsenhetsid
            if (!String.IsNullOrWhiteSpace(selectedUnitId))
            {
                orgenhetsId = _portalSosRepository.GetOrganisationsenhetsId(selectedUnitId, ftpAccount.OrganisationsId);
            }

            var levId = _portalSosRepository.GetNewLeveransId(user.Id, user.Namn, ftpAccount.OrganisationsId, selectedRegisterId, orgenhetsId, forvantadLevId, "Levererad", ftpAccount.Id);
            var hash = GetHashAddOn(orgCode, levId);

            var regId = _portalSosRepository.GetSubDirectoryById(selectedRegisterId).RegisterId;
            var reg = _portalSosRepository.GetDirectoryById(regId);

            UploadWholeSFTPFile(fileList, resultList, hash, levId, selectedUnitId, reg.Kortnamn);

            //Om inga filer kunde sparas, rensa levid
            if (!resultList.Any())
            {
                _portalSosRepository.DeleteDelivery(levId);
            }
            else
            {
                //Om PAR-filer, skapa statusfil och ladda upp
                var registerId = _portalSosRepository.GetSubDirectoryById(selectedRegisterId).RegisterId;
                var register = _portalSosRepository.GetDirectoryById(registerId);
                if (register.Kortnamn == "PAR")
                {
                    //Skapa statusfil
                    CreateAndUploadPARStatusFile(levId, resultList);
                }
                //Save copied files to database filelog
                foreach (var result in resultList)
                {
                    foreach (var itemFile in fileList)
                    {
                        if (result.name == itemFile.Name)
                        {
                            try
                            {
                                _portalSosRepository.SaveToFilelogg(ftpAccount.Kontonamn, itemFile.Name, result.sosName, result.leveransId, result.sequenceNumber);
                            }
                            catch (Exception e)
                            {
                                throw new ApplicationException("Kunde ej spara SFTPfil-info till databas. Konto: " + ftpAccount.Kontonamn + ", Filnamn: " + itemFile.Name + ", LeveransId: " + result.leveransId + ", sekvensnummer:" + result.sequenceNumber);
                            }
                        }
                    }
                }
            }
        }

        private void UploadWholeSFTPFile(List<FileInfo> fileList, List<ViewDataUploadFilesResult> statuses, string hash, int levId, string selectedUnitId, string dirShortName)
        {
            var extendedFileName = String.Empty;

            for (int i = 0; i < fileList.Count; i++)
            {
                var file = fileList[i];

                //TODO - check filename depending on chosen registertype
                var okFileType = false;

                var _acceptedFileTypes = new List<string>();
                var acceptedFiletypesStr = ConfigurationManager.AppSettings["AcceptedFileTypes"];
                string[] acceptedFiletypes = acceptedFiletypesStr.Split(',');
                foreach (var acceptedFiletype in acceptedFiletypes)
                {
                    _acceptedFileTypes.Add(acceptedFiletype);
                }

                foreach (var acceptedFileType in _acceptedFileTypes)
                {
                    if (Path.GetExtension(file.Name).ToLower() == "." + acceptedFileType)
                    {
                        okFileType = true;
                    }
                }
                if (okFileType)
                {
                    String pathOnServer = Path.Combine(StorageRoot);
                    var filOfFilesAddOn = "!" + (i + 1).ToString() + "!" + (fileList.Count).ToString();
                    var timestamp = DateTime.Now.ToString("yyyyMMdd" + "T" + "HHmmss");

                    if (dirShortName == "PAR") //TODO - anpassa PAR till övriga? Taggas nu före filnamnet
                    {
                        extendedFileName = hash + filOfFilesAddOn + "!" + timestamp + "!" + selectedUnitId + "#" + file.Name;
                    }
                    else
                    {
                        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.Name);
                        var extension = Path.GetExtension(file.Name);
                        extendedFileName = fileNameWithoutExtension + hash + filOfFilesAddOn + "!" + timestamp + "!" + selectedUnitId + extension;

                    }
                    var fullPath = Path.Combine(pathOnServer, Path.GetFileName(extendedFileName));
                    file.CopyTo(fullPath);
                    //TODO - Fix this? To handle large files - If file.length larger than an int can hold (2147483647), set it to dummy-value (9999)
                    //Stop using file.length in statuses? Only presented in Filip upload-page, so for SFTP-files it will nerver show?
                    var filelength = 0;
                    if (file.Length > 2147483647)
                    {
                        filelength = 9999;
                    }
                    else
                    {
                        filelength = Convert.ToInt32(file.Length);
                    }
                    statuses.Add(UploadResult(file.Name, filelength, file.Name, (extendedFileName), levId, i + 1, timestamp));
                }
            }
        }
        

        private void UploadWholeFile(HttpContextBase requestContext, List<ViewDataUploadFilesResult> statuses, string hash, int levId, string selectedUnitId, string dirShortName)
        {
            var extendedFileName = String.Empty;
            var request = requestContext.Request;

            var _acceptedFileTypes = new List<string>();
            var acceptedFiletypesStr = ConfigurationManager.AppSettings["AcceptedFileTypes"];
            string[] acceptedFiletypes = acceptedFiletypesStr.Split(',');
            foreach (var acceptedFiletype in acceptedFiletypes)
            {
                _acceptedFileTypes.Add(acceptedFiletype);
            }

            for (int i = 0; i < request.Files.Count; i++)
            {
                var file = request.Files[i];

                //TODO - check filename depending on chosen registertype
                var okFileType = false;
                foreach (var acceptedFileType in _acceptedFileTypes)
                {
                    if (Path.GetExtension(file.FileName).ToLower() == "." + acceptedFileType)
                    {
                        okFileType = true;
                    }
                }
                if (okFileType)
                {
                    String pathOnServer = Path.Combine(StorageRoot);
                    var filOfFilesAddOn = "!" + (i + 1).ToString() + "!" + (request.Files.Count).ToString();
                    var timestamp = DateTime.Now.ToString("yyyyMMdd" + "T" + "HHmmss");
                    if (dirShortName == "PAR") //TODO - anpassa PAR till övriga? Taggas nu före filnamnet 
                    {
                        extendedFileName = hash + filOfFilesAddOn + "!" + timestamp + "!" + selectedUnitId + "#" + file.FileName;

                    }
                    else
                    {
                        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
                        var extension = Path.GetExtension(file.FileName);
                        //var extendedFileName = hash + filOfFilesAddOn + "!" + timestamp + "!" + selectedUnitId + "#" + file.FileName;
                        extendedFileName = fileNameWithoutExtension + hash + filOfFilesAddOn + "!" + timestamp + "!" + selectedUnitId + extension;

                    }
                    var fullPath = Path.Combine(pathOnServer, Path.GetFileName(extendedFileName));
                    file.SaveAs(fullPath);
                    var filelength = 0;

                    if (file.ContentLength > 2147483647)
                    {
                        filelength = 9999;
                    }
                    else
                    {
                        filelength = Convert.ToInt32(file.ContentLength);
                    }
                    statuses.Add(UploadResult(file.FileName, filelength, file.FileName, (extendedFileName), levId, i + 1, timestamp));
                }
            }
        }

        private void UploadWholeFiledropFile(HttpContextBase requestContext, List<ViewDataUploadFilesResult> statuses, string userId,string  userName, int selectedCaseId, string caseNumber)
        {
            var extendedFileName = String.Empty;
            var request = requestContext.Request;
            for (int i = 0; i < request.Files.Count; i++)
            {
                var file = request.Files[i];
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd" + "T" + "HHmmss");
                var timestampForMail = DateTime.Now.ToString("yyyy-MM-dd" + " " + "HH:mm:ss");
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
                var extension = Path.GetExtension(file.FileName);
                extendedFileName = caseNumber + "_" + timestamp + "_" + fileNameWithoutExtension + extension;


                //Save to database
                var filedropId = _portalSosRepository.SaveFiledropFile(file.FileName, extendedFileName, file.ContentLength, selectedCaseId, userId, userName);

                //TODO - check filename depending on chosen registertype
                var okFileType = false;

                var _acceptedFileTypes = new List<string>();
                var acceptedFiletypesStr = ConfigurationManager.AppSettings["AcceptedFileTypes"];
                string[] acceptedFiletypes = acceptedFiletypesStr.Split(',');
                foreach (var acceptedFiletype in acceptedFiletypes)
                {
                    _acceptedFileTypes.Add(acceptedFiletype);
                }

                foreach (var acceptedFileType in _acceptedFileTypes)
                {
                    if (Path.GetExtension(file.FileName).ToLower() == "." + acceptedFileType)
                    {
                        okFileType = true;
                    }
                }
                if (okFileType)
                {
                    String pathOnServer = Path.Combine(StorageRoot);

                    var fullPath = Path.Combine(pathOnServer, Path.GetFileName(extendedFileName));
                    file.SaveAs(fullPath);
                    //Stop using file.length in statuses? Only presented in Filip upload-page, so for SFTP-files it will nerver show?
                    var filelength = 0;
                    if (file.ContentLength > 2147483647)
                    {
                        filelength = 9999;
                    }
                    else
                    {
                        filelength = Convert.ToInt32(file.ContentLength);
                    }
                    statuses.Add(UploadResult(file.FileName, filelength, file.FileName, extendedFileName, filedropId, i + 1, timestampForMail));
                }
            }
        }


        private void UploadPartialFile(string fileName, HttpContextBase requestContext, List<ViewDataUploadFilesResult> statuses)
        {
            //TODO - partial upload
            var request = requestContext.Request;
            if (request.Files.Count != 1) throw new HttpRequestValidationException("Attempt to upload chunked file containing more than one fragment per request");
            var file = request.Files[0];
            var inputStream = file.InputStream;
            String patchOnServer = Path.Combine(StorageRoot);
            var fullName = Path.Combine(patchOnServer, Path.GetFileName(file.FileName));
            using (var fs = new FileStream(fullName, FileMode.Append, FileAccess.Write))
            {
                var buffer = new byte[1024];

                var l = inputStream.Read(buffer, 0, 1024);
                while (l > 0)
                {
                    fs.Write(buffer, 0, l);
                    l = inputStream.Read(buffer, 0, 1024);
                }
                fs.Flush();
                fs.Close();
            }
            //statuses.Add(UploadResult(file.FileName, file.ContentLength, file.FileName));
        }

        //public IEnumerable<FilloggDetaljDTO> HamtaFillogg(int leveransId)
        //{
        //    var filloggar = _portalSosRepository.GetFilerForLeveransId(leveransId);

        //    return null;

        //    //return filloggar.Select(FilloggDetaljDTO.)
        //}


        private string GetHashAddOn(string orgKod, int levId)
        {
            var hashAddOn = String.Empty;

            hashAddOn = "#" + orgKod + "!" + levId;

            return hashAddOn;
        }


        public ViewDataUploadFilesResult UploadResult(String FileName,int fileSize,String FileFullPath, String SosFileName, int LeveransId, int SequenceNumber, string Timestamp)
        {
            String getType = System.Web.MimeMapping.GetMimeMapping(FileFullPath);
            var result = new ViewDataUploadFilesResult()
            {
                name = FileName,
                size = fileSize,
                type = getType,
                url = UrlBase + FileName,
                deleteUrl = DeleteURL + FileName,
                thumbnailUrl = CheckThumb(getType, FileName),
                deleteType = DeleteType,
                sosName = SosFileName,
                leveransId = LeveransId,
                sequenceNumber = SequenceNumber,
                timestamp = Timestamp
            };
            return result;
        }

        //public ViewDataUploadFilesResult UploadResult(String FileName, int fileSize, String FileFullPath)
        //{
        //    String getType = System.Web.MimeMapping.GetMimeMapping(FileFullPath);
        //    var result = new ViewDataUploadFilesResult()
        //    {
        //        name = FileName,
        //        size = fileSize,
        //        type = getType,
        //        url = UrlBase + FileName,
        //        deleteUrl = DeleteURL + FileName,
        //        thumbnailUrl = CheckThumb(getType, FileName),
        //        deleteType = DeleteType
        //    };
        //    return result;
        //}

        public String CheckThumb(String type,String FileName)
        {
            var splited = type.Split('/');
            if (splited.Length == 2)
            {
                string extansion = splited[1].ToLower();
                if(extansion.Equals("jpeg") || extansion.Equals("jpg") || extansion.Equals("png") || extansion.Equals("gif"))
                {
                    String thumbnailUrl = UrlBase + "thumbs/" + Path.GetFileNameWithoutExtension(FileName) + "80x80.jpg";
                    return thumbnailUrl;
                }
                else
                {
                    if (extansion.Equals("octet-stream")) //Fix for exe files
                    {
                        return "/Content/Free-file-icons/48px/exe.png";

                    }
                    if (extansion.Contains("zip")) //Fix for exe files
                    {
                        return "/Content/Free-file-icons/48px/zip.png";
                    }
                    String thumbnailUrl = "/Content/Free-file-icons/48px/"+ extansion +".png";
                    return thumbnailUrl;
                }
            }
            else
            {
                return UrlBase + "/thumbs/" + Path.GetFileNameWithoutExtension(FileName) + "80x80.jpg";
            }
           
        }


        //public List<String> FilesList()
        //{

        //    List<String> Filess = new List<String>();
        //    string path = HostingEnvironment.MapPath(serverMapPath);
        //    System.Diagnostics.Debug.WriteLine(path);
        //    if (Directory.Exists(path))
        //    {
        //        DirectoryInfo di = new DirectoryInfo(path);
        //        foreach (FileInfo fi in di.GetFiles())
        //        {
        //            Filess.Add(fi.Name);
        //            System.Diagnostics.Debug.WriteLine(fi.Name);
        //        }

        //    }
        //    return Filess;
        //}

        private void CreateAndUploadPARStatusFile(int levId, List<ViewDataUploadFilesResult> resultList )
        {
            var datStr = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            String pathOnServer = Path.Combine(StorageRoot);
            var filepath = pathOnServer + "Filleverans_" + levId + "_" + datStr + ".csv";
            using (StreamWriter writer = new StreamWriter(new FileStream(filepath, FileMode.Create, FileAccess.Write)))
            {
                //writer.WriteLine("sep=,");
                writer.WriteLine("Lev_ID,FileName,Status");
                foreach (var file in resultList)
                {
                    var fileName = file.sosName;
                    writer.WriteLine(levId + "," + fileName + ",0");
                }
            }
        }


        private void NotifyCaseManager(List<ViewDataUploadFilesResult> uploadResult, Arende arende, string userName)
        {
            //List<FileInfo> filesInFolder = dir.GetFiles().OrderByDescending(p => p.CreationTime).ToList();
            var arendeansvarig = _portalSosRepository.GetCaseResponsible(arende.ArendeansvarId);
            var arendeansvarigEpostadress = arendeansvarig.Epostadress;
            var arendetyp = _portalSosRepository.GetCaseType(arende.ArendetypId);
            string subject = "Indata till ärende " + arende.Arendenr + " har inkommit";
            string body = "Mejladress " + userName + " har skickat in fil/filer till nedanstående ärende.<br><br>";
            body += "Ärendetyp: " + arende.Arendetyp.ArendetypNamn + " <br>";
            body += "Ärendenummer: " + arende.Arendenr + "<br> ";
            body += "Ärendenamn: " + arende.Arendenamn + " <br><br>";
            body += "Följande fil/filer har inkommit: <br>";
            body += "<table><tr><td style=\"text-align:left;width:300px\">Filnamn</td><td style=\"text-align:right;width:150px;padding-right:25px\">Filstorlek (byte)</td>";
            body += "<td style=\"text-align:left;width:500px\">Nytt filnamn</td><td style=\"text-align:left;width:200px\">Tidpunkt</td></tr>";
            body += "<tbody>";
            foreach (var result in uploadResult)
            {
                body += "<tr><td style=\"text-align:left;width:300px\">" + result.name + "</td><td style=\"text-align:right;width:150px;padding-right:25px\">" + result.size +
                        "</td><td style=\"text-align:left;width:500px\">" + result.sosName + "</td><td style=\"text-align:left;width:200px\">" + result.timestamp + "</td></tr>";
            }
            body += "</tbody></table><br><br>Hälsningar inrapporteringsservice<br>";

            MailMessage msg = new MailMessage();
            msg.IsBodyHtml = true;
            MailAddress toMail = new MailAddress(arendeansvarigEpostadress);
            msg.To.Add(toMail);
            MailAddress fromMail = new MailAddress("no-reply.inrapportering@socialstyrelsen.se");
            msg.From = fromMail;
            //CC:a ev epostadresser för ärendetyp
            if (!String.IsNullOrEmpty(arendetyp.KontaktpersonerStr))
            {
                //var contacts = arendetyp.KontaktpersonerStr.Replace(' ', ',');
                var newEmailStr = arendetyp.KontaktpersonerStr.Split(',');

                foreach (var email in newEmailStr)
                {
                    if (!String.IsNullOrEmpty(email.Trim()))
                    {
                        MailAddress emailadress = new MailAddress(email.Trim());
                        msg.CC.Add(emailadress);
                    }
                }
            }

            msg.Subject = subject;
            msg.Body = body;
            _mailHelper.SendEmail(msg);
        }

        private void NotifyFileUploader(List<ViewDataUploadFilesResult> uploadResult, Arende arende, string userName)
        {
            string subject = "Socialstyrelsen har tagit emot filer i ärende " + arende.Arendenr;
            string body = "Hej! <br><br>";
            body += "Vi har tagit emot " + uploadResult.Count + " fil/filer i ärende " + arende.Arendenr + " " + arende.Arendenamn + ".<br><br>";
            body += "För mer detaljerad information om vilka filer som vi tagit emot, vänligen logga in på ditt <a href=\"https://filip.socialstyrelsen.se\">Filip-konto</a>.<br>";
            body += "Historiken för uppladdade filer hittar du genom att välja <i>Lämna filer</i> i menyn högst upp och därefter i listrutan välja det ärende som avses.<br><br><br>";
 
            body += "<br><br>Hälsningar inrapporteringsservice<br>";

            MailMessage msg = new MailMessage();
            msg.IsBodyHtml = true;
            MailAddress toMail = new MailAddress(userName);
            msg.To.Add(toMail);
            MailAddress fromMail = new MailAddress("no-reply.inrapportering@socialstyrelsen.se");
            msg.From = fromMail;
            msg.Subject = subject;
            msg.Body = body;
            _mailHelper.SendEmail(msg);
        }


        //************* Files on disk *******************//

        public IEnumerable<FileInfo> GetAllTemplateFiles()
        {
            var templateFileList = new List<FileInfo>();
            var fileareaPath = ConfigurationManager.AppSettings["TemplatesNetworkPath"];
            var _filesInFolder = new List<FileInfo>();

            //Om utvecklingsmiljön
            if (ConfigurationManager.AppSettings["Env"] == "Utv")
            {
                DirectoryInfo dir = new DirectoryInfo(fileareaPath);
                _filesInFolder = dir.GetFiles().OrderByDescending(p => p.CreationTime).ToList();
            }
            else //KT,AT,Prod
            {
                //var credentials = new NetworkCredential(@ConfigurationManager.AppSettings["NetworkUser"],
                //    ConfigurationManager.AppSettings["NetworkPwd"], ConfigurationManager.AppSettings["DomainToReach"]);
                var credentials = new NetworkCredential(@ConfigurationManager.AppSettings["NetworkUser"],
                    ConfigurationManager.AppSettings["NetworkPwd"]);

                try
                {
                    using (new NetworkConnection(fileareaPath, credentials))
                    {
                        DirectoryInfo dir = new DirectoryInfo(fileareaPath);
                        _filesInFolder = dir.GetFiles().OrderByDescending(p => p.CreationTime).ToList();
                    }
                }
                catch (Win32Exception e)
                {
                    ErrorManager.WriteToErrorLog("FilesHelper", "UploadTemplateFileAndShowResults", e.ToString(),
                        e.HResult);
                    Console.WriteLine(e.Message);
                    Console.ReadLine();
                }
                catch (Exception ex)
                {
                    ErrorManager.WriteToErrorLog("FilesHelper", "UploadTemplateFileAndShowResults", ex.ToString(),
                        ex.HResult);
                    Console.WriteLine(ex.Message);
                    Console.ReadLine();
                }
            }

            //foreach (var templateFile in _filesInFolder)
            //{
            //    var datCreate = templateFile.CreationTime;
            //    var datModified = templateFile.LastWriteTime;
            //    var size = templateFile.Length;
            //    var name = templateFile.Name;
            //}
            return _filesInFolder;
        }

        public void DeleteTemplateFile(string filename)
        {
            var _fileareaPath = ConfigurationManager.AppSettings["TemplatesNetworkPath"];
            var _filesInFolder = new List<FileInfo>();
            //Om utvecklingsmiljön
            if (ConfigurationManager.AppSettings["Env"] == "Utv")
            {
                DirectoryInfo dir = new DirectoryInfo(_fileareaPath);
                _filesInFolder = dir.GetFiles().OrderByDescending(p => p.CreationTime).ToList();
            }
            else //KT,AT,Prod
            {
                var credentials = new NetworkCredential(@ConfigurationManager.AppSettings["NetworkUser"],
                    ConfigurationManager.AppSettings["NetworkPwd"]);
                try
                {
                    using (new NetworkConnection(_fileareaPath, credentials))
                    {
                        DirectoryInfo dir = new DirectoryInfo(_fileareaPath);
                        _filesInFolder = dir.GetFiles().OrderByDescending(p => p.CreationTime).ToList();
                    }
                }
                catch (Win32Exception e)
                {
                    ErrorManager.WriteToErrorLog("FilesHelper", "DeleteTemplateFile", e.ToString(),
                        e.HResult);
                    Console.WriteLine(e.Message);
                    Console.ReadLine();
                }
                catch (Exception ex)
                {
                    ErrorManager.WriteToErrorLog("FilesHelper", "DeleteTemplateFile", ex.ToString(),
                        ex.HResult);
                    Console.WriteLine(ex.Message);
                    Console.ReadLine();
                }
            }

            foreach (var templateFile in _filesInFolder)
            {
                if (templateFile.Name == filename)
                {
                    templateFile.Delete();
                }
            }
        }



        public byte[] DownloadFile(string filename)
        {
            var dir = ConfigurationManager.AppSettings["DirForFeedback"];
            string filepath = dir + filename;
            byte[] filedata = new byte[] { };

            if (ConfigurationManager.AppSettings["Env"] == "Utv")
            {
                filedata = System.IO.File.ReadAllBytes(filepath);
            }
            else //KT,AT,Prod
            {
                var credentials = new NetworkCredential(@ConfigurationManager.AppSettings["NetworkUser"],
                    ConfigurationManager.AppSettings["NetworkPwd"]);
                try
                {
                    using (new NetworkConnection(filepath, credentials))
                    {
                        filedata = System.IO.File.ReadAllBytes(filepath);
                    }
                }
                catch (Win32Exception e)
                {
                    ErrorManager.WriteToErrorLog("FilesHelper", "DownloadFile", e.ToString(),e.HResult);
                    Console.WriteLine(e.Message);
                    Console.ReadLine();
                }
                catch (Exception ex)
                {
                    ErrorManager.WriteToErrorLog("FilesHelper", "DownloadFile", ex.ToString(),ex.HResult);
                    Console.WriteLine(ex.Message);
                    Console.ReadLine();
                }

            }

            return filedata;
        }



        //*************  Files in db  *******************//

        public IEnumerable<AdmDokument> GetAllFilesFromDb()
        {
            var fileList = _portalSosRepository.GetAllFiles();
            return fileList;
        }

        public AdmDokument GetFileFromDb(int fileId)
        {
            var file = _portalSosRepository.GetFile(fileId);
            return file;
        }

        public void SaveFile(AdmDokument file, string userName)
        {
            //Sätt datum och användare
            file.SkapadDatum = DateTime.Now;
            file.SkapadAv = userName;
            file.AndradDatum = DateTime.Now;
            file.AndradAv = userName;
            _portalSosRepository.SaveFile(file);
        }

        public void UpdateFile(AdmDokument file, string userName)
        {
            //Sätt datum och användare
            file.AndradDatum = DateTime.Now;
            file.AndradAv = userName;
            _portalSosRepository.UpdateFile(file);
        }

        public void DeleteFile(AdmDokument file)
        {
            _portalSosRepository.DeleteFile(file);
        }

    }

    public class ViewDataUploadFilesResult
    {
        public string name { get; set; }
        public int size { get; set; }
        public string type { get; set; }
        public string url { get; set; }
        public string deleteUrl { get; set; }
        public string thumbnailUrl { get; set; }
        public string deleteType { get; set; }
        public string sosName { get; set; }
        public int leveransId { get; set; }
        public int sequenceNumber { get; set; }
        public string timestamp { get; set; }
    }
    public class JsonFiles
    {
        public ViewDataUploadFilesResult[] files;
        public string TempFolder { get; set; }
        public JsonFiles(List<ViewDataUploadFilesResult> filesList)
        {
            files = new ViewDataUploadFilesResult[filesList.Count];
            for (int i = 0; i < filesList.Count; i++)
            {
                files[i] = filesList.ElementAt(i);
            }

        }
    }

}

    