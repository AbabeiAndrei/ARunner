using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace ARunner
{
    public partial class Startup
    {
        private static void ConfigureJsonOptions(MvcJsonOptions options)
        {
            options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            options.SerializerSettings.Converters.Add(new StringEnumConverter { CamelCaseText = true });
        }

        private void ConfigureIdentity(IdentityOptions options)
        {
            if (_environment.IsDevelopment())
            {
                options.User.RequireUniqueEmail = false;
                options.Password.RequiredLength = 2;
                options.SignIn.RequireConfirmedEmail = true;
            }
            else
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
                options.SignIn.RequireConfirmedEmail = true;
            }

            options.Cookies.ApplicationCookie.LoginPath = "/Account/Login";

        }
    }
}
