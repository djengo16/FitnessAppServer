﻿namespace FitnessApp.Controllers
{
    using FitnessApp.Common;
    using FitnessApp.Dto.Users;
    using FitnessApp.Models;
    using FitnessApp.Services.Data;
    using FitnessApp.Services.Security;
    using FitnessApp.Services.ServiceConstants;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;

    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUsersService usersService;
        private readonly IJwtService jwtService;
        private readonly IWorkoutsService workoutsService;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            IUsersService usersService,
            IJwtService JwtService, 
            IWorkoutsService workoutsService)
        {
            this.userManager = userManager;
            this.usersService = usersService;
            this.jwtService = JwtService;
            this.workoutsService = workoutsService;
        }
        /**
         * @params (search -> seach parameter, page -> current page, count -> data per page)
         */
        [HttpGet]
        public async Task<UsersPageDTO> Users(string search, int page, int count)
        {
            // GetUsersAsync(string searchParams, int? take = null, int skip = 0)
            // If we are on page 2 we should skip page - 1's data * data (count per page)
            int skip = page != 1 ? (page - 1) * count : 0;

            var users = await usersService.GetUsersAsync(search, take:count, skip);

            var dto = new UsersPageDTO()
            {
                Users = users.ToList(),
                TotalData = search != null
                ? usersService.GetCountBySearchParams(search)
                : usersService.GetCount()
            };

            return dto;
        }

        [HttpGet("workoutPlanIds")]
        public async Task<IActionResult> WorkoutPlanIds(string userId)
        {
            return Ok(workoutsService.GetUserWorkoutPlanIds(userId));
        }

        [HttpGet("{id}")]
        public async Task<UserDetailsDTO> UserDetails(string id)
        {
            var user =  await usersService.GetUserByIdAsync(id);
            return user;
        }
        
        [HttpGet("workoutPlan")]
        public IActionResult GetUserWorkoutPlanDetails(string userId, string planId)
        {
            string activeUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string activeUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (activeUserId != userId && activeUserRole != GlobalConstants.AdministratorRoleName)
            {
                return Unauthorized(ErrorMessages.AccesToPlanDenied);
            }

            var workout = workoutsService.GetUserWorkoutPlan(userId, planId);

            return Ok(workout);
        }

        [HttpGet("activePlanId")]
        [Authorize]
        public IActionResult GetUserActivePlanId(string userId)
        {
            var workoutId = usersService.GetActiveWorkoutPlanId(userId);

            return Ok(workoutId);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterInputModel userInput)
        {
            var existingUser = await userManager.FindByEmailAsync(userInput.Email);
            if(existingUser != null)
            {
                return Unauthorized(ErrorMessages.UserWithEmailAlreadyExists);
            }

            var user = new ApplicationUser()
            {
                Email = userInput.Email,
                UserName = userInput.Email,
                FirstName = userInput.FirstName,
                LastName = userInput.LastName,
            };

            if(userInput.Id != null)
            {
                user.Id = userInput.Id;
            }

            var result = await this.userManager.CreateAsync(user, userInput.Password);
            return this.Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginInputModel userLoginModel)
        {
            var user = await userManager.FindByEmailAsync(userLoginModel.Email);

            if (!(user != null && await userManager.CheckPasswordAsync(user, userLoginModel.Password)))
            {
                return Unauthorized(ErrorMessages.UserWithPasswordOrEmailNotExists);
            }

            var isInAdminRole = await userManager.IsInRoleAsync(user, GlobalConstants.AdministratorRoleName);
            
            var token = jwtService.GenerateToken(user, isInAdminRole);
            var tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);
            return Ok(new
            {
                tokenAsString,
                expiration = token.ValidTo
            });
        }

        [HttpPut("edit")]
        [Authorize]
        public async Task<IActionResult> Put(UpdateUserDetailsInputModel user)
        {
            var userInContextId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if(userInContextId != user.Id && role != GlobalConstants.AdministratorRoleName)
            {
                return Forbid();
            }

            await usersService.UpdateUserDetailsAsync(user);
            
            return this.Ok();
        }

        [HttpPut("assignToRole")]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> AssignToRole(UserUpdateRoleDTO user)
        {
            await usersService.AssignRoleAsync(user.UserId, user.Role);

            return Ok();
        }

        [HttpPut("removeFromRole")]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> RemoveFrom(UserUpdateRoleDTO user)
        {
            await usersService.RemoveFromRoleAsync(user.UserId, user.Role);

            return Ok();
        }

        [HttpPut("updatePicture")]
        public async Task<IActionResult> UpdateProfilePicture(UpdateUserPictureDTO userDto)
        {
            await usersService.UpdateProfilePictureAsync(userDto.UserId, userDto.PictureUrl);
            return this.Ok();
        }

        [HttpGet("getProfilePicture/{id}")]
        public string GetProfilePicture(string id)
        {
            var url = usersService.GetProfilePictureUrl(id);
            return url;
        }

        [HttpPut("changepassword")]
        [Authorize]
        public async Task<IActionResult> Put(ChangePasswordInputModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                BadRequest(string.Join(", ", errors));
            }

            var user = await userManager.GetUserAsync(User);

            var changePasswordResult = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (!changePasswordResult.Succeeded)
            {
                List<string> errors = new List<string>();
                foreach (var error in changePasswordResult.Errors)
                {
                    errors.Add(error.Description);
                }
                return BadRequest(string.Join(", ", errors));
            }

            return this.Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            // only user in the current context could delete his account
            if (userId != id && role != GlobalConstants.AdministratorRoleName)
            {
                return Forbid();
            }

            await usersService.HardDeleteUserAsync(id);
            return this.NoContent();
        }
    }
}