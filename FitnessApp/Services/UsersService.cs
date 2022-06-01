namespace FitnessApp.Services
{
    using FitnessApp.Dto;
    using FitnessApp.Models;
    using FitnessApp.Models.Repositories;

    public class UsersService : IUsersService
    {
        private readonly IDeletableEntityRepository<ApplicationUser> usersRepository;
        public UsersService(IDeletableEntityRepository<ApplicationUser> usersRepository)
        {
            this.usersRepository = usersRepository;
        }
        public IEnumerable<UserDTO> GetUsers()
        {
            var users = usersRepository.AllAsNoTracking().Select(x => new UserDTO
            {
                Email = x.Email
            });

            return users;
        }
    }
}