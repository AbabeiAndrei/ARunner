using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ARunner.BusinessLogic.Exceptions;
using ARunner.DataLayer;
using ARunner.DataLayer.Model;
using Microsoft.EntityFrameworkCore;

namespace ARunner.Repositories
{
    public class JoggingsRepository : IJoggingsRepository
    {
        private readonly ADbContext _context;

        public JoggingsRepository(ADbContext context)
        {
            _context = context;
        }

        public IQueryable<Jogging> GetJoggings()
        {
            return _context.Joggings.Include(j => j.User)
                           .Where(j => j.RowState != RowState.Deleted);
        }

        public void Add(Jogging entity)
        {
            _context.Joggings.Add(entity);
            _context.SaveChanges();
        }

        public void Save(Jogging entity)
        {
            var original = GetJoggings().FirstOrDefault(j => j.Id == entity.Id);

            original.Created = entity.Created;
            original.Distance = entity.Distance;
            original.Time = entity.Time;
            original.User = entity.User;

            _context.SaveChanges();
        }

        public void Delete(Jogging jogging)
        {
            jogging = GetJoggings().FirstOrDefault(j => j.Id == jogging.Id);

            if(jogging == null)
                throw new NotFoundException("Jogging not found");

            jogging.RowState = RowState.Deleted;
            _context.SaveChanges();
        }
    }
}
