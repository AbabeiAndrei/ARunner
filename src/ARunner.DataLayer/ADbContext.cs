using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ARunner.DataLayer.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MySQL.Data.EntityFrameworkCore.Extensions;

namespace ARunner.DataLayer
{
    public enum RowState : short
    {
        Created = 0,
        Deleted = 1
    }

    public class ADbContext : IdentityDbContext<User>
    {
        private readonly IConfigurationRoot _config;

        public ADbContext(IConfigurationRoot config, DbContextOptions options)
            : base(options)
        {
            _config = config;
        }
        
        public DbSet<Jogging> Joggings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder buileBuilder)
        {
            base.OnConfiguring(buileBuilder);

            buileBuilder.UseMySQL(_config["connectionString"], builder => builder.MigrationsAssembly("ARunner"));
        }
    }
}
