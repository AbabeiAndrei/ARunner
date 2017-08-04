using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ARunner.BusinessLogic.Filters;
using ARunner.DataLayer.Model;

namespace ARunner.BusinessLogic
{
    public class PaggingCollection<T>
    {
        private readonly IEnumerable<T> _source;
        private readonly EntityFilter _filter;

        public IEnumerable<T> Items => GetPaged();

        public int Pages => (int) Math.Ceiling((double)_source.Count() / _filter.PageSize);

        public int Page => _filter.Page;

        public PaggingCollection(IEnumerable<T> source, EntityFilter filter)
        {
            if(source == null)
                throw new ArgumentNullException(nameof(source));

            if(filter == null)
                throw new ArgumentNullException(nameof(filter));

            _source = source;
            _filter = filter;
        }

        private IEnumerable<T> GetPaged()
        {
            return _filter != null
                       ? _source.Skip(_filter.Page * _filter.PageSize).Take(_filter.PageSize)
                       : _source;
        }
    }
}
