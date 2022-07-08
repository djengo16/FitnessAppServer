namespace FitnessApp.Services.Data
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using FitnessApp.Dto.Exercises;
    using FitnessApp.Models;
    using FitnessApp.Models.Repositories;
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
    }
}