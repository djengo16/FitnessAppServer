namespace FitnessApp.AutoMapperProfiles
{
    using AutoMapper;
    using FitnessApp.Dto.Exercises;
    using FitnessApp.Models;

    public class ExerciseProfiles : Profile
    {
        public ExerciseProfiles()
        {
            CreateMap<Exercise, ExerciseDTО>()
                .ForMember(dest => dest.MuscleGroup, opt => opt.MapFrom(src => src.MuscleGroup.ToString()))
                .ForMember(dest => dest.Difficulty, opt => opt.MapFrom(src => src.Difficulty.ToString()));
            CreateMap<Exercise, ExerciseInListDTO>()
                .ForMember(dest => dest.MuscleGroup, opt => opt.MapFrom(src => src.MuscleGroup.ToString()));
        }
    }
}