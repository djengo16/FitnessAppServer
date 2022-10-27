namespace FitnessApp.Helper.Builders.Exercise
{
    using System.Linq;
    public abstract class QueryBuilder<T>
    {
        public QueryBuilder(IQueryable<T> currentQuery)
        {
            CurrentQuery = currentQuery;
        }
        public abstract IQueryable<T> CurrentQuery { get; set; }

        public abstract QueryBuilder<T> ApplySearch(string searchParams);
        public int? GetCount(int? take)
        {
            if (CurrentQuery.Count() < take)
            {
                take = CurrentQuery.Count();
            }
            return take;
        }
        public int GetTotalCount() => CurrentQuery.Count();
        public QueryBuilder<T> ApplyPagination(int? take = null, int skip = 0)
        {
            CurrentQuery = take.HasValue
                ? CurrentQuery.Skip(skip).Take(take.Value)
                : CurrentQuery.Skip(skip);

            return this;
        }
        public IQueryable<T> Build() => CurrentQuery;
    }
}
