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
        FilesHelper filesHelper;
        private GeneralHelper _generalHelper;

        private string StorageRoot
        {
            get { return Path.Combine(ConfigurationManager.AppSettings["uploadFolder"]); }
        }
        
        public FileUploadController()
        {
            filesHelper = new FilesHelper(StorageRoot);
            _generalHelper = new GeneralHelper();
            _portalSosService = new PortalSosService(new PortalSosRepository(new InrappSosDbContext(), new InrappSosAstridDbContext()));
        }

        [Authorize] 
        public ActionResult Index()
        {
            return View();
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

        public ActionResult CustomError(CustomErrorPageModel model)
        {
            return View(model);
        }

    }
}