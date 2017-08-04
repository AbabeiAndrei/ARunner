using System.Collections.Generic;
using System.Linq;
using ARunner.DataLayer.Model;

namespace ARunner.Repositories
{
    public interface IJoggingsRepository
    {
        IQueryable<Jogging> GetJoggings();
        void Add(Jogging entity);
        void Save(Jogging entity);
        void Delete(Jogging jogging);
    }
}