namespace FitnessApp.Services
{
    using FitnessApp.Data;
    using FitnessApp.Dto;

    public class UsersService : IUsersService
    {
        ApplicationDbContext dbContext;
        public UsersService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public IEnumerable<UserDTO> GetUsers()
        {
            return this.dbContext.Users.Select(x => new UserDTO
            {
                Email = x.Email
            }).ToList();
        }
    }
}
