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
                //.ForMember(dest => dest.MuscleGroup, opt => opt.MapFrom(src => src.MuscleGroup.ToString()))
                //.ForMember(dest => dest.Difficulty, opt => opt.MapFrom(src => src.Difficulty.ToString()));
        }
    }
}
