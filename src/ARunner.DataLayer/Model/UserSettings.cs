using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ARunner.DataLayer.Model
{
    public enum Units
    {
        Metric,
        Imperial
    }

    public class UserSettings
    {
        public string Language { get; set; }

        public Units Units { get; set; }
        public bool TemporaryPassword { get; set; }
    }
}
