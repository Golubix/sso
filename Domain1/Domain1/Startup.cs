using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Domain1.Startup))]
namespace Domain1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
