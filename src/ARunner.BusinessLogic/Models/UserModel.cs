using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using ARunner.DataLayer;
using ARunner.DataLayer.Model;
using Newtonsoft.Json;

namespace ARunner.BusinessLogic.Models
{
    public class UserModel : UserBaseModel
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public UserState State { get; set; }

        public UserAccess Access { get; set; }

        public string Metadata { get; set; }
        
        public UserSettings Settings => Metadata != null ? JsonConvert.DeserializeObject<UserSettings>(Metadata) : null;
    }
}
