using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using InrappSos.ApplicationService;
using InrappSos.ApplicationService.DTOModel;
using InrappSos.ApplicationService.Interface;
using InrappSos.DataAccess;
using InrappSos.DomainModel;
using InrappSos.AstridWeb.Helpers;
using InrappSos.AstridWeb.Models;
using InrappSos.AstridWeb.Models.ViewModels;
using Microsoft.AspNet.Identity;

namespace InrappSos.AstridWeb.Controllers
{
    public class SystemController : Controller
    {
        private readonly IPortalSosService _portalSosService;

        public SystemController()
        {
            _portalSosService = new PortalSosService(new PortalSosRepository(new InrappSosDbContext(), new InrappSosAstridDbContext()));

        }

        // GET: System
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        // GET: FAQCategories
        [Authorize]
        public ActionResult GetFAQCategories()
        {
            var model = new SystemViewModels.SystemViewModel();
            try
            {
                model.FAQCategories = _portalSosService.HamtaFAQkategorier();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("SystemController", "GetFAQCategories", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid hämtning av FAQ-kategorier",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);

            }
            return View("EditFAQCategory", model);
        }


        // GET: FAQs
        [Authorize]
        public ActionResult GetFAQs(int faqCatId = 0, string faqCatName = "")
        {
            var model = new SystemViewModels.SystemViewModel();
            try
            {
                var faqs = _portalSosService.HamtaFAQs(faqCatId);
                model.FAQs = ConvertAdmFAQToViewModel(faqs.ToList());
                model.SelectedFAQCategory = faqCatId;
                if (faqCatId != 0 && faqCatName.IsEmpty())
                {
                    var faqCat = _portalSosService.HamtaFAQKategori(faqCatId);
                    model.SelectedFAQCategoryName = faqCat.Kategori;
                }
                else
                {
                    model.SelectedFAQCategoryName = faqCatName;
                }
                
                // Ladda drop down lists. 
                var registerList = _portalSosService.HamtaAllaRegisterForPortalen();
                this.ViewBag.RegisterList = CreateRegisterDropDownList(registerList);
                //model.SelectedFAQ.SelectedRegisterId = 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("SystemController", "GetFAQs", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid hämtning av FAQs",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);

            }
            return View("EditFAQs", model);
        }


        // GET: InformationTexts
        [Authorize]
        [ValidateInput(false)]
        public ActionResult GetInformationTexts(string selectedInfoType = "", string selectedText = "")
        {
            var model = new SystemViewModels.SystemViewModel();
            try
            {
                model.InfoPages = _portalSosService.HamtaInformationstexter();
                model.SelectedInfo = selectedInfoType;
                model.SelectedInfoText = selectedText;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("SystemController", "GetInformationTexts", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid hämtning av informationstexter.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);

            }
            return View("EditInfoTexts", model);
        }


        // GET: OpeningHours (AdmKonfiguration)
        [Authorize]
        public ActionResult GetOpeningHours()
        {
            var model = new SystemViewModels.OpeningHours();
            try
            {
                var admKonf = _portalSosService.HamtaOppettider();
                model.ClosedAnyway = admKonf.ClosedAnyway;
                model.ClosedDaysList = _portalSosService.MarkeraStangdaDagar(admKonf.ClosedDays);
                //model.OpeningTime = SetTime(admKonf.ClosedToHour, admKonf.ClosedToMin);
                //model.ClosingTime = SetTime(admKonf.ClosedFromHour, admKonf.ClosedFromMin);
                DateTime s = DateTime.MinValue;
                TimeSpan ts = new TimeSpan(10, 30, 0);
                model.OpeningTime = s.Date + ts;
                model.OpeningTimeStr = admKonf.ClosedToHour.ToString() + ":" + admKonf.ClosedToMin.ToString();
                //model.ClosingTime = SetTime(admKonf.ClosedFromHour, admKonf.ClosedFromMin);
                model.ClosingTimeStr = admKonf.ClosedFromHour.ToString() + ":" + admKonf.ClosedFromMin.ToString();
                model.InfoTextForClosedPage = _portalSosService.HamtaInfoText("Stangtsida").Text;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("SystemController", "GetOpeningHours", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid hämtning av öppettider.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);

            }
            return View("EditOpeningHours", model);
        }


        // GET: Helgdagar
        [Authorize]
        public ActionResult GetHolidays()
        {
            var model = new SystemViewModels.SystemViewModel();
            try
            {
                var holidayList = _portalSosService.HamtaAllaHelgdagar();
                model.Holidays = ConvertAdmHelgdagToViewModel(holidayList.ToList());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("SystemController", "GetHolidays", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid hämtning av helgdagar.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);

            }
            return View("EditHolidays", model);
        }

        // GET: Specialdagar
        [Authorize]
        public ActionResult GetSpecialDays()
        {
            var model = new SystemViewModels.SystemViewModel();
            try
            {
                var specialdaysList = _portalSosService.HamtaAllaSpecialdagar();
                model.SpecialDays = ConvertAdmSpecialdagToViewModel(specialdaysList.ToList());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("SystemController", "GetSpecialDays", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid hämtning av specialdagar.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);

            }
            return View("EditSpecialDays", model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateFAQCategory(AdmFAQKategori faqCategory)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userName = User.Identity.GetUserName();
                    _portalSosService.UppdateraFAQKategori(faqCategory, userName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorManager.WriteToErrorLog("SystemController", "UpdateFAQCategory", e.ToString(), e.HResult,
                        User.Identity.Name);
                    var errorModel = new CustomErrorPageModel
                    {
                        Information = "Ett fel inträffade vid uppdatering av FAQ-kategori.",
                        ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                    };
                    return View("CustomError", errorModel);

                }
            }
            return RedirectToAction("GetFAQCategories");
        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateFAQ(SystemViewModels.SystemViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userName = User.Identity.GetUserName();
                    var faqDb = ConvertViewModelToAdmFAQ(model.SelectedFAQ);
                    _portalSosService.UppdateraFAQ(faqDb, userName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorManager.WriteToErrorLog("SystemController", "UpdateFAQ", e.ToString(), e.HResult,
                        User.Identity.Name);
                    var errorModel = new CustomErrorPageModel
                    {
                        Information = "Ett fel inträffade vid uppdatering av FAQ.",
                        ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                    };
                    return View("CustomError", errorModel);
                }
            }
            return RedirectToAction("GetFAQs", new { faqCatId = model.SelectedFAQ.FAQkategoriId });
        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateHoliday(SystemViewModels.AdmHelgdagViewModel holiday)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userName = User.Identity.GetUserName();
                    var holidayDb = ConvertViewModelToAdmHelgdag(holiday);
                    _portalSosService.UppdateraHelgdag(holidayDb, userName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorManager.WriteToErrorLog("SystemController", "UpdateHoliday", e.ToString(), e.HResult,
                        User.Identity.Name);
                    var errorModel = new CustomErrorPageModel
                    {
                        Information = "Ett fel inträffade vid uppdatering av helgdagsinformation.",
                        ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                    };
                    return View("CustomError", errorModel);
                }
            }
            return RedirectToAction("GetHolidays");
        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateSpecialDay(SystemViewModels.AdmSpecialdagViewModel specialDay)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userName = User.Identity.GetUserName();
                    var specialDayDb = ConvertViewModelToAdmSpecialdag(specialDay);
                    _portalSosService.UppdateraSpecialdag(specialDayDb, userName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorManager.WriteToErrorLog("SystemController", "UpdateSpecialDay", e.ToString(), e.HResult,
                        User.Identity.Name);
                    var errorModel = new CustomErrorPageModel
                    {
                        Information = "Ett fel inträffade vid uppdatering av specialdagsinformation.",
                        ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                    };
                    return View("CustomError", errorModel);
                }
            }
            return RedirectToAction("GetSpecialDays");
        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateInfoText(SystemViewModels.SystemViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userName = User.Identity.GetUserName();

                    AdmInformation info = new AdmInformation
                    {
                        Id = model.SelectedInfoId,
                        Text = model.SelectedInfoText
                    };
                    _portalSosService.UppdateraInformationstext(info, userName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorManager.WriteToErrorLog("SystemController", "UpdateInfoText", e.ToString(), e.HResult,
                        User.Identity.Name);
                    var errorModel = new CustomErrorPageModel
                    {
                        Information = "Ett fel inträffade vid uppdatering av informationstext.",
                        ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                    };
                    return View("CustomError", errorModel);
                }
            }
            return RedirectToAction("GetInformationTexts");
        }

        // GET
        [Authorize]
        public ActionResult CreateFAQCategory()
        {
            return View();
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult CreateFAQCategory(AdmFAQKategori faqCategory)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userName = User.Identity.GetUserName();
                    _portalSosService.SkapaFAQKategori(faqCategory, userName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorManager.WriteToErrorLog("SystemController", "CreateFAQCategory", e.ToString(), e.HResult, User.Identity.Name);
                    var errorModel = new CustomErrorPageModel
                    {
                        Information = "Ett fel inträffade när ny FAQ-kategori skulle sparas.",
                        ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                    };
                    return View("CustomError", errorModel);
                }
                return RedirectToAction("GetFAQCategories");
            }

            return View();
        }


        // GET
        [Authorize]
        public ActionResult CreateFAQ(int catId = 0)
        {
            var model = new SystemViewModels.FAQViewModel();
            model.FAQkategoriId = catId;
            // Ladda drop down lists. 
            var registerList = _portalSosService.HamtaAllaRegisterForPortalen();
            this.ViewBag.RegisterList = CreateRegisterDropDownList(registerList);
            model.SelectedRegisterId = 0;
            return View(model);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult CreateFAQ(SystemViewModels.FAQViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userName = User.Identity.GetUserName();

                    AdmFAQ faq = new AdmFAQ
                    {
                        FAQkategoriId = model.FAQkategoriId,
                        RegisterId = model.SelectedRegisterId,
                        Fraga = model.Fraga,
                        Svar = model.Svar,
                        Sortering = model.Sortering
                    };

                    if (model.SelectedRegisterId == 0)
                    {
                        faq.RegisterId = null;
                    }
                    _portalSosService.SkapaFAQ(faq, userName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorManager.WriteToErrorLog("SystemController", "CreateFAQ", e.ToString(), e.HResult, User.Identity.Name);
                    var errorModel = new CustomErrorPageModel
                    {
                        Information = "Ett fel inträffade när ny faq skulle sparas.",
                        ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                    };
                    return View("CustomError", errorModel);
                }
                return RedirectToAction("GetFAQs", new { faqCatId = model.FAQkategoriId });
            }

            return View();
        }

        // GET
        [Authorize]
        public ActionResult CreateHoliday()
        {
            var model = new SystemViewModels.AdmHelgdagViewModel();
            // Ladda drop down lists. 
            var informationList = _portalSosService.HamtaInformationstexter();
            this.ViewBag.InformationTextList = CreateInformationTextDropDownList(informationList);
            model.SelectedInformationId = 0;
            return View(model);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult CreateHoliday(SystemViewModels.AdmHelgdagViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userName = User.Identity.GetUserName();

                    AdmHelgdag holiday = new AdmHelgdag()
                    {
                        InformationsId = model.SelectedInformationId,
                        Helgdatum = model.Helgdatum,
                        Helgdag = model.Helgdag
                    };
                    _portalSosService.SkapaHelgdag(holiday, userName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorManager.WriteToErrorLog("SystemController", "CreateHoliday", e.ToString(), e.HResult, User.Identity.Name);
                    var errorModel = new CustomErrorPageModel
                    {
                        Information = "Ett fel inträffade när ny helgdag skulle läggas till.",
                        ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                    };
                    return View("CustomError", errorModel);
                }
                return RedirectToAction("GetHolidays");
            }

            return View();
        }

        // GET
        [Authorize]
        public ActionResult CreateSpecialDay()
        {
            var model = new SystemViewModels.AdmSpecialdagViewModel();
            // Ladda drop down lists. 
            var informationList = _portalSosService.HamtaInformationstexter();
            this.ViewBag.InformationTextList = CreateInformationTextDropDownList(informationList);
            model.SelectedInformationId = 0;
            return View(model);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult CreateSpecialDay(SystemViewModels.AdmSpecialdagViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userName = User.Identity.GetUserName();

                    AdmSpecialdag specialDay = new AdmSpecialdag()
                    {
                        InformationsId = model.SelectedInformationId,
                        Specialdagdatum = model.Specialdagdatum,
                        Oppna = model.Oppna,
                        Stang = model.Stang,
                        Anledning = model.Anledning
                    };
                    _portalSosService.SkapaSpecialdag(specialDay, userName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorManager.WriteToErrorLog("SystemController", "CreateSpecialDay", e.ToString(), e.HResult, User.Identity.Name);
                    var errorModel = new CustomErrorPageModel
                    {
                        Information = "Ett fel inträffade när ny specialdag skulle läggas till.",
                        ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                    };
                    return View("CustomError", errorModel);
                }
                return RedirectToAction("GetSpecialDays");
            }

            return View();
        }


        // GET
        [Authorize]
        public ActionResult EditSelectedFAQ(int faqId = 0)
        {
            var model = new SystemViewModels.SystemViewModel();
            model.SelectedFAQ = new SystemViewModels.FAQViewModel();
            var selectedFAQDb = _portalSosService.HamtaFAQ(faqId);
            model.SelectedFAQ.FAQkategoriId = selectedFAQDb.FAQkategoriId;
            model.SelectedFAQ.Id = selectedFAQDb.Id;
            model.SelectedFAQ.Fraga = selectedFAQDb.Fraga;
            model.SelectedFAQ.Svar = selectedFAQDb.Svar;
            model.SelectedFAQ.Sortering = selectedFAQDb.Sortering;

            if (selectedFAQDb.RegisterId != null)
            {
                model.SelectedFAQ.RegisterId = selectedFAQDb.RegisterId;
            }

            if (model.SelectedFAQ.RegisterId != null)
            {
                model.SelectedFAQ.SelectedRegisterId = model.SelectedFAQ.RegisterId;
            }
            else
            {
                model.SelectedFAQ.SelectedRegisterId = 0;
            }
            // Ladda drop down lists. 
            var registerList = _portalSosService.HamtaAllaRegisterForPortalen();
            this.ViewBag.RegisterList = CreateRegisterDropDownList(registerList);

            
            return View("_EditSelectedFAQ", model);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult EditSelectedFAQ(SystemViewModels.SystemViewModel model)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var userName = User.Identity.GetUserName();

                    AdmFAQ faq = new AdmFAQ
                    {
                        Id = model.SelectedFAQ.Id,
                        FAQkategoriId = model.SelectedFAQ.FAQkategoriId,
                        RegisterId = model.SelectedFAQ.RegisterId,
                        Fraga = model.SelectedFAQ.Fraga,
                        Svar = model.SelectedFAQ.Svar,
                        Sortering = model.SelectedFAQ.Sortering

                    };
                    _portalSosService.UppdateraFAQ(faq, userName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorManager.WriteToErrorLog("SystemController", "EditSelectedFAQ", e.ToString(), e.HResult, User.Identity.Name);
                    var errorModel = new CustomErrorPageModel
                    {
                        Information = "Ett fel inträffade när faq skulle sparas.",
                        ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                    };
                    return View("CustomError", errorModel);
                }
                return RedirectToAction("GetFAQs", new { faqCatId = model.SelectedFAQ.FAQkategoriId });
            }

            return RedirectToAction("GetFAQs", new {faqCatId = model.SelectedFAQ.FAQkategoriId });
        }


        // GET
        [Authorize]
        public ActionResult CreateInformationText()
        {
            return View();
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult CreateInformationText(SystemViewModels.InfoTextViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userName = User.Identity.GetUserName();
                    var infoText = new AdmInformation
                    {
                       Informationstyp = model.Informationstyp,
                       Text =  model.Text,
                    };
                    infoText.Text = model.Text;
                    _portalSosService.SkapaInformationsText(infoText, userName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorManager.WriteToErrorLog("SystemController", "CreateInfoText", e.ToString(), e.HResult, User.Identity.Name);
                    var errorModel = new CustomErrorPageModel
                    {
                        Information = "Ett fel inträffade när ny informationstext skulle sparas.",
                        ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                    };
                    return View("CustomError", errorModel);
                }
                return RedirectToAction("GetInformationTexts");
            }

            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult SaveOpeningHoursInfo(SystemViewModels.OpeningHours openHours )
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userName = User.Identity.GetUserName();

                    OpeningHoursInfoDTO openHoursDTO = new OpeningHoursInfoDTO();

                    openHoursDTO = SetHoursAndMinutes(openHours);
                    openHoursDTO.ClosedAnyway = openHours.ClosedAnyway;
                    openHoursDTO.InfoTextForClosedPage = openHours.InfoTextForClosedPage;

                    //Days
                    var daysListDTO = new List<string>();
                    foreach (var day in openHours.ClosedDaysList)
                    {
                        if (day.Selected)
                            daysListDTO.Add(day.NameEnglish);
                    }
                    openHoursDTO.ClosedDays = daysListDTO;

                    _portalSosService.SparaOppettider(openHoursDTO, userName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    ErrorManager.WriteToErrorLog("SystemController", "SaveOpeningHoursInfo", e.ToString(), e.HResult, User.Identity.Name);
                    var errorModel = new CustomErrorPageModel
                    {
                        Information = "Ett fel inträffade när information om öppettider skulle sparas.",
                        ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                    };
                    return View("CustomError", errorModel);
                }
                return RedirectToAction("GetOpeningHours");
            }

            //var x = Server.HtmlEncode(openHours.InfoTextForClosedPage);
            return RedirectToAction("GetOpeningHours");
        }

        [HttpPost]
        [Authorize]
        public ActionResult DeleteFAQCategory(int faqCatId)
        {
            try
            {
               _portalSosService.TaBortFAQKategori(faqCatId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("SystemController", "DeleteFAQCategory", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade när FAQ-kategori skulle tas bort.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
            return RedirectToAction("GetFAQCategories");
        }

        [HttpPost]
        [Authorize]
        public ActionResult DeleteFAQ(int faqId, int faqCatId)
        {
            try
            {
                _portalSosService.TaBortFAQ(faqId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("SystemController", "DeleteFAQ", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade när FAQ skulle tas bort.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
            return RedirectToAction("GetFAQs", new { faqCatId = faqCatId });
        }

        [HttpPost]
        [Authorize]
        public ActionResult DeleteHoliday(int holidayId)
        {
            try
            {
                _portalSosService.TaBortHelgdag(holidayId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("SystemController", "DeleteHoliday", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade när helgdagsinformation skulle tas bort.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
            return RedirectToAction("GetHolidays");
        }

        [HttpPost]
        [Authorize]
        public ActionResult DeleteSpecialDay(int specialDayId)
        {
            try
            {
                _portalSosService.TaBortSpecialdag(specialDayId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("SystemController", "DeleteSpecialDay", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade när information om specialdag skulle tas bort.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                return View("CustomError", errorModel);
            }
            return RedirectToAction("GetSpecialDays");
        }


        private DateTime SetTime(int hour, int minute)
        {
            DateTime time = new DateTime();

            var newDate = time.Date + new TimeSpan(hour, minute, 00);

            return newDate;
        }

        private OpeningHoursInfoDTO SetHoursAndMinutes(SystemViewModels.OpeningHours openingHours)
        {
            var openingHoursDTO = new OpeningHoursInfoDTO();

            string[] openFromSplit = openingHours.OpeningTimeStr.Split(':');
            openingHoursDTO.ClosedToHour = openFromSplit[0];
            openingHoursDTO.ClosedToMin = openFromSplit[1];

            string[] openToSplit = openingHours.ClosingTimeStr.Split(':');
            openingHoursDTO.ClosedFromHour = openToSplit[0];
            openingHoursDTO.ClosedFromMin = openToSplit[1];

            return openingHoursDTO;
        }

        /// <summary>  
        /// Create list for register-dropdown  
        /// </summary>  
        /// <returns>Return register for drop down list.</returns>  
        private IEnumerable<SelectListItem> CreateRegisterDropDownList(IEnumerable<AdmRegister> registerInfoList)
        {
            SelectList lstobj = null;

            var list = registerInfoList
                .Select(p =>
                    new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.Kortnamn
                    });

            // Setting.  
            lstobj = new SelectList(list, "Value", "Text");

            return lstobj;
        }

        /// <summary>  
        /// Create list for infotext-dropdown  
        /// </summary>  
        /// <returns>Return infotext-types for drop down list.</returns>  
        private IEnumerable<SelectListItem> CreateInformationTextDropDownList(IEnumerable<AdmInformation> informationList)
        {
            SelectList lstobj = null;

            var list = informationList
                .Select(p =>
                    new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.Informationstyp
                    });

            // Setting.  
            lstobj = new SelectList(list, "Value", "Text");

            return lstobj;
        }

        private List<SystemViewModels.AdmHelgdagViewModel> ConvertAdmHelgdagToViewModel(List<AdmHelgdag> holidayList)
        {
            var holidayViewList = new List<SystemViewModels.AdmHelgdagViewModel>();
            foreach (var holiday in holidayList)
            {
                var holidayView = new SystemViewModels.AdmHelgdagViewModel()
                {
                    Id = holiday.Id,
                    Helgdatum = holiday.Helgdatum,
                    Helgdag = holiday.Helgdag,
                    Informationstyp = _portalSosService.HamtaInfo(holiday.InformationsId).Informationstyp
                };

                holidayViewList.Add(holidayView);
            }
            return holidayViewList;
        }

        private List<SystemViewModels.AdmSpecialdagViewModel> ConvertAdmSpecialdagToViewModel(List<AdmSpecialdag> specialdagList)
        {
            var specialdayViewList = new List<SystemViewModels.AdmSpecialdagViewModel>();
            foreach (var specialdag in specialdagList)
            {
                var specialdayView = new SystemViewModels.AdmSpecialdagViewModel()
                {
                    Id = specialdag.Id,
                    Specialdagdatum = specialdag.Specialdagdatum,
                    Oppna = specialdag.Oppna,
                    Stang = specialdag.Stang,
                    Anledning = specialdag.Anledning,
                    Informationstyp = _portalSosService.HamtaInfo(specialdag.InformationsId).Informationstyp
                };

                specialdayViewList.Add(specialdayView);
            }
            return specialdayViewList;
        }

        private List<SystemViewModels.FAQViewModel> ConvertAdmFAQToViewModel(List<AdmFAQ> faqList)
        {
            var faqViewList = new List<SystemViewModels.FAQViewModel>();
            foreach (var faq in faqList)
            {
                var faqView = new SystemViewModels.FAQViewModel
                {
                    Id = faq.Id,
                    RegisterId = faq.RegisterId,
                    SelectedRegisterId = faq.RegisterId,
                    FAQkategoriId = faq.FAQkategoriId,
                    Fraga = faq.Fraga,
                    Svar = faq.Svar,
                    Sortering = faq.Sortering
                };

                if (faq.RegisterId != null )
                {
                    var id = Convert.ToInt32(faq.RegisterId);
                    faqView.RegisterKortNamn = _portalSosService.HamtaKortnamnForRegister(id);
                }

                faqViewList.Add(faqView);
            }
            return faqViewList;
        }

        private AdmFAQ ConvertViewModelToAdmFAQ(SystemViewModels.FAQViewModel faq)
        {
            var faqDb = new AdmFAQ
            {
                Id = faq.Id,
                FAQkategoriId = faq.FAQkategoriId,
                Fraga = faq.Fraga,
                Svar = faq.Svar,
                Sortering = faq.Sortering
            };

            if (faq.SelectedRegisterId == 0)
            {
                faqDb.RegisterId = null;
            }
            else
            {
                faqDb.RegisterId = faq.SelectedRegisterId;
            }

            return faqDb;
        }

        private AdmHelgdag ConvertViewModelToAdmHelgdag(SystemViewModels.AdmHelgdagViewModel holiday)
        {
            var holidayDb = new AdmHelgdag()
            {
                Id = holiday.Id,
                InformationsId = _portalSosService.HamtaInfoText(holiday.Informationstyp).Id,
                Helgdatum = holiday.Helgdatum,
                Helgdag = holiday.Helgdag
            };
            return holidayDb;
        }

        private AdmSpecialdag ConvertViewModelToAdmSpecialdag(SystemViewModels.AdmSpecialdagViewModel specialDay)
        {
            var specialDayDb = new AdmSpecialdag()
            {
                Id = specialDay.Id,
                InformationsId = _portalSosService.HamtaInfoText(specialDay.Informationstyp).Id,
                Specialdagdatum = specialDay.Specialdagdatum,
                Oppna = specialDay.Oppna,
                Stang = specialDay.Stang,
                Anledning = specialDay.Anledning,
            };
            return specialDayDb;
        }
    }
}