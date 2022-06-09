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
                .ForMember(x => x.Difficulty, y => y.MapFrom(t => t.Exercise.Difficulty));
        }
    }
}