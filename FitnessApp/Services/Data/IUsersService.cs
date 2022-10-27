namespace FitnessApp.Services.Data
{
    using FitnessApp.Dto.Users;

    public interface IUsersService
    {
        Task<IEnumerable<UserDTO>> GetUsersAsync(string searchParams, int? take = null, int skip = 0);
        int GetCount();
        string GetActiveWorkoutPlanId(string userId);
        int GetCountBySearchParams(string searchParams);
        Task<UserDetailsDTO> GetUserByIdAsync(string id);

        Task<string> UpdateUserDetailsAsync(UpdateUserDetailsInputModel model, string userId);
        Task UpdateProfilePictureAsync(string userId, string pictureUrl);
        string GetProfilePictureUrl(string userId);

        Task DeleteUserAsync(string userId);
        Task AssignTrainingProgramToUser(string programId, string userId);
        Task AssignRoleAsync(string userId, string roleName);
        Task RemoveFromRoleAsync(string userId, string roleName);
        Task<string> GetRoleNameAsync(string userId);
        Task<string> GetEmailAsync(string userId);
        Task<string> GetNameAsync(string userId);
    }
}
