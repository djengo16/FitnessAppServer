namespace FitnessApp.Helper.Builders.Exercise
{
    using FitnessApp.Models;
    using FitnessApp.Models.Enums;

    public class ExerciseQueryBuilder : QueryBuilder<Exercise>
    {

        public ExerciseQueryBuilder(IQueryable<Exercise> currentQuery) : base(currentQuery)
        {
            CurrentQuery = currentQuery;
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

            CurrentQuery = CurrentQuery
            .Where(x => !string.IsNullOrEmpty(searchParams)
                            ? x.Name.Contains(searchParams)
                            || (int)x.MuscleGroup == muscleGroup
                            : true);
            return this;
        }
        public QueryBuilder<Exercise> ApplyFilter(MuscleGroup muscleGroup, Difficulty difficulty)
        {
            // When we get 0, we don't apply this filters, it's default values, so that means the user haven't filtered anything
            if (muscleGroup != 0)
            {
                CurrentQuery = CurrentQuery
                    .Where(x => x.MuscleGroup == muscleGroup);
            }
            if (difficulty != 0)
            {
                CurrentQuery = CurrentQuery
                .Where(x => x.Difficulty == difficulty);
                return this;
            }

            return this;
        }
    }
}
