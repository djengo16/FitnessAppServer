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

        public ExercisesService(
            IDeletableEntityRepository<Exercise> exercisesStorage,
            IMapper mapper)
        {
            this.exercisesStorage = exercisesStorage;
            this.mapper = mapper;
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

        public async Task Delete(int id)
        {
            var exercise = GetById(id);

            if(exercise == null)
            {
                throw new ArgumentException(ErrorMessages.ExerciseNotFound);
            }

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