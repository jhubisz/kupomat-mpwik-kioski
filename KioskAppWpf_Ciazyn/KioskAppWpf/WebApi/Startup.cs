using Owin;
using System.Web.Http;

namespace KioskAppWpf.WebApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            HttpConfiguration config = new HttpConfiguration();

            // Enable attribute routing
            config.MapHttpAttributeRoutes();

            // Register default route (in case attribute routing isn't used)
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Add Web API to the OWIN pipeline
            appBuilder.UseWebApi(config);
        }
    }
}