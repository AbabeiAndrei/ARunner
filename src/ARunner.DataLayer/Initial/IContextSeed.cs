using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ARunner.DataLayer.Initial
{
    public interface IContextSeed
    {
        Task ApplicationSeed();
        Task ApplyMigrations();
    }
}
