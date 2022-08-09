namespace FitnessApp.AutoMapperProfiles
{
    using AutoMapper;
    using FitnessApp.Dto.Notification;
    using FitnessApp.Models;

    public class NotificationProfiles : Profile
    {
        public NotificationProfiles()
        {
            CreateMap<Notification, NotificationResponseDTO>();
        }
    }
}
