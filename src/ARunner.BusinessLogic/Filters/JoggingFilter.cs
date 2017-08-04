using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ARunner.BusinessLogic.Models;
using ARunner.DataLayer.Model;

namespace ARunner.BusinessLogic.Filters
{
    public class JoggingFilter : EntityMapper<Jogging, JoggingViewModel>
    {
        public DateTime? From { get; set; }

        public DateTime? To { get; set; }

        public string UserId { get; set; }

        public override PaggingCollection<JoggingViewModel> ApplyFilter(IQueryable<Jogging> source, Func<Jogging, JoggingViewModel> mapper)
        {
            source = source.Where(j => j.User.Id == UserId);

            if (From.HasValue)
                source = source.Where(j => j.Created >= From.Value);

            if (To.HasValue)
                source = source.Where(j => j.Created <= To.Value);

            source = source.OrderByDescending(j => j.Created);

            return new PaggingCollection<JoggingViewModel>(source.ToList().Select(mapper), this);
        }
    }
}
