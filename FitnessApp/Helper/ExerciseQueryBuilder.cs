namespace FitnessApp.Helper
{
    using FitnessApp.Models;

    public class ExerciseQueryBuilder : QueryBuilder<Exercise>
    {

        public ExerciseQueryBuilder(IQueryable<Exercise> currentQuery) : base(currentQuery)
        {
            this.CurrentQuery = currentQuery;
        }
        public override IQueryable<Exercise> CurrentQuery { get; set; }
        /// <summary>
        /// Apply search to the given query if we have searchParams if not
        /// we return true for every entity.
        /// </summary>
        /// <param name="searchParams">searchParameter used to check if entity params match with it</param>
        /// <returns>QueryBuilder</returns>
        public override QueryBuilder<Exercise> ApplySearch(string searchParams)
        {
            var muscleGroup = MuscleGroupFinder.FindMuscleGroup(searchParams);

            this.CurrentQuery = CurrentQuery
            .Where(x => !string.IsNullOrEmpty(searchParams)
                            ? (x.Name.Contains(searchParams)
                            || (int)x.MuscleGroup == muscleGroup)
                            : true);
            return this;
        }
    }
}
