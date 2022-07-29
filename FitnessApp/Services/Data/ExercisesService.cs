namespace FitnessApp.Services.Data
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using FitnessApp.Dto.Exercises;
    using FitnessApp.Models;
    using FitnessApp.Models.Enums;
    using FitnessApp.Models.Repositories;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FitnessApp.Helper;
    using FitnessApp.Services.ServiceConstants;

    public class ExercisesService : IExercisesService
    {
        private readonly IDeletableEntityRepository<Exercise> exercisesStorage;
        private readonly IMapper mapper;
        private readonly IExerciseInWorkoutDayService exerciseInWorkoutDayService;

        public ExercisesService(
            IDeletableEntityRepository<Exercise> exercisesStorage,
            IMapper mapper,
            IExerciseInWorkoutDayService exerciseInWorkoutDayService)
        {
            this.exercisesStorage = exercisesStorage;
            this.mapper = mapper;
            this.exerciseInWorkoutDayService = exerciseInWorkoutDayService;
        }

        public async Task CreateExerciseAsync(CreateOrUpdateExerciseDTO exerciseDTO)
        {
            var exercise = new Exercise
            {
                Name = exerciseDTO.Name,
                MuscleGroup = exerciseDTO.MuscleGroup,
                Difficulty = exerciseDTO.Difficulty,
                Description = exerciseDTO.Description,
                PictureResourceUrl = exerciseDTO.PictureResourceUrl,
                VideoResourceUrl = exerciseDTO.VideoResourceUrl
            };
            await exercisesStorage.AddAsync(exercise);
            await exercisesStorage.SaveChangesAsync();
        }

        public GetExerciseDetailsDTO GetExerciseDetails(int exerciseId)
        {
            var exercise = exercisesStorage
                .All()
                .Where(x => x.Id == exerciseId)
                .ProjectTo<GetExerciseDetailsDTO>(this.mapper.ConfigurationProvider)
                .FirstOrDefault();

            return exercise;
        }

        public IEnumerable<ExerciseInListDTO> GetExercises(string searchParams, int? take = null, int skip = 0)
        {
            var exerciseQueryBuilder = new ExerciseQueryBuilder(exercisesStorage.All());

            take = exerciseQueryBuilder.ApplySearch(searchParams).GetCount(take);

            var currentQuery =
            exerciseQueryBuilder
                .ApplySearch(searchParams)
                .ApplyPagination(take, skip)
                .Build();

            return currentQuery.ProjectTo<ExerciseInListDTO>(mapper.ConfigurationProvider).ToList();
        }

        public int GetCount()
        {
            return this.exercisesStorage.AllAsNoTracking().Count();
        }

        public int GetCountBySearchParams(string searchParams)
        {
            var muscleGroup = MuscleGroupFinder.FindMuscleGroup(searchParams);
            return this.exercisesStorage
                .AllAsNoTracking()
                .Where(x => !string.IsNullOrEmpty(searchParams)
                             ? (x.Name.Contains(searchParams)
                             || (int)x.MuscleGroup == muscleGroup)
                             : true)
                .Count();
        }

        public async Task UpdateExerciseAsync(int exerciseId, CreateOrUpdateExerciseDTO exerciseDTO)
        {
            var exercise = this.GetById(exerciseId);

            exercise.Name = exerciseDTO.Name;
            exercise.Description = exerciseDTO.Description;
            exercise.MuscleGroup = exerciseDTO.MuscleGroup;
            exercise.Difficulty = exerciseDTO.Difficulty;
            exercise.PictureResourceUrl = exerciseDTO.PictureResourceUrl;
            exercise.VideoResourceUrl = exerciseDTO.VideoResourceUrl;

            await this.exercisesStorage.SaveChangesAsync();
        }
        /***
         * First (soft) deletes all ExerciseInWorkoutDay entities
         * with the given exercise id then (soft) deletes the specific Exercise.
         */
        public async Task Delete(int id)
        {
            var exercise = GetById(id);

            if(exercise == null)
            {
                throw new ArgumentException(ErrorMessages.ExerciseNotFound);
            }

            await this.exerciseInWorkoutDayService.DeleteAllWithExerciseId(id);

            this.exercisesStorage.Delete(exercise);

            await this.exercisesStorage.SaveChangesAsync();
        }

        public Exercise GetById(int id) => this.exercisesStorage.All().FirstOrDefault(x => x.Id == id);
    }
}