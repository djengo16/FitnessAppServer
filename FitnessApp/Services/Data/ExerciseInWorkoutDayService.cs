using FitnessApp.Dto.ExerciseInWorkoutDay;
using FitnessApp.Models;
using FitnessApp.Models.Repositories;

namespace FitnessApp.Services.Data
{
    public class ExerciseInWorkoutDayService : IExerciseInWorkoutDayService
    {
        private readonly IRepository<ExerciseInWorkoutDay> exerciseInWorkoutDayStorage;

        public ExerciseInWorkoutDayService(IRepository<ExerciseInWorkoutDay> exerciseInWorkoutDayStorage)
        {
            this.exerciseInWorkoutDayStorage = exerciseInWorkoutDayStorage;
        }
        public async Task AddAsync(ExerciseInWorkoutDayDTO exerciseInWorkoutDayDTO)
        {
            ExerciseInWorkoutDay exercise = new ExerciseInWorkoutDay
            {
                ExerciseId = exerciseInWorkoutDayDTO.ExerciseId,
                WorkoutDayId = exerciseInWorkoutDayDTO.WorkoutDayId,
                MinReps = exerciseInWorkoutDayDTO.MinReps,
                MaxReps = exerciseInWorkoutDayDTO.MaxReps,
                Sets = exerciseInWorkoutDayDTO.Sets,
            };

            await exerciseInWorkoutDayStorage.AddAsync(exercise);
            await exerciseInWorkoutDayStorage.SaveChangesAsync();
        }
        public async Task DeleteAllWithExerciseId(int id)
        {
            var entities = exerciseInWorkoutDayStorage.All().Where(x => x.ExerciseId == id).ToList();
            entities.ForEach(x => exerciseInWorkoutDayStorage.Delete(x));

            await exerciseInWorkoutDayStorage.SaveChangesAsync();
        }

        public async Task DeleteExerciseInWorkoutDay(int exerciseId, string workoutId)
        {
            var exerciseToDelete = exerciseInWorkoutDayStorage
                .All()
                .FirstOrDefault(x => x.ExerciseId == exerciseId && x.WorkoutDayId == workoutId);

            exerciseInWorkoutDayStorage.Delete(exerciseToDelete);
            await exerciseInWorkoutDayStorage.SaveChangesAsync();
        }

    }
}
