namespace FitnessApp.Tests.Services.Data
{
    using FitnessApp.Data;
    using FitnessApp.Dto.ExerciseInWorkoutDay;
    using FitnessApp.Dto.Workouts;
    using FitnessApp.Models;
    using FitnessApp.Models.Enums;
    using FitnessApp.Seeding;
    using FitnessApp.Services.Data;
    using FitnessApp.Services.ServiceConstants;
    using FitnessApp.Tests.Helper;
    using System.ComponentModel.DataAnnotations;

    public class ExerciseInWorkoutDayServiceTests
    {
        private IExerciseInWorkoutDayService exerciseInWorkoutDayService;
        private IWorkoutsService workoutsService;
        private ApplicationDbContext db;


        [SetUp]
        public async Task Setup()
        {

            db = ServiceHelper.GetRequiredService<ApplicationDbContext>();
            exerciseInWorkoutDayService = ServiceHelper.GetRequiredService<IExerciseInWorkoutDayService>();
            workoutsService = ServiceHelper.GetRequiredService<IWorkoutsService>();

            ExercisesSeeder exercisesSeeder = new ExercisesSeeder();
            await exercisesSeeder.SeedAsync(db, null);
        }

        [Test]
        public async Task AddAsyncWillAddTheExercise()
        {
            string TestUserId = Guid.NewGuid().ToString();
            var plan = await SetupAndGetWorkoutPlanAsync(TestUserId);

            var workoutDay = plan.WorkoutDays.First();

            var exercise = db.Exercises
                .FirstOrDefault(x => !workoutDay.ExercisesInWorkoutDays.Select(x => x.ExerciseId).ToList().Contains(x.Id));

            await this.exerciseInWorkoutDayService.AddAsync(new ExerciseInWorkoutDayDTO()
            {
                ExerciseId = exercise.Id,
                WorkoutDayId = workoutDay.Id,
                MinReps = WorkoutConstants.AvgExerciseMinReps,
                MaxReps = WorkoutConstants.AvgExerciseMaxReps,
                Sets = WorkoutConstants.AvgExerciseSets
            });

            var test = db.WorkoutDays.FirstOrDefault(x => x.Id == workoutDay.Id);

            var updatedPlan = workoutsService.GetUserWorkoutPlan(TestUserId, plan.Id);
            var updatedWorkoutDay = updatedPlan.WorkoutDays.First();

            Assert.That(updatedWorkoutDay.ExercisesInWorkoutDays.Any(x => x.ExerciseId == exercise.Id));
        }

        [Test]
        public async Task AddToExistingWorkoutDayAsyncWillAddExercise()
        {
            string TestUserId = Guid.NewGuid().ToString();
            var plan = await SetupAndGetWorkoutPlanAsync(TestUserId);
            var workoutDay = plan.WorkoutDays.First();
            var exercise = db.Exercises
                .FirstOrDefault(x => !workoutDay.ExercisesInWorkoutDays.Select(x => x.ExerciseId).ToList().Contains(x.Id));

            await exerciseInWorkoutDayService.AddToExistingWorkoutDayAsync(new AddExerciseToExistingDayDTO()
            {
                ExerciseId = exercise.Id,
                WorkoutDayId = workoutDay.Id,
            });

            var updatedPlan = workoutsService.GetUserWorkoutPlan(TestUserId, plan.Id);
            var updatedWorkoutDay = updatedPlan.WorkoutDays.First();

            Assert.That(updatedWorkoutDay.ExercisesInWorkoutDays.Any(x => x.ExerciseId == exercise.Id));
        }

        [Test]
        public async Task AddToExistingWorkoutDayAsyncWillThrowExceptionWhenTryingToAddOneExerciseTwice()
        {
            string TestUserId = Guid.NewGuid().ToString();
            var plan = await SetupAndGetWorkoutPlanAsync(TestUserId);
            var workoutDay = plan.WorkoutDays.First();
            var exercise = db.Exercises
                .FirstOrDefault(x => !workoutDay.ExercisesInWorkoutDays.Select(x => x.ExerciseId).ToList().Contains(x.Id));

            var exerciseDTO = new AddExerciseToExistingDayDTO()
            {
                ExerciseId = exercise.Id,
                WorkoutDayId = workoutDay.Id,
            };

            await exerciseInWorkoutDayService.AddToExistingWorkoutDayAsync(exerciseDTO);

            Assert.ThrowsAsync(typeof(ArgumentException), async () => await exerciseInWorkoutDayService.AddToExistingWorkoutDayAsync(exerciseDTO));
        }

        [Test]
        public async Task DeleteAllWithExerciseIdAsyncWillDeleteAllEntitiesWithTheGivenExerciseId()
        {
            string TestUserId = Guid.NewGuid().ToString();
            var plan = await SetupAndGetWorkoutPlanAsync(TestUserId);
            var workoutDay = plan.WorkoutDays.First();

            var exerciseInPlan = workoutDay.ExercisesInWorkoutDays.First();

            await exerciseInWorkoutDayService.DeleteAllWithExerciseIdAsync(exerciseInPlan.ExerciseId);

            var exerciseInWorkoutDayResult = db.ExercisesInWorkoutDays.Where(x => x.ExerciseId == exerciseInPlan.ExerciseId).ToList();

            Assert.That(exerciseInWorkoutDayResult.Count() == 0);
        }

        [Test]
        public async Task DeleteAsyncWillDeleteAndRemoveTheExerciseFromWorkoutDay()
        {
            string TestUserId = Guid.NewGuid().ToString();
            var plan = await SetupAndGetWorkoutPlanAsync(TestUserId);
            var workoutDay = plan.WorkoutDays.First();
            var exerciseToDelete = workoutDay.ExercisesInWorkoutDays.First();

            await exerciseInWorkoutDayService.DeleteAsync(exerciseToDelete.ExerciseId, workoutDay.Id);

            var updatedDay = workoutsService
                .GetUserWorkoutPlan(TestUserId, plan.Id)
                .WorkoutDays
                .FirstOrDefault(x => x.Id == workoutDay.Id);

            Assert.That(!updatedDay.ExercisesInWorkoutDays.Any(x => x.ExerciseId == exerciseToDelete.ExerciseId));
        }

        [Test]
        public async Task UpdateRangeAsyncWillUpdateTheExercisesCorrectly()
        {
            string TestUserId = Guid.NewGuid().ToString();
            var plan = await SetupAndGetWorkoutPlanAsync(TestUserId);
            var workoutDay = plan.WorkoutDays.First();

            int sets = 10;
            int minReps = 30;
            int maxReps = 40;

            List<UpdateExerciseInWorkoutDayDTO> exercisesToUpdate = new List<UpdateExerciseInWorkoutDayDTO>();

            foreach (var exercise in workoutDay.ExercisesInWorkoutDays)
            {
                var currentExerciseToUpdate = new UpdateExerciseInWorkoutDayDTO()
                {
                    ExerciseId = exercise.ExerciseId,
                    WorkoutDayId = workoutDay.Id,
                    Sets = sets,
                    MinReps = minReps,
                    MaxReps = maxReps,
                };
                exercisesToUpdate.Add(currentExerciseToUpdate);
            }

            await exerciseInWorkoutDayService.UpdateRangeAsync(exercisesToUpdate);

            var updatedDay = workoutsService
                .GetUserWorkoutPlan(TestUserId, plan.Id)
                .WorkoutDays
                .FirstOrDefault(x => x.Id == workoutDay.Id);

            Assert.That(!updatedDay.ExercisesInWorkoutDays.Any(x => x.Sets != sets && x.MinReps != minReps && x.MaxReps != maxReps));
        }

        private async Task<GeneratedWorkoutPlanDTO> SetupAndGetWorkoutPlanAsync(string userId)
        {
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

            var workoutPlanId = await this.workoutsService.SaveWorkoutPlanAsync(workoutPlan);

            return workoutsService.GetUserWorkoutPlan(userId, workoutPlanId);
        }
    }
}
