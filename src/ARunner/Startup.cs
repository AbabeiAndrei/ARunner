using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ARunner.Auth;
using ARunner.BusinessLogic.Hasher;
using ARunner.DataLayer;
using ARunner.DataLayer.Initial;
using ARunner.DataLayer.Model;
using ARunner.Managers;
using ARunner.Repositories;
using ARunner.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;

namespace ARunner
{
    public partial class Startup
    {
        private readonly IHostingEnvironment _environment;
        private readonly IConfigurationRoot _config;

        public Startup(IHostingEnvironment environment)
        {
            _environment = environment;
            var builder = new ConfigurationBuilder().AddJsonFile(Path.Combine(environment.ContentRootPath, "config.json"));

            _config = builder.Build();
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(_config);

            services.AddDbContext<ADbContext>();

            var dataProtectionProviderType = typeof(DataProtectorTokenProvider<>).MakeGenericType(typeof(User));
            var emailTokenProviderType = typeof(EmailTokenProvider<>).MakeGenericType(typeof(User));
            services.AddScoped<IPasswordHasher<User>, CustomPasswordHasher>();

            services.AddIdentity<User, IdentityRole>(ConfigureIdentity)
                    .AddEntityFrameworkStores<ADbContext>()
                    .AddDefaultTokenProviders()
                    .AddTokenProvider(TokenOptions.DefaultProvider, dataProtectionProviderType)
                    .AddTokenProvider(TokenOptions.DefaultEmailProvider, emailTokenProviderType);
            
            services.AddTransient<IContextSeed, ARunnerContextSeed>();
            services.AddScoped<IJoggingsRepository, JoggingsRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IAccountManager, AccountManager>();
            services.AddScoped<IMailSender, SendGridMailService>();
            services.AddScoped<IUserTokenGenerator, UserTokenGenerator>();

            services.AddLogging();

            services.AddMvc()
                    .AddJsonOptions(ConfigureJsonOptions);

            services.AddAuthorization(ConfigureAuthorization);

        }

        private void ConfigureAuthorization(AuthorizationOptions options)
        {
            options.AddPolicy("Admin", builder => builder.Requirements.Add(new AccessRequired(UserAccess.Admin)));
            options.AddPolicy("Manager", builder => builder.Requirements.Add(new AccessRequired(UserAccess.Manager)));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IContextSeed seeder, ILoggerFactory factory)
        {
            Mapper.Initialize(ConfigureMapping);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                factory.AddDebug(LogLevel.Information);
            }
            else
                factory.AddDebug(LogLevel.Warning);

            app.UseStaticFiles();

            app.UseIdentity();

            app.UseMvc(ConfigureRoutes);

            RunSeeder(seeder).Wait();
        }
    }
}
