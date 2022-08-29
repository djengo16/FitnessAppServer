namespace FitnessApp.AutoMapperProfiles
{
    using AutoMapper;
    using FitnessApp.Dto.Conversation;
    using FitnessApp.Models;

    public class ConversationProfiles : Profile
    {
        public ConversationProfiles()
        {
            CreateMap<Conversation, ConversationDetailsDTO>();
        }
    }
}
