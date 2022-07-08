namespace FitnessApp.AutoMapperProfiles
{
    using AutoMapper;
    using FitnessApp.Dto.Exercises;
    using FitnessApp.Models;

    public class ExerciseProfiles : Profile
    {
        public ExerciseProfiles()
        {
            CreateMap<Exercise, GetExerciseDetailsDTO>();
        }
    }
}