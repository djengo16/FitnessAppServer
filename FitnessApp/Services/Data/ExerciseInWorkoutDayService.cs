namespace FitnessApp.Services.Data
{
    using FitnessApp.Dto.ExerciseInWorkoutDay;
    using FitnessApp.Models;
    using FitnessApp.Models.Repositories;
    using FitnessApp.Services.ServiceConstants;
    using System.Collections.Generic;

    public class ExerciseInWorkoutDayService : IExerciseInWorkoutDayService
    {
        private readonly IRepository<ExerciseInWorkoutDay> exerciseInWorkoutDayStorage;
        private readonly IDeletableEntityRepository<Exercise> exerciseStorage;
        private readonly IWorkoutDaysService workoutDaysService;

        public ExerciseInWorkoutDayService(
            IRepository<ExerciseInWorkoutDay> exerciseInWorkoutDayStorage,
            IDeletableEntityRepository<Exercise> exerciseStorage,
            IWorkoutDaysService workoutDaysService)
        {
            this.exerciseInWorkoutDayStorage = exerciseInWorkoutDayStorage;
            this.exerciseStorage = exerciseStorage;
            this.workoutDaysService = workoutDaysService;
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

        /// <summary>
        /// This method adds exercise to user's workout day if the exercise is not already in this day.
        /// </summary>
        /// <param name="dto">Exercise that will be added to user's workout day.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">When the exercise is already there. One exercise cannot be twice in one program.</exception>
        public async Task<GeneratedExerciseInWorkoutDayDTO> AddToExistingWorkoutDayAsync(AddExerciseToExistingDayDTO dto)
        {
            if (DoesExerciseExistInWorkoutDay(dto.WorkoutDayId, dto.ExerciseId))
            {
                throw new ArgumentException(ErrorMessages.ExerciseAlreadyInProgram);
            }

            await this.AddAsync(new ExerciseInWorkoutDayDTO ()
            {
                WorkoutDayId = dto.WorkoutDayId,
                ExerciseId = dto.ExerciseId,
                MinReps = WorkoutConstants.AvgExerciseMinReps,
                MaxReps = WorkoutConstants.AvgExerciseMaxReps,
                Sets = WorkoutConstants.AvgExerciseSets
            });
            var exerciseDetails = exerciseStorage.GetById(dto.ExerciseId);

            return new GeneratedExerciseInWorkoutDayDTO()
            {
                ExerciseId = dto.ExerciseId,
                Name = exerciseDetails.Name,
                Difficulty = exerciseDetails.Difficulty,
                Sets = WorkoutConstants.AvgExerciseSets,
                MinReps = WorkoutConstants.AvgExerciseMinReps,
                MaxReps = WorkoutConstants.AvgExerciseMaxReps,
                Description = exerciseDetails.Description,
                PictureResourceUrl = exerciseDetails.PictureResourceUrl,
                VideoResourceUrl = exerciseDetails.VideoResourceUrl,
            };
        }

        public async Task DeleteAllWithExerciseIdAsync(int id)
        {
            var entities = exerciseInWorkoutDayStorage.All().Where(x => x.ExerciseId == id).ToList();
            entities.ForEach(x => exerciseInWorkoutDayStorage.Delete(x));

            await exerciseInWorkoutDayStorage.SaveChangesAsync();
        }

        public async Task DeleteAsync(int exerciseId, string workoutDayId)
        {
            var exerciseToDelete = exerciseInWorkoutDayStorage
                .All()
                .FirstOrDefault(x => x.ExerciseId == exerciseId && x.WorkoutDayId == workoutDayId);

            exerciseInWorkoutDayStorage.Delete(exerciseToDelete);
            await exerciseInWorkoutDayStorage.SaveChangesAsync();
        }

        public async Task UpdateRangeAsync(ICollection<UpdateExerciseInWorkoutDayDTO> dtos)
        {
            foreach (var dto in dtos)
            {
                var exercise = exerciseInWorkoutDayStorage.All()
                .FirstOrDefault(x => x.ExerciseId == dto.ExerciseId && x.WorkoutDayId == dto.WorkoutDayId);

                exercise.Sets = dto.Sets;
                exercise.MinReps = dto.MinReps;
                exercise.MaxReps = dto.MaxReps;
                exerciseInWorkoutDayStorage.Update(exercise);
            }
            await exerciseInWorkoutDayStorage.SaveChangesAsync();
        }

        private bool DoesExerciseExistInWorkoutDay(string workoutDayId, int exerciseId)
        {
            var exerciseIdsInWorkoutDay = this.workoutDaysService.GetExerciseIdsInWorkoutDay(workoutDayId);

            foreach (var id in exerciseIdsInWorkoutDay)
            {
                if(id == exerciseId)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
