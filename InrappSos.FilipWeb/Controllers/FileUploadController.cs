using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.Mvc;
using InrappSos.DataAccess;
using InrappSos.ApplicationService;
using InrappSos.ApplicationService.DTOModel;
using InrappSos.ApplicationService.Interface;
using InrappSos.ApplicationService.Helpers;
using InrappSos.DomainModel;
using InrappSos.FilipWeb.Models;
using InrappSos.FilipWeb.Models.ViewModels;
using Microsoft.AspNet.Identity;

namespace InrappSos.FilipWeb.Controllers
{
    [MvcApplication.NoDirectAccessAttribute]
    public class FileUploadController : Controller
    {
        private readonly IPortalSosService _portalService;
        private FilesViewModel _model = new FilesViewModel();
        FilesHelper filesHelper;
        private GeneralHelper _generalHelper;
        //String tempPath = "~/somefiles/";
        //String serverMapPath = "~/UppladdadeRegisterfiler/";

        private string StorageRoot
        {
            get { return Path.Combine(ConfigurationManager.AppSettings["uploadFolder"]); }
        }

        //private string UrlBase = "/Files/somefiles/";
        //String DeleteURL = "/FileUpload/DeleteFile/?file=";
        //String DeleteType = "GET";

        public FileUploadController()
        {
            filesHelper = new FilesHelper(StorageRoot);
            //filesHelper = new FilesHelper(DeleteURL, DeleteType, StorageRoot, UrlBase, tempPath, serverMapPath);
            _generalHelper = new GeneralHelper();

            _portalService =
                new PortalSosService(new PortalSosRepository(new InrappSosDbContext()));
        }

        [Authorize] 
        public ActionResult Index()
        {
            try
            {
                //Kolla om öppet, annars visa stängt-sida
                if (!_portalService.IsOpen())
                {
                    //Om användaren tillhör en test-organisation så ska hen släppas in även om portalen är stängd (#335)
                    //var tmp = User;
                    //var testTeamUser = UserManager.FindByNameAsync(User.Identity.GetUserName());
                    //var testTeamUserOrg = _portalService.HamtaOrgForAnvandare(User.Identity.GetUserId());
                    //var testOrg = IsTestOrg(testTeamUserOrg.Id);
                    var testOrg = _generalHelper.IsTestUser(User.Identity.GetUserId());
                    if (!testOrg)
                    {
                        ViewBag.Text = _portalService.HamtaInfoText("Stangtsida").Text;
                        return View("Closed");
                    }
                    //ViewBag.Text = _portalService.HamtaInfoText("Stangtsida").Text;
                    //return View("Closed");
                }
                var userOrg = _portalService.HamtaOrgForAnvandare(User.Identity.GetUserId());
                //Hämta info om valbara register
                var valdaDelregisterInfoList = _portalService.HamtaValdaDelregisterForAnvandare(User.Identity.GetUserId(), userOrg.Id);
                _model.RegisterList = valdaDelregisterInfoList;

                // Ladda drop down lists.  
                this.ViewBag.RegisterList = CreateRegisterDropDownList(valdaDelregisterInfoList);
                _model.SelectedRegisterId = "0";
                _model.SelectedPeriod = "0";

                //Hämta historiken för användarens organisation/kommun
                var userId = User.Identity.GetUserId();

                //var kommunKodForUser = userOrg.Kommunkod;
                var orgIdForUser = userOrg.Id;

                _model.StartUrl = ConfigurationManager.AppSettings["StartUrl"];
                _model.GiltigKommunKod = userOrg.Kommunkod;
                _model.GiltigLandstingsKod = userOrg.Landstingskod;
                _model.GiltigInrapporteringsKod = userOrg.Inrapporteringskod;
                _model.OrganisationsNamn = userOrg.Organisationsnamn;

                var historyFileList = _portalService.HamtaTop10HistorikForOrganisationAndDelreg(orgIdForUser, valdaDelregisterInfoList).ToList();

                //Filtrera historiken utfrån användarens valda register
                IEnumerable<FilloggDetaljDTO> filteredHistoryFileList = _portalService.FiltreraHistorikForAnvandare(userId, valdaDelregisterInfoList, historyFileList);

                _model.HistorikLista = historyFileList;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("FileUploadController", "Index", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade på filuppladdningssidan.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }

            return View(_model);
        }

        public ActionResult Show()
        {
            JsonFiles ListOfFiles = filesHelper.GetFileList();


            var model = new FilesViewModel()
            {
                Files = ListOfFiles.files,
                //HistorikLista = historyFileList.ToList()
            };

            return View(model);
        }

        public ActionResult Edit()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public JsonResult Upload(FilesViewModel model)
        {
            var enhetskod = String.Empty;
            var resultList = new List<ViewDataUploadFilesResult>();
            var userName = "";
            try
            {
                //Get orgcode
                var orgCode = String.Empty;
                var org = _portalService.HamtaOrgForAnvandare(User.Identity.GetUserId());
                var orgtypeListForOrg = _portalService.HamtaOrgtyperForOrganisation(org.Id);
                var orgtypeListForSubDir = _portalService.HamtaOrgtyperForDelregister(Convert.ToInt32(model.SelectedRegisterId));

                //Compare organisations orgtypes with orgtypes for current subdir
                foreach (var subDirOrgtype in orgtypeListForSubDir)
                {
                    foreach (var orgOrgtype in orgtypeListForOrg)
                    {
                        if (subDirOrgtype.Typnamn == orgOrgtype.Typnamn )
                        {
                            if (orgOrgtype.Typnamn == "Kommun")
                            {
                                orgCode = _portalService.HamtaKommunKodForAnvandare(User.Identity.GetUserId());
                            }
                            else if (orgOrgtype.Typnamn == "Landsting")
                            {
                                orgCode = _portalService.HamtaLandstingsKodForAnvandare(User.Identity.GetUserId());
                            }
                            else
                            {
                                orgCode = _portalService.HamtaInrapporteringskodKodForAnvandare(User.Identity.GetUserId());
                            }
                        }
                    }
                }

                //var orgCodeName = orgtypeList[0].Typnamn;
                //if (orgCodeName == "Kommun")
                //{
                //    orgCode = _portalService.HamtaKommunKodForAnvandare(User.Identity.GetUserId());
                //}
                //else if (orgCodeName == "Landsting")
                //{
                //    orgCode = _portalService.HamtaLandstingsKodForAnvandare(User.Identity.GetUserId());
                //}
                //else
                //{
                //    orgCode = _portalService.HamtaInrapporteringskodKodForAnvandare(User.Identity.GetUserId());
                //}
                //var kommunKod = _portalService.HamtaKommunKodForAnvandare(User.Identity.GetUserId());
                userName = User.Identity.GetUserName();
                var CurrentContext = HttpContext;

                if (model.SelectedUnitId != null)
                {
                    enhetskod = model.SelectedUnitId;
                    if (String.IsNullOrEmpty(orgCode))
                    orgCode = enhetskod;
                }
                    

                //Lägg till kontroll att antal filer > 0 (kan ha stoppats av användarens webbläsare (?))
                var request = CurrentContext.Request;
                var numFiles = request.Files.Count;
                if (numFiles <= 0)
                {
                    throw new System.ArgumentException("Filer saknas vid uppladdning av fil.");
                }

                filesHelper.UploadAndShowResults(CurrentContext, resultList, User.Identity.GetUserId(), userName,
                    orgCode, Convert.ToInt32(model.SelectedRegisterId), enhetskod, model.SelectedPeriod,
                    model.RegisterList);
            }
            catch (ArgumentException e)
            {
                ErrorManager.WriteToErrorLog("FileUploadController", "Upload", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Filer saknas vid uppladdning av fil.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                RedirectToAction("CustomError", new { model = errorModel });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("FileUploadController", "Upload", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid uppladdning av fil.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                RedirectToAction("CustomError", new { model = errorModel });
            }

            JsonFiles files = new JsonFiles(resultList);

            bool isEmpty = !resultList.Any();
            if (isEmpty)
            {
                return Json("Error");
            }
            else
            {
                //Save to database filelog
                foreach (var itemFile in resultList)
                {
                    try
                    {
                        _portalService.SparaTillDatabasFillogg(userName, itemFile.name, itemFile.sosName, itemFile.leveransId, itemFile.sequenceNumber);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        ErrorManager.WriteToErrorLog("FileUploadController", "Upload", e.ToString(), e.HResult, User.Identity.Name);
                        var errorModel = new CustomErrorPageModel
                        {
                            Information = "Ett fel inträffade när filen skulle sparas till registrets logg.",
                            ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                        };
                        RedirectToAction("CustomError", new { model = errorModel});
                    }
                }
                return Json(files);
            }
        }

        [Authorize]
        public ActionResult DownloadFile(string filename)
        {
            try
            {
                var dir = WebConfigurationManager.AppSettings["DirForFeedback"];
                string filepath = dir + filename;
                byte[] filedata = System.IO.File.ReadAllBytes(filepath);
                string contentType = MimeMapping.GetMimeMapping(filepath);

                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = filename,
                    Inline = true,
                };

                Response.Headers.Add("Content-Disposition", cd.ToString());

                var a =  File(filedata, contentType);
                //View file
                var x =  File(filedata, MediaTypeNames.Text.Plain);
                //Download file
                var y =  File(filedata, MediaTypeNames.Text.Plain, "Test.txt");

                //Öppnar excel
                return File(filedata, contentType);

                    //Funkar ej
                    //return File(filedata, MediaTypeNames.Text.Plain, "Test.txt");

                    //Öppnar filen as is
                    //return File(filedata, MediaTypeNames.Text.Plain);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("FileUploadController", "DownloadFile", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid öppningen av återkopplingsfilen",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);

            }
        }



        public JsonResult GetFileList()
        {
            var list=filesHelper.GetFileList();
            return Json(list,JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult DeleteFile(string file)
        {
            filesHelper.DeleteFile(file);
            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        public void AddRegisterListToViewBag(int kommunKod)
        {
            _model.SelectedRegisterId = "0";
            ViewBag.RegisterList = CreateRegisterDropDownList(_model.RegisterList);
        }

        //public IEnumerable<RegisterInfo> GetRegisterInfo()
        //{
        //    var allaRegisterList= _portalService.HamtaAllRegisterInformation();

        //    //Visa bara de register som användaren valt att repportera till
        //    var chosenRegisters = _portalService.HamtaValdaRegistersForAnvandare(User.Identity.GetUserId());

        //    foreach (var register in allaRegisterList)
        //    {
        //        foreach (var VARIABLE in COLLECTION)
        //        {
                    
        //        }
        //    }

            
        //    return registerList;
        //}

        
        /// <summary>  
        /// Create list for register-dropdown  
        /// </summary>  
        /// <returns>Return register for drop down list.</returns>  
        private IEnumerable<SelectListItem> CreateRegisterDropDownList(List<RegisterInfo> valdaDelregisterInfoList)
        {
            SelectList lstobj = null;

            var list = valdaDelregisterInfoList
                .Select(p =>
                    new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.Namn
                    });

            // Setting.  
            lstobj = new SelectList(list, "Value", "Text");

            return lstobj;
        }
         
        
        //public FilesViewModel UpdateHistory()
        //{
        //    var model = new FilesViewModel();
        //    //TODO - refresh Historiklistan, annan model
        //    var userId = User.Identity.GetUserId();
        //    var orgIdForUser = _portalService.HamtaUserOrganisationId(userId);
        //    IEnumerable<FilloggDetaljDTO> historyFileList = _portalService.HamtaHistorikForOrganisation(orgIdForUser);
        //    model.HistorikLista = historyFileList.ToList();

        //    return model;
        //}

        public ActionResult RefreshFilesHistory(FilesViewModel model)
        {
            var userId = User.Identity.GetUserId();
            var orgIdForUser = _portalService.HamtaUserOrganisationId(userId);
            var valdaDelregisterInfoList = _portalService.HamtaValdaDelregisterForAnvandare(User.Identity.GetUserId(), orgIdForUser).ToList();

            //List<FilloggDetaljDTO> historyFileList = _portalService.HamtaHistorikForOrganisation(orgIdForUser).ToList();
            var historyFileList = _portalService.HamtaTop10HistorikForOrganisationAndDelreg(orgIdForUser, valdaDelregisterInfoList).ToList();

            //Filtrera historiken utfrån användarens valda register
            IEnumerable<FilloggDetaljDTO> filteredHistoryFileList = _portalService.FiltreraHistorikForAnvandare(userId, valdaDelregisterInfoList, historyFileList);

            model.HistorikLista = filteredHistoryFileList.ToList();


            //return Json(historyFileList, JsonRequestBehavior.AllowGet);


            //return Json(new JsonResult()
            //{
            //    Data = "Result"
            //}, JsonRequestBehavior.AllowGet);

            return PartialView("_FilesHistory", model);
        }

        public ActionResult CustomError(CustomErrorPageModel model)
        {
            return View(model);
        }

        [HttpPost]
        public ActionResult IngetAttRapportera(FilesViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //Hämta orgId, skapa leverans för orgId, spara i db
                    var orgId = _portalService.HamtaUserOrganisationId(User.Identity.GetUserId());
                    var orgenhet = new Organisationsenhet();
                    if (!String.IsNullOrEmpty(model.IngetAttRapporteraForSelectedUnitId))
                    {
                        orgenhet = _portalService.HamtaOrganisationsenhetMedEnhetskod(model.IngetAttRapporteraForSelectedUnitId, orgId);
                    }
                    var id = Convert.ToInt32(model.IngetAttRapporteraForRegisterId);
                    
                    var forvLevId = _portalService.HamtaForvantadleveransIdForRegisterOchPeriod(Convert.ToInt32(model.IngetAttRapporteraForRegisterId),model.IngetAttRapporteraForPeriod);
                    var levId = _portalService.HamtaNyttLeveransId(User.Identity.GetUserId(),User.Identity.GetUserName(), orgId, Convert.ToInt32(model.IngetAttRapporteraForRegisterId), orgenhet.Id, forvLevId,
                        "Inget att rapportera");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorManager.WriteToErrorLog("FileUploadController", "IngetAttRapportera", e.ToString(), e.HResult, User.Identity.Name);
                    var errorModel = new CustomErrorPageModel
                    {
                        Information = "Ett fel inträffade när information om att inget finns att rapportera för aktuell period skulle sparas.",
                        ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                    };
                    return View("CustomError", errorModel);

                }
            }
            return RedirectToAction("Index");

        }

    }
}