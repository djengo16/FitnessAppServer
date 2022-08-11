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

        public ExerciseDTО GetExerciseDetails(int exerciseId)
        {
            var exercise = exercisesStorage
                .All()
                .Where(x => x.Id == exerciseId)
                .ProjectTo<ExerciseDTО>(this.mapper.ConfigurationProvider)
                .FirstOrDefault();

            return exercise;
        }

        public ExercisesPageDTO GetExercises(
            string searchParams, 
            int? take = null, 
            int skip = 0, 
            Difficulty difficulty = 0, 
            MuscleGroup muscleGroup = 0)
        {
            var exerciseQueryBuilder = new ExerciseQueryBuilder(exercisesStorage.All());

            //take = exerciseQueryBuilder
            //    .ApplyFilter(muscleGroup, difficulty)
            //    .ApplySearch(searchParams)
            //    .GetCount(take);

            int totalCount = exerciseQueryBuilder
                .ApplyFilter(muscleGroup, difficulty)
                .ApplySearch(searchParams)
                .GetTotalCount();

            var currentQuery =
            exerciseQueryBuilder
                .ApplyFilter(muscleGroup, difficulty)
                .ApplySearch(searchParams)
                .ApplyPagination(take, skip)
                .Build();

            var dto = new ExercisesPageDTO()
            {
                Exercises = currentQuery.ProjectTo<ExerciseInListDTO>(mapper.ConfigurationProvider).ToList(),
                TotalData = totalCount,
            };

            return dto;
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

        public async Task CreateAsync(ExerciseInputDTO exerciseInput)
        {
            var exercise = new Exercise()
            {
                Name = exerciseInput.Name,
                PictureResourceUrl = exerciseInput.PictureResourceUrl,
                VideoResourceUrl = exerciseInput.VideoResourceUrl,
                Difficulty = Enum.Parse<Difficulty>(exerciseInput.Difficulty),
                MuscleGroup = Enum.Parse<MuscleGroup>(exerciseInput.MuscleGroup),
                Description = exerciseInput.Description
            };

            await this.exercisesStorage.AddAsync(exercise);
            await this.exercisesStorage.SaveChangesAsync();
        }

        public async Task UpdateAsync(ExerciseUpdateDTO exerciseInput)
        {
            var exercise = GetById(exerciseInput.Id);


            exercise.Name = exerciseInput.Name;
            exercise.PictureResourceUrl = exerciseInput.PictureResourceUrl;
            exercise.VideoResourceUrl = exerciseInput.VideoResourceUrl;
            exercise.Difficulty = Enum.Parse<Difficulty>(exerciseInput.Difficulty);
            exercise.MuscleGroup = Enum.Parse<MuscleGroup>(exerciseInput.MuscleGroup);
            exercise.Description = exerciseInput.Description;


            this.exercisesStorage.Update(exercise);
            await this.exercisesStorage.SaveChangesAsync();
        }
    }
}