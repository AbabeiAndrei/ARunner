using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ARunner.DataLayer.Model
{
    public enum UserAccess : short
    {
        Regular = 0,
        Manager = 1,
        Admin = 2    
    }

    public enum UserState : short
    {
        Created = 0,
        Invited = 1,
        Active = 2,
        Suspended = 3
    }

    public sealed class User : IdentityUser
    {
        public User()
        {

        }

        public User(string userName) : base(userName)
        {

        }

        [Required]
        public string FullName { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime LastLoggedIn { get; set; }

        [Required]
        public UserState State { get; set; }

        [Required]
        public UserAccess Access { get; set; }

        [Required]
        public RowState RowState { get; set; }

        public string Metadata { get; set; }

        [NotMapped]
        public UserSettings Settings => Metadata != null ? JsonConvert.DeserializeObject<UserSettings>(Metadata) : null;
    }
}
