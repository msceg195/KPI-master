using Microsoft.Owin;
using Owin;
using System.Configuration;
using System.Web.Mvc;
[assembly: OwinStartupAttribute(typeof(KPI_APP.Startup))]
namespace KPI_APP
{
    [Authorize]
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            var minutes = int.Parse(ConfigurationManager.AppSettings["minutes"]);
            var hours = int.Parse(ConfigurationManager.AppSettings["hours"]);
            var Interval = int.Parse(ConfigurationManager.AppSettings["Interval"]);
        }
    }
}
