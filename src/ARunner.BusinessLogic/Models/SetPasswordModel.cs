using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ARunner.BusinessLogic.Models
{
    public class SetPasswordModel
    {
        public string Token { get; set; }

        public string Password { get; set; }
    }
}
