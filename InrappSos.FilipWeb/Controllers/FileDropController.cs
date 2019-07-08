using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InrappSos.ApplicationService;
using InrappSos.ApplicationService.DTOModel;
using InrappSos.ApplicationService.Helpers;
using InrappSos.ApplicationService.Interface;
using InrappSos.DataAccess;
using InrappSos.DomainModel;
using InrappSos.FilipWeb.Models;
using InrappSos.FilipWeb.Models.ViewModels;
using Microsoft.AspNet.Identity;

namespace InrappSos.FilipWeb.Controllers
{
    public class FileDropController : Controller
    {
        private readonly IPortalSosService _portalService;
        private FileDropViewModel _model = new FileDropViewModel();
        FilesHelper filesHelper;
        private GeneralHelper _generalHelper;


        private string StorageRoot
        {
            get { return Path.Combine(ConfigurationManager.AppSettings["fileDropUploadFolder"]); }
        }

        public FileDropController()
        {
            filesHelper = new FilesHelper(StorageRoot);
            _portalService =
                new PortalSosService(new PortalSosRepository(new InrappSosDbContext()));
            _generalHelper = new GeneralHelper();

        }


        // GET: FileDrop
        public ActionResult Index()
        {
            try
            {
                //Kolla om öppet, annars visa stängt-sida
                if (!_portalService.IsOpen())
                {
                    var testOrg = _generalHelper.IsTestUser(User.Identity.GetUserId());
                    if (!testOrg)
                    {
                        ViewBag.Text = _portalService.HamtaInfoText("Stangtsida").Text;
                        return View("Closed");
                    }
                }
                var userOrg = _portalService.HamtaOrgForAnvandare(User.Identity.GetUserId());
                _model.OrganisationsNamn = userOrg.Organisationsnamn;

                //Hämta historik för användarens ärenden
                var historyFileList = _portalService.HamtaFildroppsHistorikForAnvandaresArenden(User.Identity.GetUserId()).ToList();
                _model.HistorikLista = historyFileList;

                var usersCases = _portalService.HamtaAnvandaresArenden(User.Identity.GetUserId()).ToList();
                // Ladda drop down list.  
                ViewBag.CaseList = CreateCaseDropDownList(usersCases);
                _model.SelectedCaseId = "0";

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("FileDropController", "Index", e.ToString(), e.HResult,
                    User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade på filuppladdningssidan.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
            return View(_model);
        }

        [HttpPost]
        [Authorize]
        public JsonResult Upload(FileDropViewModel model)
        {
            var userName = "";
            var resultList = new List<ViewDataUploadFilesResult>();
            try
            {
                userName = User.Identity.GetUserName();
                var CurrentContext = HttpContext;

                //Lägg till kontroll att antal filer > 0 (kan ha stoppats av användarens webbläsare (?))
                var request = CurrentContext.Request;
                var numFiles = request.Files.Count;
                if (numFiles <= 0)
                {
                    throw new System.ArgumentException("Filer saknas vid uppladdning av fil.");
                }

                filesHelper.UploadFileDropFilesAndShowResults(CurrentContext, resultList, Convert.ToInt32(model.SelectedCaseId), User.Identity.GetUserId(), userName);
            }
            catch (ArgumentException e)
            {
                ErrorManager.WriteToErrorLog("FileDropController", "Upload", e.ToString(), e.HResult, User.Identity.Name);
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
                ErrorManager.WriteToErrorLog("FileDropController", "Upload", e.ToString(), e.HResult, User.Identity.Name);
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
                return Json(files);
            }
        }

        public ActionResult CustomError(CustomErrorPageModel model)
        {
            return View(model);
        }

        public ActionResult RefreshFilesHistory(FileDropViewModel model)
        {
            var userId = User.Identity.GetUserId();

            List<FildroppDetaljDTO> historyFileList = _portalService.HamtaFildroppsHistorikForAnvandaresArenden(userId).ToList();

            model.HistorikLista = historyFileList;

            return PartialView("_FilesHistory", model);
        }


        /// <summary>  
        /// Create list for register-dropdown  
        /// </summary>  
        /// <returns>Return register for drop down list.</returns>  
        private IEnumerable<SelectListItem> CreateCaseDropDownList(List<Arende> userCases)
        {
            SelectList lstobj = null;

            var list = userCases
                .Select(p =>
                    new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.Arendenr + ", " + p.Arendenamn
                    });

            // Setting.  
            lstobj = new SelectList(list, "Value", "Text");

            return lstobj;
        }
    }
}