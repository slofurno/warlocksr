using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(warlocks.Startup))]
namespace warlocks
{

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}