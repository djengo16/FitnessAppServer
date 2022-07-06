namespace FitnessApp.AutoMapperProfiles
{
    using AutoMapper;
    using FitnessApp.Dto.Workouts;
    using FitnessApp.Models;

    public class WorkoutPlanProfiles : Profile
    {
        public WorkoutPlanProfiles()
        {
            CreateMap<WorkoutPlan, GeneratedWorkoutPlanDTO>()
                .ForMember(x => x.WorkoutDays,y => y.MapFrom(t => t.WorkoutDays))
                .ForMember(x => x.DaysInWeek, y => y.MapFrom(t => t.WorkoutDays.Count));
        }
    }
}
