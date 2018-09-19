using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(InrappSos.AstridWeb.Startup))]
namespace InrappSos.AstridWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
