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
            var muscleGroup = FindMuscleGroup(searchParams);

            var exercisesQueryModel = exercisesStorage
            .All().Where(x => !string.IsNullOrEmpty(searchParams)
                             ? (x.Name.Contains(searchParams) 
                             || (int)x.MuscleGroup == muscleGroup)
                             : true);

            if (exercisesQueryModel.Count() < take)
            {
                take = exercisesQueryModel.Count();
            }

            exercisesQueryModel =
                take.HasValue ? exercisesQueryModel.Skip(skip).Take(take.Value) : exercisesQueryModel.Skip(skip);

            return exercisesQueryModel.ProjectTo<ExerciseInListDTO>(mapper.ConfigurationProvider).ToList();
        }

        public int GetCount()
        {
            return this.exercisesStorage.AllAsNoTracking().Count();
        }

        public int GetCountBySearchParams(string searchParams)
        {
            var muscleGroup = FindMuscleGroup(searchParams);
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
            var exercise = this.exercisesStorage.All().FirstOrDefault(x => x.Id == exerciseId);

            exercise.Name = exerciseDTO.Name;
            exercise.Description = exerciseDTO.Description;
            exercise.MuscleGroup = exerciseDTO.MuscleGroup;
            exercise.Difficulty = exerciseDTO.Difficulty;
            exercise.PictureResourceUrl = exerciseDTO.PictureResourceUrl;
            exercise.VideoResourceUrl = exerciseDTO.VideoResourceUrl;

            await this.exercisesStorage.SaveChangesAsync();
        }
        private int FindMuscleGroup(string keywords)
        {
            if (keywords == null)
                return -1;
            foreach (string name in Enum.GetNames(typeof(MuscleGroup)))
            {
                if (name.ToUpper().Contains(keywords.ToUpper()))
                {
                    return (int)Enum.Parse(typeof(MuscleGroup), name);
                }
            }
            return -1;
        }
    }
}