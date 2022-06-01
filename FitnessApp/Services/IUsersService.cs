﻿namespace FitnessApp.Services
{
    using FitnessApp.Dto;
    using FitnessApp.Models;

    public interface IUsersService
    {
        IEnumerable<UserDTO> GetUsers();

        Task<string> UpdateUserDetailsAsync(UpdateUserDetailsInputModel model, string userId);

        Task DeleteUserAsync(string userId);
    }
}