using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
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
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace InrappSos.AstridWeb.Controllers
{
    public class SearchController : Controller
    {
        private readonly IPortalSosService _portalSosService;
        private FilipApplicationRoleManager _filipRoleManager;
        private FilipApplicationUserManager _filipUserManager;


        public SearchController()
        {
            _portalSosService = new PortalSosService(new PortalSosRepository(new InrappSosDbContext(), new InrappSosAstridDbContext()));

        }

        public SearchController(FilipApplicationRoleManager filipRoleManager, FilipApplicationUserManager filipUserManager)
        {
            _portalSosService = new PortalSosService(new PortalSosRepository(new InrappSosDbContext(), new InrappSosAstridDbContext()));
            FilipRoleManager = filipRoleManager;
            FilipUserManager = filipUserManager;
        }

        public FilipApplicationRoleManager FilipRoleManager
        {
            get
            {
                return _filipRoleManager ?? Request.GetOwinContext().GetUserManager<FilipApplicationRoleManager>();
            }
            private set { _filipRoleManager = value; }
        }

        public FilipApplicationUserManager FilipUserManager
        {
            get
            {
                return _filipUserManager ?? HttpContext.GetOwinContext().GetUserManager<FilipApplicationUserManager>();
            }
            private set
            {
                _filipUserManager = value;
            }
        }

        // GET: Search
        public ActionResult Index()
        {
            var model = new SearchViewModels.SearchViewModel();
            model.SearchResultList = new List<List<SearchViewModels.SearchResult>>();
            return View(model);

        }


        [Authorize]
        // GET: Organisation
        public ActionResult Search(string searchText, string origin)
        {
            var model = new SearchViewModels.SearchViewModel();
            var trimmedSearchText = searchText.Trim();

            try
            {
                switch (origin)
                {
                    case "cases":
                        model.SearchResultList = SearchCases(trimmedSearchText, origin);
                        //Om endats en träff, hämta datat direkt
                        var oneHit = CheckIfOnlyOneHit(model.SearchResultList);
                        if (oneHit)
                        {
                            return RedirectToAction("GetOrganisationsCases", "Organisation",
                                new { selectedOrganisationId = model.SearchResultList[0][0].Id });
                        }
                        break;
                    case "contacts":
                        model.SearchResultList = SearchContacts(trimmedSearchText, origin);
                        //Om endats en träff, hämta datat direkt
                        if (model.SearchResultList.Count == 1 && model.SearchResultList[0].Count == 1)
                        {
                            return RedirectToAction("GetOrganisationsContacts", "Organisation",
                                new {selectedOrganisationId = model.SearchResultList[0][0].Id});
                        }
                        //Om inga träffar, tillbaka till söksidan
                        else if (model.SearchResultList.Count == 0 || (model.SearchResultList.Count == 0 && model.SearchResultList[0].Count == 0))
                        {
                            return RedirectToAction("GetContacts", "Organisation",
                                new { origin = origin });
                        }
                        break;
                    default:
                        var errorModel = new CustomErrorPageModel
                        {
                            Information = "Felaktig avsändare till sökfunktionen.",
                            ContactEmail = ConfigurationManager.AppSettings["ContactEmail"]
                        };
                        return View("CustomError", errorModel);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("SearchController", "Search", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid sökning.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                if (e.Message == "Sequence contains no elements")
                {
                    errorModel.Information = "Ingen sökträff.";
                }

                return View("CustomError", errorModel);
            }

            return View("Index", model);

        }

        private List<List<SearchViewModels.SearchResult>> SearchCases(string searchText, string origin)
        {
            var searchResultList= new List<List<SearchViewModels.SearchResult>>();

            //Sök på Organisationer
            var searchResOrg = _portalSosService.SokCaseOrganisation(searchText);
            var searchResOrgVM = ConvertSearchResOrgToVM(searchResOrg, origin);

            //Sök på Ärenden
            var searchResCases = _portalSosService.SokArende(searchText);
            var searchResCasesVM = ConvertSearchResCasesToVM(searchResCases, origin);

            searchResultList.AddRange(searchResOrgVM);
            searchResultList.AddRange(searchResCasesVM);

            return searchResultList;
        }

        private List<List<SearchViewModels.SearchResult>> SearchContacts(string searchText, string origin)
        {
            var searchResultList = new List<List<SearchViewModels.SearchResult>>();

            //Sök på Organisationer
            var searchResOrg = _portalSosService.SokOrganisation(searchText);
            var searchResOrgVM = ConvertSearchResOrgToVM(searchResOrg, origin);

            //Sök på Kontaktpersoner
            var searchResContacts = _portalSosService.SokKontaktperson(searchText);
            var searchResContactsVM = ConvertSearchResContactToVM(searchResContacts, origin);

            if (searchResOrgVM.Count == 1 && searchResOrgVM[0].Count > 0)
            {
                searchResultList.AddRange(searchResOrgVM);
            }
            if (searchResContactsVM.Count == 1 && searchResContactsVM[0].Count > 0)
            {
                searchResultList.AddRange(searchResContactsVM);
            }

            ////Om endats en träff, hämta datat direkt
            //if (searchResultList.Count == 1 && searchResultList[0].Count == 1)
            //{
            //    RedirectToAction("GetOrganisationsContacts", "Organisation",
            //        new { selectedOrganisationId = searchResultList[0][0].Id });
            //}
            //else
            //{
            //    model.SearchResultList = searchResultList;
            //}

            return searchResultList;
        }

        [Authorize]
        // GET: Contact
        public ActionResult SearchCase(string searchText, string origin)
        {
            var model = new OrganisationViewModels.OrganisationViewModel();
            var searchResultCaseList = new List<List<Arende>>();
            try
            {
                searchResultCaseList = _portalSosService.SokArende(searchText);
                model.Origin = origin;

                //Om endats en träff, hämta datat direkt
                if (searchResultCaseList.Count == 1 && searchResultCaseList[0].Count == 1)
                {
                    switch (origin)
                    {
                        case "cases":
                            return RedirectToAction("GetOrganisationsCases","Organisation", new { selectedOrganisationId = searchResultCaseList[0][0].OrganisationsId });
                        default:
                            var errorModel = new CustomErrorPageModel
                            {
                                Information = "Felaktig avsändare till sökfunktionen.",
                                ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                            };
                            return View("CustomError", errorModel);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorManager.WriteToErrorLog("SearchController", "SearchCase", e.ToString(), e.HResult, User.Identity.Name);
                var errorModel = new CustomErrorPageModel
                {
                    Information = "Ett fel inträffade vid sökning efter ärende.",
                    ContactEmail = ConfigurationManager.AppSettings["ContactEmail"],
                };
                if (e.Message == "Sequence contains no elements")
                {
                    errorModel.Information = "Inget ärende kunde hittas.";
                }

                return View("CustomError", errorModel);
            }
            //return View("Organisation","EditContacts", model);
            return RedirectToAction("EditCasesForDifferentOrganisations", "Organisation", new { searchText = searchText, origin = origin });

        }

        private void Test(List<List<SearchViewModels.SearchResult>> searchResultList, string origin)
        {
            RedirectToAction("GetOrganisationsContacts", "Organisation", new { selectedOrganisationId = searchResultList[0][0].Id });

        }


        private List<List<SearchViewModels.SearchResult>> ConvertSearchResOrgToVM(List<List<Organisation>> searchResOrg, string origin)
        {
            var searchResultList = new List<List<SearchViewModels.SearchResult>>();
            var searchResList = new List<SearchViewModels.SearchResult>();
            foreach (var orgList in searchResOrg)
            {
                foreach (var org in orgList)
                {
                    var searchRes = new SearchViewModels.SearchResult()
                    {
                        Origin = origin,
                        Name = org.Organisationsnamn,
                        DomainModelName = "Organisation",
                        Id = org.Id
                    };
                    searchResList.Add(searchRes);
                }
                searchResultList.Add(searchResList);
            }
            return searchResultList;
        }

        private List<List<SearchViewModels.SearchResult>> ConvertSearchResCasesToVM(List<List<Arende>> searchResCases, string origin)
        {
            var searchResultList = new List<List<SearchViewModels.SearchResult>>();
            var searchResList = new List<SearchViewModels.SearchResult>();
            foreach (var caseList in searchResCases)
            {
                foreach (var arende in caseList)
                {
                    var searchRes = new SearchViewModels.SearchResult()
                    {
                        Origin = origin,
                        Name = arende.Arendenr,
                        DomainModelName = "Arende",
                        Id = arende.Id
                    };
                    searchResList.Add(searchRes);
                }
                searchResultList.Add(searchResList);
            }
            return searchResultList;
        }

        private List<List<SearchViewModels.SearchResult>> ConvertSearchResContactToVM(List<List<ApplicationUser>> searchResContacts, string origin)
        {
            var searchResultList = new List<List<SearchViewModels.SearchResult>>();
            var searchResList = new List<SearchViewModels.SearchResult>();
            foreach (var contactList in searchResContacts)
            {
                foreach (var contact in contactList)
                {
                    var searchRes = new SearchViewModels.SearchResult()
                    {
                        Origin = origin,
                        Name = contact.Email,
                        DomainModelName = "ApplicationUser",
                        Id = contact.OrganisationId,
                        IdStr = contact.Id
                    };
                    searchResList.Add(searchRes);
                }
                searchResultList.Add(searchResList);
            }
            return searchResultList;
        }

        private bool CheckIfOnlyOneHit(List<List<SearchViewModels.SearchResult>> searchResult)
        {
            var hits = 0;
            foreach (var itemList in searchResult)
            {
                foreach (var hit in itemList)
                {
                    hits++;
                }
            }
            if (hits == 1)
                return true;
            return false;
        }
    }
}