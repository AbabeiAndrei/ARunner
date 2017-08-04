using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace ARunner
{
    public partial class Startup
    {
        private static void ConfigureRoutes(IRouteBuilder builder)
        {
            builder.MapRoute("Account",
                             "{controller}/{action}/{id?}",
                             new { controller = "Jogging", action = "Home" });
        }
    }
}
