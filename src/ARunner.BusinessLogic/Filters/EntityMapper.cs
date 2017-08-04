using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ARunner.BusinessLogic.Filters
{
    public enum SortDirection
    {
        Ascending,
        Descending
    }

    public abstract class EntityMapper<TSource, TModel> : EntityFilter
    {
        public abstract PaggingCollection<TModel> ApplyFilter(IQueryable<TSource> source, Func<TSource, TModel> mapper);

    }
}
