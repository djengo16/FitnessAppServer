namespace FitnessApp.AutoMapperProfiles
{
    using AutoMapper;
    using FitnessApp.Dto.WorkoutDays;
    using FitnessApp.Models;

    public class WorkoutDayProfiles : Profile
    {
        public WorkoutDayProfiles()
        {
            CreateMap<WorkoutDay, GeneratedWorkoutDayDTO>();
        }
    }
}
