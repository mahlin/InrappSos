using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InrappSos.ApplicationService;
using InrappSos.ApplicationService.Interface;
using InrappSos.DataAccess;
using InrappSos.AstridWeb.Helpers;
using InrappSos.AstridWeb.Models;

namespace InrappSos.AstridWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPortalSosService _portalSosService;

        public HomeController()
        {
            _portalSosService =
                new PortalSosService(new PortalSosRepository(new InrappSosDbContext(), new InrappSosIdentityDbContext()));

        }

        public ActionResult Index(AccountController.AccountMessageId? message)
        {
            ViewBag.StatusMessage =
                message == AccountController.AccountMessageId.AddUserSuccess ? "Användaren har registrerats."
                : "";

            return View();
        }

        public ActionResult About(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
            message == ManageMessageId.ChangePasswordSuccess ? "Ditt lösenord har ändrats."
            : message == ManageMessageId.SetPasswordSuccess ? "Ditt lösenord är sparat."
            : message == ManageMessageId.SetTwoFactorSuccess ? "Din två-faktor-autentiseringsleverantör är sparad."
            : message == ManageMessageId.Error ? "Ett fel har uppstått."
            : message == ManageMessageId.AddPhoneSuccess ? "Ditt mobilnummer har sparats."
            : message == ManageMessageId.RemovePhoneSuccess ? "Ditt mobilnummer har tagits bort."
            : message == ManageMessageId.ChangeChosenRegister ? "Valda register har registrerats."
            : message == ManageMessageId.ChangeNameSuccess ? "Ditt namn har ändrats."
            : "";

            try
            {
                //Uppdatera valda register
                var x = _portalSosService.HamtaHistorikForOrganisation(29);
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangeChosenRegister });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("Test", "test", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid byte av valda register.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            ChangeChosenRegister,
            ChangeNameSuccess,
            Error
        }
    }
}