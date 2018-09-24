using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using InrappSos.DataAccess;
using InrappSos.ApplicationService;
using InrappSos.ApplicationService.Helpers;
using InrappSos.ApplicationService.Interface;
using InrappSos.FilipWeb.Models;
using InrappSos.FilipWeb.Models.ViewModels;

namespace InrappSos.FilipWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPortalSosService _portalSosService =
            new PortalSosService(new PortalSosRepository(new InrappSosDbContext()));

        public ActionResult Index()
        {
            try
            {
                //Kolla om avvikande öppettider finns inom den kommande veckan
                var str = _portalSosService.ClosedComingWeek();
                if (str != String.Empty)
                    ViewBag.AvvikandeOppettider = "Avvikande öppettider<br/>" + str;
                    //"Avvikande öppettider<br/>Måndag 20 augusti 10 - 16 Underhåll <br/>Måndag 24 december stängt Julafton <br/> ";

                    //Kolla om öppet, annars visa stängt-sida
                if (_portalSosService.IsOpen())
                {
                    ViewBag.Text = _portalSosService.HamtaInfoText("Startsida");

                    return View();
                }
                else
                {
                    if (_portalSosService.IsHelgdag() || _portalSosService.IsSpecialdag())
                    {
                        ViewBag.Text = _portalSosService.HamtaHelgEllerSpecialdagsInfo();
                    }
                    else
                    {
                        ViewBag.Text = _portalSosService.HamtaInfoText("Stangtsida");
                    }
                    return View("Closed");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("HomeController", "Index", e.ToString(), e.HResult);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid öppning av startsidan.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
        }

        public ActionResult About( bool closed = false)
        {
            try
            {
                //Hamta FAQs
                var model = new AboutViewModel();
                model.FaqCategories = _portalSosService.HamtaAllaFAQs();
                model.PortalClosed = closed;
                ViewBag.Text = _portalSosService.HamtaInfoText("Hjalpsida");
                return View(model);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("HomeController", "About", e.ToString(), e.HResult);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid öppning av hjälpsidan.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
        }

        public ActionResult Contact(bool closed = false)
        {
            try
            {
                var model = new AboutViewModel();
                model.PortalClosed = closed;
                ViewBag.Text = _portalSosService.HamtaInfoText("Kontaktsida");
                return View(model);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("HomeController", "Contact", e.ToString(), e.HResult);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid öppning av kontaktsidan.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
        }
    }
}
