namespace FitnessApp.Services
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using FitnessApp.Dto.Users;
    using FitnessApp.Models;
    using FitnessApp.Models.Repositories;
    using Microsoft.AspNetCore.Identity;

    public class UsersService : IUsersService
    {
        private readonly IDeletableEntityRepository<ApplicationUser> usersRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMapper mapper;
        public UsersService(
            IDeletableEntityRepository<ApplicationUser> usersRepository,
            UserManager<ApplicationUser> userManager,
            IMapper mapper)
        {
            this.usersRepository = usersRepository;
            this.userManager = userManager;
            this.mapper = mapper;
        }

        public async Task DeleteUserAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            this.usersRepository.Delete(user);
            await this.usersRepository.SaveChangesAsync();
        }

        public IEnumerable<UserDTO> GetUsers()
        {
            var users = usersRepository
                .AllAsNoTracking()
                .ProjectTo<UserDTO>(this.mapper.ConfigurationProvider)
                .ToList();

            return users;
        }

        public async Task<string> UpdateUserDetailsAsync(UpdateUserDetailsInputModel model, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            user.UserName = model.Username;
            user.Email = model.Email;
            user.Description = model.Description;
            user.ProfilePicture = model.ProfilePictureUrl;
            user.PhoneNumber = model.PhoneNumber;

            await userManager.UpdateAsync(user);

            return userId;
        }
    }
}