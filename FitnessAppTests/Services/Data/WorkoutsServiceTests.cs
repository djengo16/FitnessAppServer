namespace FitnessApp.Tests.Services.Data
{
    using FitnessApp.Data;
    using FitnessApp.Dto.Workouts;
    using FitnessApp.Models;
    using FitnessApp.Models.Enums;
    using FitnessApp.Seeding;
    using FitnessApp.Services.Data;
    using FitnessApp.Tests.Helper;

    public class WorkoutsServiceTests
    {
        private IWorkoutsService workoutsService;
        private ApplicationDbContext db;

        private const int threeWorkoutDays = 3;
        private const int fourWorkoutDays = 4;
        private const int fiveWorkoutDays = 5;

        [SetUp]
        public async Task Setup()
        {
            db = ServiceHelper.GetRequiredService<ApplicationDbContext>();
            workoutsService = ServiceHelper.GetRequiredService<IWorkoutsService>();

            ExercisesSeeder exercisesSeeder = new ExercisesSeeder();
            await exercisesSeeder.SeedAsync(db, null);
        }

        [Test]
        [TestCase(threeWorkoutDays)]
        [TestCase(fourWorkoutDays)]
        [TestCase(fiveWorkoutDays)]
        public void GenerateWorkoutPlansWillGenerateProgramsComplyingWithTheInputedDays(int days)
        {
            WorkoutGenerationInputModel input = new WorkoutGenerationInputModel();
            input.UserId = Guid.NewGuid().ToString();
            input.Days = days;
            input.Difficulty = Difficulty.Medium;
            input.Goal = Goal.LoseWeight;
            input.Count = 1;

            this.workoutsService.GenerateWorkoutPlans(input);
            var result = this.workoutsService
                .GetGeneratedWorkoutPlans()
                .First();

            Assert.That(result.WorkoutDays.Count == days);
        }

        [Test]
        [TestCase(threeWorkoutDays, Difficulty.Easy, Goal.GainMuscle)]
        [TestCase(fourWorkoutDays, Difficulty.Medium, Goal.LoseWeight)]
        [TestCase(fiveWorkoutDays, Difficulty.Hard, Goal.Maintain)]
        public void GenerateWorkoutPlansWillGenerateProgramsWithExercisesThatDontRepeatInADay(
            int days, Difficulty difficulty, Goal goal)
        {
            WorkoutGenerationInputModel input = new WorkoutGenerationInputModel();
            input.UserId = Guid.NewGuid().ToString();
            input.Days = days;
            input.Difficulty = difficulty;
            input.Goal = goal;
            input.Count = 1;

            this.workoutsService.GenerateWorkoutPlans(input);
            var workoutPlan = this.workoutsService
                .GetGeneratedWorkoutPlans()
                .First();

            bool doRepeat = false;

            foreach (var day in workoutPlan.WorkoutDays)
            {
                //if we have 2 or more exercises with the same name,
                //will get them as a new collection (shown bellow).
                var duplicate =
                    day.ExercisesInWorkoutDays.GroupBy(x => x.Name).Where(g => g.Count() > 1);

                if (duplicate.Any())
                {
                    doRepeat = true;
                    break;
                }
            }

            Assert.IsFalse(doRepeat);
        }

        [Test]
        [TestCase(threeWorkoutDays, Difficulty.Easy, Goal.LoseWeight)]
        [TestCase(fourWorkoutDays, Difficulty.Medium, Goal.LoseWeight)]
        [TestCase(fiveWorkoutDays, Difficulty.Hard, Goal.LoseWeight)]
        public void GenerateWorkoutPlansWillGenerateProgramsWithCardioAddedToLeastBusyDayIfGoalIsLooseWeight(
            int days, Difficulty difficulty, Goal goal)
        {
            WorkoutGenerationInputModel input = new WorkoutGenerationInputModel();
            input.UserId = Guid.NewGuid().ToString();
            input.Days = days;
            input.Difficulty = difficulty;
            input.Goal = goal;
            input.Count = 1;

            this.workoutsService.GenerateWorkoutPlans(input);
            var workoutPlan = this.workoutsService
                .GetGeneratedWorkoutPlans()
                .First();

            bool doHaveCardioExercise = false;


            var leastBusyDay = workoutPlan
                .WorkoutDays
                .OrderBy(x => x.ExercisesInWorkoutDays.Count)
                .First();

            foreach (var exercise in leastBusyDay.ExercisesInWorkoutDays)
            {
                var group = db
                    .Exercises
                    .First(x => x.Id == exercise.ExerciseId)
                    .MuscleGroup;

                if (group == MuscleGroup.Cardio)
                {
                    doHaveCardioExercise = true;
                }
            }

            Assert.IsTrue(doHaveCardioExercise);
        }

        [Test]
        public async Task SaveWorkoutPlanAsyncWillWorkCorrectly()
        {
            string userId = Guid.NewGuid().ToString();

            await db.Users.AddAsync(new ApplicationUser() { Id = userId });
            await db.SaveChangesAsync();

            WorkoutGenerationInputModel input = new WorkoutGenerationInputModel();
            input.UserId = userId;
            input.Days = 4;
            input.Difficulty = Difficulty.Hard;
            input.Goal = Goal.Maintain;
            input.Count = 1;

            this.workoutsService.GenerateWorkoutPlans(input);
            var workoutPlan = this.workoutsService
                .GetGeneratedWorkoutPlans()
                .First();

            var workoutPlanId  = await this.workoutsService.SaveWorkoutPlanAsync(workoutPlan);

            var resultPlan = this.workoutsService.GetUserWorkoutPlan(userId, workoutPlanId);

            Assert.NotNull(resultPlan);
            Assert.That(resultPlan.WorkoutDays.Count() == 4);
        }

        //Should get the plan with the given id and should set its IsActive property to true
        //if the current plan is the last saved for this user (the active one)
        [Test]
        public async Task GetUserWorkoutPlansWillWorkCorrectly()
        {
            string userId = Guid.NewGuid().ToString();

            await db.Users.AddAsync(new ApplicationUser() { Id = userId });
            await db.SaveChangesAsync();

            //First personalization
            WorkoutGenerationInputModel input = new WorkoutGenerationInputModel();
            input.UserId = userId;
            input.Days = 4;
            input.Difficulty = Difficulty.Hard;
            input.Goal = Goal.Maintain;
            input.Count = 1;

            this.workoutsService.GenerateWorkoutPlans(input);
            var workoutPlan = this.workoutsService
                .GetGeneratedWorkoutPlans()
                .First();

            await this.workoutsService.SaveWorkoutPlanAsync(workoutPlan);

            //Second personalization (we expect the user's active plan to be this one)
            input.Days = 5;
            input.Difficulty = Difficulty.Medium;
            input.Goal = Goal.LoseWeight;
            this.workoutsService.GenerateWorkoutPlans(input);
            var workoutPlan2 = this.workoutsService
                .GetGeneratedWorkoutPlans()
                .First();

            var activePlanId = await this.workoutsService.SaveWorkoutPlanAsync(workoutPlan2);

            var resultPlan = this.workoutsService.GetUserWorkoutPlan(userId, activePlanId);

            Assert.NotNull(resultPlan);
            Assert.That(resultPlan.WorkoutDays.Count() == 5);
            Assert.That(resultPlan.Difficulty == Difficulty.Medium);
            Assert.That(resultPlan.Goal == Goal.LoseWeight);
            Assert.True(resultPlan.IsActive);
        }



    }
}
