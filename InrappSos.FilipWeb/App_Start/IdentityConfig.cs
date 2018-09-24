using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using InrappSos.DataAccess;
using InrappSos.DomainModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using InrappSos.FilipWeb.Models;

namespace InrappSos.FilipWeb
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            SendMail(message);
            return Task.FromResult(0);
        }

        private void SendMail(IdentityMessage message)
        {
            #region formatter
            string text = string.Format("Vänligen klicka på den här länken för att bekäfta ditt konto:  {1}", message.Subject, message.Body);
            //string html = "Vänligen bekräfta ditt konto i Socialstyrelsens inrapporteringsportal genom att klicka på den här länken: <a href=" + message.Body + ">Bekräfta e-postadress</a><br/>";
            string html = message.Body;

            #endregion

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress(ConfigurationManager.AppSettings["MailSender"]);
            //TODO
            //msg.To.Add(new MailAddress("marie.ahlin@socialstyrelsen.se"));
            msg.To.Add(new MailAddress(message.Destination));
            msg.Subject = message.Subject;
            msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
            msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));

            //var credentials = new NetworkCredential(
            //    ConfigurationManager.AppSettings["MailSender"],
            //    ConfigurationManager.AppSettings["MailPwd"]);

            using (SmtpClient smtpClient = new SmtpClient(ConfigurationManager.AppSettings["MailServer"]))
            {
                if (ConfigurationManager.AppSettings["EnableSsl"] == "True")
                {
                    smtpClient.EnableSsl = true;
                }
                smtpClient.Send(msg);
            }

            //SmtpClient smtpClient = new SmtpClient(ConfigurationManager.AppSettings["MailServer"]);
            ////smtpClient.Credentials = credentials;
            //if (ConfigurationManager.AppSettings["EnableSsl"] == "True")
            //{
            //    smtpClient.EnableSsl = true;
            //}
            //smtpClient.Send(msg);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            var usr = WebConfigurationManager.AppSettings["SMSUser"];
            var pwd = WebConfigurationManager.AppSettings["SMSPwd"];
            var sender = WebConfigurationManager.AppSettings["SMSSender"];
            var proxy = WebConfigurationManager.AppSettings["Proxy"];

            HttpWebRequest request = WebRequest.Create("https://api.smsteknik.se/send/?id=Socialstyrelsen&user=" + usr + "&pass=" + pwd + "&nr=" + message.Destination + "&sender=" + sender + "&msg=" + message.Body) as HttpWebRequest;

            //TODO
            WebProxy proxyObject = new WebProxy(proxy);
            WebRequest.DefaultWebProxy = proxyObject;

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            response.Close();
        }
    }  

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<InrappSosDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };


            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 4,
                RequireNonLetterOrDigit = false,
                RequireDigit = true,
                RequireLowercase = false,
                RequireUppercase = false,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Välkommen till Socialstyrelsens InrappSos. För att logga in ange följande verifieringskod på webbsidan: {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Verifieringskod",
                BodyFormat = "Din verifieringskod är {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = 
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}
