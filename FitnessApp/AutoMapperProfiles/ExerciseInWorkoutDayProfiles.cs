namespace FitnessApp.AutoMapperProfiles
{
    using AutoMapper;
    using FitnessApp.Dto.ExerciseInWorkoutDay;
    using FitnessApp.Models;
    public class ExerciseInWorkoutDayProfiles : Profile
    {
        public ExerciseInWorkoutDayProfiles()
        {
            CreateMap<ExerciseInWorkoutDay, GeneratedExerciseInWorkoutDayDTO>()
                .ForMember(x => x.Name, y => y.MapFrom(t => t.Exercise.Name))
                .ForMember(x => x.Difficulty, y => y.MapFrom(t => t.Exercise.Difficulty))
                .ForMember(x => x.Description, y => y.MapFrom(t => t.Exercise.Description))
                .ForMember(x => x.PictureResourceUrl, y => y.MapFrom(t => t.Exercise.PictureResourceUrl))
                .ForMember(x => x.VideoResourceUrl, y => y.MapFrom(t => t.Exercise.VideoResourceUrl));
        }
    }
}