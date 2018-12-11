using System;
using InrappSos.DomainModel;
using InrappSos.DataAccess;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

namespace InrappSos.AstridWeb
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit https://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(InrappSosAstridDbContext.Create);
            app.CreatePerOwinContext(InrappSosDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<FilipApplicationUserManager>(FilipApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);
            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);
            app.CreatePerOwinContext<FilipApplicationRoleManager>(FilipApplicationRoleManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity =
                        SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, AppUserAdmin>(
                            validateInterval: TimeSpan.FromMinutes(30),
                            regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            //createRolesandUsers();
        }

        // In this method we will create default User roles and Admin user for login   
        //private void createRolesandUsers()
        //{
        //    IdentityDbContext context = new IdentityDbContext();

        //    var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
        //    var UserManager = new UserManager<AppUserAdmin>(new UserStore<AppUserAdmin>(context));


        //    // In Startup iam creating first Admin Role and creating a default Admin User    
        //    if (!roleManager.RoleExists("Admin"))
        //    {
        //        // first we create Admin role   
        //        var role = new IdentityRole();
        //        role.Name = "Admin";
        //        roleManager.Create(role);

        //        //Here we create a Admin super user who will maintain the website              
        //        //var user = new AppUserAdmin();
        //        //user.UserName = "findus";
        //        //user.Email = "findus@gmail.com";
        //        //string userPWD = "Findus2018!";
        //        //var chkUser = UserManager.Create(user, userPWD);

        //        var user = UserManager.FindByEmail("marie.ahlin@knivsta.se");
        //        //Add default User to Role Admin  
        //        var result1 = UserManager.AddToRole(user.Id, "Admin");

        //    }

        //    // creating Creating Manager role    
        //    if (!roleManager.RoleExists("Manager"))
        //    {
        //        var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
        //        role.Name = "Manager";
        //        roleManager.Create(role);

        //    }

        //    // creating Creating Employee role    
        //    if (!roleManager.RoleExists("Employee"))
        //    {
        //        var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
        //        role.Name = "Employee";
        //        roleManager.Create(role);

        //    }
        //}
    }
}