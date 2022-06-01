namespace FitnessApp.Services
{
    using FitnessApp.Dto;
    public interface IUsersService
    {
        IEnumerable<UserDTO> GetUsers();
    }
}
