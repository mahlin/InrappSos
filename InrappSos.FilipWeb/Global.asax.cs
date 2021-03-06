﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using InrappSos.ApplicationService.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace InrappSos.FilipWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        protected void Application_Error()
        {
            var ex = Server.GetLastError();
            //log the error!
            ErrorManager.WriteToErrorLog("", "", ex.Message, ex.HResult);

            HttpContext ctx = HttpContext.Current;
            KeyValuePair<string, object> error = new KeyValuePair<string, object>("ErrorMessage", ctx.Server.GetLastError().ToString());
            ctx.Response.Clear();
            RequestContext rc = ((MvcHandler)ctx.CurrentHandler).RequestContext;
            string controllerName = rc.RouteData.GetRequiredString("controller");
            IControllerFactory factory = ControllerBuilder.Current.GetControllerFactory();
            IController controller = factory.CreateController(rc, controllerName);
            ControllerContext cc = new ControllerContext(rc, (ControllerBase)controller);

            ViewResult viewResult = new ViewResult { ViewName = "Error" };
            viewResult.ViewData.Add(error);
            viewResult.ExecuteResult(cc);
            ctx.Server.ClearError();
            //ctx.Response.End();

        }

        protected void Session_start()
        {
            // starting a session and already authenticated means we have an old cookie
            var existingUser = System.Web.HttpContext.Current.User;
            if (existingUser != null && existingUser.Identity.Name != "")
            {
                // clear any existing cookies
                IAuthenticationManager authMgr = System.Web.HttpContext.Current.GetOwinContext().Authentication;
                authMgr.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

                // manually clear user from HttpContext so Authorize attr works
                HttpContext.Current.User = new ClaimsPrincipal(new ClaimsIdentity());
            }

        }

        //Prevent direct URL access: Call [NoDirectAccess] to all controllers to block
        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
        public class NoDirectAccessAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                if (filterContext.HttpContext.Request.UrlReferrer == null ||
                    filterContext.HttpContext.Request.Url.Host != filterContext.HttpContext.Request.UrlReferrer.Host)
                {
                    filterContext.Result = new RedirectToRouteResult(new
                        RouteValueDictionary(new { controller = "Home", action = "Index", area = "" }));
                }
            }
        }
    }
}
