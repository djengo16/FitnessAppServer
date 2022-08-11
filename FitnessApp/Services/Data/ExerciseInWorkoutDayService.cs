namespace FitnessApp.Services.Data
{
    using AutoMapper;
    using FitnessApp.Dto.ExerciseInWorkoutDay;
    using FitnessApp.Models;
    using FitnessApp.Models.Enums;
    using FitnessApp.Models.Repositories;
    using FitnessApp.Services.ServiceConstants;

    public class ExerciseInWorkoutDayService : IExerciseInWorkoutDayService
    {
        private readonly IRepository<ExerciseInWorkoutDay> exerciseInWorkoutDayStorage;
        private readonly IDeletableEntityRepository<Exercise> exerciseStorage;
        private readonly IWorkoutDaysService workoutDaysService;
        private readonly IMapper mapper;

        public ExerciseInWorkoutDayService(
            IRepository<ExerciseInWorkoutDay> exerciseInWorkoutDayStorage,
            IDeletableEntityRepository<Exercise> exerciseStorage,
            IWorkoutDaysService workoutDaysService,
             IMapper mapper)
        {
            this.exerciseInWorkoutDayStorage = exerciseInWorkoutDayStorage;
            this.exerciseStorage = exerciseStorage;
            this.workoutDaysService = workoutDaysService;
            this.mapper = mapper;
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

        public async Task<GeneratedExerciseInWorkoutDayDTO> AddExerciseInWorkoutDayAsync(AddExerciseInWorkoutDayDTO dto)
        {
            var exerciseIdsInWorkoutDay = this.workoutDaysService.GetExerciseIdsInWorkoutDay(dto.WorkoutDayId);

            exerciseIdsInWorkoutDay.ForEach(id =>
            {
                if (id == dto.ExerciseId)
                {
                    throw new ArgumentException(ErrorMessages.ExerciseAlreadyInProgram);
                }
            });


            ExerciseInWorkoutDay exercise = new ExerciseInWorkoutDay
            {
                ExerciseId = dto.ExerciseId,
                WorkoutDayId = dto.WorkoutDayId,
                MinReps = 8,
                MaxReps = 12,
                Sets = 3,
            };

            await exerciseInWorkoutDayStorage.AddAsync(exercise);
            await exerciseInWorkoutDayStorage.SaveChangesAsync();

            var exerciseDetails = exerciseStorage.GetById(exercise.ExerciseId);

            return new GeneratedExerciseInWorkoutDayDTO()
            {
                ExerciseId = dto.ExerciseId,
                Name = exerciseDetails.Name,
                Difficulty = exerciseDetails.Difficulty,
                Sets = 3,
                MinReps = 8,
                MaxReps = 12,
                Description = exerciseDetails.Description,
                PictureResourceUrl = exerciseDetails.PictureResourceUrl,
                VideoResourceUrl = exerciseDetails.VideoResourceUrl,
            };
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
