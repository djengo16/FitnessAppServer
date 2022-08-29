namespace FitnessApp.AutoMapperProfiles
{
    using AutoMapper;
    using FitnessApp.Dto.Message;
    using FitnessApp.Models;

    public class MessageProfiles : Profile
    {
        public MessageProfiles()
        {
            CreateMap<Message, MessageDTO>();
        }
    }
}
