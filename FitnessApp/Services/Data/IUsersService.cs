namespace FitnessApp.Services.Data
{
    using FitnessApp.Dto.Users;
    using FitnessApp.Models;

    public interface IUsersService
    {
        IEnumerable<UserDTO> GetUsers();
        Task<UserDetailsDTO> GetUserByIdAsync(string id);

        Task<string> UpdateUserDetailsAsync(UpdateUserDetailsInputModel model, string userId);

        Task DeleteUserAsync(string userId);
    }
}
