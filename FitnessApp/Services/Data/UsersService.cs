namespace FitnessApp.Services.Data
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
            usersRepository.Delete(user);
            await usersRepository.SaveChangesAsync();
        }

        public async Task<UserDetailsDTO> GetUserByIdAsync(string id)
        {
            var user =  await userManager.FindByIdAsync(id);
            return mapper.Map<UserDetailsDTO>(user);
        }

        public IEnumerable<UserDTO> GetUsers(string searchParams, int? take = null, int skip = 0)
        {
            var usersQueryModel = usersRepository
                .All().Where(x => !string.IsNullOrEmpty(searchParams) 
                ? x.Email.Contains(searchParams) : true);

            if (usersQueryModel.Count() < take)
            {
                take = usersQueryModel.Count();
            }

            usersQueryModel = 
                take.HasValue? usersQueryModel.Skip(skip).Take(take.Value) : usersQueryModel.Skip(skip);

            return usersQueryModel.ProjectTo<UserDTO>(mapper.ConfigurationProvider).ToList();
        }

        public int GetCount()

        {
            return this.usersRepository.All().Count();
        }

        public int GetCountBySearchParams(string searchParams)

        {
            return this.usersRepository.All().Where(entityt => entityt.Email.Contains(searchParams)).Count();
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