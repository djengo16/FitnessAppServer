namespace FitnessApp.AutoMapperProfiles
{
    using AutoMapper;
    using FitnessApp.Dto.Users;
    using FitnessApp.Models;

    public class UserProfiles : Profile
    {
        public UserProfiles()
        {
            CreateMap<ApplicationUser, UserDTO>();
            CreateMap<ApplicationUser, UserDetailsDTO>()
                .ForMember(member => member.RegisteredOn, opt => opt.MapFrom(target => target.CreatedOn.ToShortDateString()));
        }
    }
}