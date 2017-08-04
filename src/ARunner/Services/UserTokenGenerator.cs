using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ARunner.DataLayer.Model;
using Microsoft.AspNetCore.Identity;

namespace ARunner.Services
{
    public class UserTokenGenerator : IUserTokenGenerator
    {
        public string GenerateToken(User user, string reason)
        {
            return WebUtility.UrlEncode(user.Id + "-" + user.SecurityStamp + "-" + reason.GetHashCode());
        }

        public bool ValidateToken(User user, string reason, string token)
        {
            return GenerateToken(user, reason) == token;
        }
    }
}
