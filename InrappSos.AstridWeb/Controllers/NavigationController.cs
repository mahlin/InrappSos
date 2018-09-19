using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace InrappSos.AstridWeb.Controllers
{
    public class NavigationController : Controller
    {
        private ApplicationRoleManager _roleManager;
        private ApplicationUserManager _userManager;

        public NavigationController( ApplicationRoleManager roleManager, ApplicationUserManager userManager)
        {
            RoleManager = roleManager;
            UserManager = userManager;
        }

        public NavigationController()
        {

        }

        [ChildActionOnly]
        public ActionResult Menu()
        {
            //TODO - tillfälligt/istf Roles
            var user = UserManager.FindById(User.Identity.GetUserId());

            if (user != null)
            {
                if (UserManager.IsInRole(user.Id, "Admin"))
                {
                    return PartialView("_LoginAdmin");
                }
                else
                {
                    return PartialView("_LoginPartial");
                }
            }

            //if (Roles.IsUserInRole("Admin"))
            //{
            //    return PartialView("_LoginAdmin");
            //}

            return PartialView("_LoginPartial");
        }


        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? Request.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            }
            private set { _roleManager = value; }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
    }
}