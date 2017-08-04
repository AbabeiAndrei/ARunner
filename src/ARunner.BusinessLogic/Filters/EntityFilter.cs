namespace ARunner.BusinessLogic.Filters
{
    public class EntityFilter
    {
        protected EntityFilter()
        {
            PageSize = 15;
            OrderDirection = SortDirection.Ascending;
        }

        public string OrderBy { get; set; }
        public SortDirection OrderDirection { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }

        public static EntityFilter Default => new EntityFilter();
    }
}