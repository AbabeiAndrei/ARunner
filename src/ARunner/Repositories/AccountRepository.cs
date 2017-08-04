using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ARunner.BusinessLogic.Exceptions;
using ARunner.DataLayer;
using ARunner.DataLayer.Model;

namespace ARunner.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ADbContext _context;

        public AccountRepository(ADbContext context)
        {
            _context = context;
        }

        public IQueryable<User> GetUsers()
        {
            return _context.Users.Where(u => u.RowState != RowState.Deleted);
        }

        public void Add(User entity)
        {
            _context.Users.Add(entity);
            _context.SaveChanges();
        }

        public void Save(User entity)
        {
            var original = GetUsers().FirstOrDefault(u => u.Id == entity.Id);

            original.Access = entity.Access;
            original.FullName = entity.FullName;
            original.Metadata = entity.Metadata;
            original.State = entity.State;
            original.Email = entity.Email;
            original.UserName = entity.UserName;
            original.PasswordHash = entity.PasswordHash;
            original.LastLoggedIn = entity.LastLoggedIn;

            _context.SaveChanges();
        }

        public void Delete(User user)
        {
            user = GetUsers().FirstOrDefault(u => u.Id == user.Id);

            if(user == null)
                throw new NotFoundException("User not found");

            user.RowState = RowState.Deleted;
            _context.SaveChanges();
        }
    }
}
