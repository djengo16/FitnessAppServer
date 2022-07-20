namespace FitnessApp.Services.Data
{
    using FitnessApp.Dto.Users;
    using FitnessApp.Models;

    public interface IUsersService
    {
        IEnumerable<UserDTO> GetUsers(string searchParams, int? take = null, int skip = 0);
        int GetUsersCount();
        int GetUsersCountBySearchParams(string searchParams);
        Task<UserDetailsDTO> GetUserByIdAsync(string id);

        Task<string> UpdateUserDetailsAsync(UpdateUserDetailsInputModel model, string userId);

        Task DeleteUserAsync(string userId);
    }
}
