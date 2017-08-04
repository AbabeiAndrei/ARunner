using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ARunner.DataLayer.Initial;
using Microsoft.Extensions.Logging;

namespace ARunner
{
    public partial class Startup
    {
        public async Task RunSeeder(IContextSeed seeder)
        {
            try
            {
                //await seeder.ApplyMigrations();
                await seeder.ApplicationSeed();
            }
            catch (Exception mex)
            {
                throw;
            }
        }
    }
}
