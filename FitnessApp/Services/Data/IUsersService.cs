namespace FitnessApp.Services.Data
{
    using FitnessApp.Dto.Users;
    using FitnessApp.Models;

    public interface IUsersService
    {
        IEnumerable<UserDTO> GetUsers(string searchParams, int? take = null, int skip = 0);
        int GetCount();
        string GetActiveWorkoutPlanId(string userId);
        int GetCountBySearchParams(string searchParams);
        Task<UserDetailsDTO> GetUserByIdAsync(string id);

        Task<string> UpdateUserDetailsAsync(UpdateUserDetailsInputModel model, string userId);
        Task UpdateProfilePictureAsync(string userId, string pictureUrl);

        Task DeleteUserAsync(string userId);
        Task AssignTrainingProgramToUser(string programId, string userId);
    }
}
