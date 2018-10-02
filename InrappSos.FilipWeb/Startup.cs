using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(InrappSos.FilipWeb.Startup))]
namespace InrappSos.FilipWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
