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
            var searchResultLits = new List<List<SearchViewModels.SearchResult>>();

            try
            {
                //Sök på Organisationer
                var searchResOrg = _portalSosService.SokCaseOrganisation(searchText);
                var searchResOrgVM = ConvertSearchResOrgToVM(searchResOrg);

                //Sök på Ärenden
                var searchResCases = _portalSosService.SokArende(searchText);
                var searchResCasesVM = ConvertSearchResCasesToVM(searchResCases);

                searchResultLits.AddRange(searchResOrgVM);
                searchResultLits.AddRange(searchResCasesVM);
                model.SearchResultList = searchResultLits;
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


        private List<List<SearchViewModels.SearchResult>> ConvertSearchResOrgToVM(List<List<Organisation>> searchResOrg)
        {
            var searchResultList = new List<List<SearchViewModels.SearchResult>>();
            var searchResList = new List<SearchViewModels.SearchResult>();
            foreach (var orgList in searchResOrg)
            {
                foreach (var org in orgList)
                {
                    var searchRes = new SearchViewModels.SearchResult()
                    {
                        Origin = "",
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

        private List<List<SearchViewModels.SearchResult>> ConvertSearchResCasesToVM(List<List<Arende>> searchResCases)
        {
            var searchResultList = new List<List<SearchViewModels.SearchResult>>();
            var searchResList = new List<SearchViewModels.SearchResult>();
            foreach (var caseList in searchResCases)
            {
                foreach (var arende in caseList)
                {
                    var searchRes = new SearchViewModels.SearchResult()
                    {
                        Origin = "",
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
    }
}