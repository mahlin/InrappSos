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
using InrappSos.AstridWeb.Models;
using InrappSos.AstridWeb.Models.ViewModels;
using Microsoft.AspNet.Identity;

namespace InrappSos.AstridWeb.Controllers
{
    [MvcApplication.NoDirectAccessAttribute]
    public class FileUploadController : Controller
    {
        private readonly IPortalSosService _portalSosService;
        private FilesViewModel _model = new FilesViewModel();
        FilesHelper _filesHelper;
        private GeneralHelper _generalHelper;

        private string StorageRoot
        {
            get { return Path.Combine(ConfigurationManager.AppSettings["uploadFolder"]); }
        }
        
        public FileUploadController()
        {
            _filesHelper = new FilesHelper(StorageRoot);
            _generalHelper = new GeneralHelper();
            _portalSosService = new PortalSosService(new PortalSosRepository(new InrappSosDbContext(), new InrappSosAstridDbContext()));
        }

        [Authorize] 
        public ActionResult Index()
        {
            return View();
        }

        // GET: InformationTexts
        [Authorize(Roles = "Admin")]
        public ActionResult GetTemplateDocuments()
        {
            //var domain = Request.Url.Host;
            //ErrorManager.WriteToErrorLog("FileUploadController", "Domain", domain);

            var model = new FileUploadViewModels.FileUploadViewModel();
            try
            {
                var mallar = _filesHelper.GetAllTemplateFiles().ToList();
                model.Mallar = ConvertFileInfoToVM(mallar);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("FileUploadController", "GetTemplateDocuments", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid hämtning av mallar.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);

            }
            return View("HandleTemplates", model);
        }

        // GET
        [Authorize(Roles = "Admin")]
        public ActionResult AddTemplate()
        {
            return View();
        }


        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult AddTemplate(FileUploadViewModels.FileInfoViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userName = User.Identity.GetUserName();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorManager.WriteToErrorLog("FileUploadController", "Addtemplate", e.ToString(), e.HResult, User.Identity.Name);
                    var errorModel = new CustomErrorPageModel
                    {
                        Information = "Ett fel inträffade när ny mall skulle sparas.",
                        ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                    };
                    return View("CustomError", errorModel);
                }
                return RedirectToAction("GetTemplateDocuments");
            }

            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult DeleteTemplate(string filename)
        {
            try
            {
                _filesHelper.DeleteTemplateFile(filename);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("FileUploadController", "DeleteTemplate", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade när mall skulle tas bort.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
            return RedirectToAction("GetTemplateDocuments");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
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

                _filesHelper.UploadTemplateFileAndShowResults(CurrentContext, resultList, User.Identity.GetUserId(), userName);
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

                //View file
                //Download file
                //Öppnar excel
                return File(filedata, contentType);
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

        private List<FileUploadViewModels.FileInfoViewModel> ConvertFileInfoToVM(List<FileInfo> files)
        {
            var filesList = new List<FileUploadViewModels.FileInfoViewModel>();
            var starturl = ConfigurationManager.AppSettings["TemplatesUrl"];


            foreach (var file in files)
            {
                var url = Path.Combine(starturl, file.Name);

                var fileVM = new FileUploadViewModels.FileInfoViewModel
                {
                    Filename = file.Name,
                    LastWriteTime = file.LastWriteTime,
                    CreationTime = file.CreationTime,
                    Length = file.Length,
                    Path = url
                };
                filesList.Add(fileVM);
            };
            return filesList;
        }

        public ActionResult CustomError(CustomErrorPageModel model)
        {
            return View(model);
        }

    }
}