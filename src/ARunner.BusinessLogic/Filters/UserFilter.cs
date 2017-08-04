using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ARunner.BusinessLogic.Models;
using ARunner.DataLayer.Model;

namespace ARunner.BusinessLogic.Filters
{
    public class UserFilter : EntityMapper<User, UserModel>
    {
        public string Name { get; set; }

        public override PaggingCollection<UserModel> ApplyFilter(IQueryable<User> source, Func<User, UserModel> mapper)
        {
            if (!string.IsNullOrEmpty(Name))
                source = source.Where(u => u.FullName.Contains(Name) || u.Email.Contains(Name) || u.UserName.Contains(Name));

            return new PaggingCollection<UserModel>(source.ToList().Select(mapper), this);
        }
    }
}
