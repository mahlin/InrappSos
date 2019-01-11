using System.Linq;
using System.Web;
using System.Web.Mvc;
using InrappSos.ApplicationService;
using InrappSos.ApplicationService.Interface;
using InrappSos.DataAccess;
using InrappSos.DomainModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;


namespace InrappSos.AstridWeb.Filters
{
    public class HasPermissionAttribute : ActionFilterAttribute
    {
        private string _permission;
        private readonly IPortalSosService _portalSosService;

        public HasPermissionAttribute(string permission)
        {
            _permission = permission;
            _portalSosService = new PortalSosService(new PortalSosRepository(new InrappSosDbContext(), new InrappSosAstridDbContext()));
        }

        //public HasPermissionAttribute(ApplicationUserManager userManager)
        //{
        //    _userManager = userManager;
        //    _portalSosService = new PortalSosService(new PortalSosRepository(new InrappSosDbContext(), new InrappSosAstridDbContext()));
        //}



        //public ApplicationUserManager UserManager
        //{
        //    get
        //    {
        //        return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
        //    }
        //    private set
        //    {
        //        _userManager = value;
        //    }
        //}

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var hasPermission = false;
            var user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var roles = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().GetRoles(user.Id);
            foreach (var roleName in roles)
            {
                var role = _portalSosService.HamtaAstridRoll(roleName);
                var rolePermissionsNamesList = _portalSosService.HamtaAstridRattighetersNamnForRoll(role.Id);
                if (rolePermissionsNamesList.Contains(_permission))
                {
                    hasPermission = true;
                }
            }

            if (!hasPermission)
            {
                // If this user does not have the required permission then redirect to login page
                var url = new UrlHelper(filterContext.RequestContext);
                var loginUrl = url.Content("~/Account/Login");
                filterContext.HttpContext.Response.Redirect(loginUrl, true);
            }
        }
    }
}