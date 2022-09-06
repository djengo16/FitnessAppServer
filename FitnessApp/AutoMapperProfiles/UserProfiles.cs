namespace FitnessApp.AutoMapperProfiles
{
    using AutoMapper;
    using FitnessApp.Dto.Users;
    using FitnessApp.Models;

    public class UserProfiles : Profile
    {
        public UserProfiles()
        {
            CreateMap<ApplicationUser, UserDTO>()
                .ForMember(member => member.Name, opt => opt.MapFrom(target => $"{target.FirstName} {target.LastName}"));
            CreateMap<ApplicationUser, UserDetailsDTO>()
                .ForMember(member => member.RegisteredOn, opt => opt.MapFrom(target => target.CreatedOn.ToShortDateString()));
        }
    }
}