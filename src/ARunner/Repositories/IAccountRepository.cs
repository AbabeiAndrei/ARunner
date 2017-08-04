using System.Collections.Generic;
using System.Linq;
using ARunner.DataLayer.Model;

namespace ARunner.Repositories
{
    public interface IAccountRepository
    {
        IQueryable<User> GetUsers();
        void Add(User entity);
        void Save(User entity);
        void Delete(User user);
    }
}