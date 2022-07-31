namespace FitnessApp.Controllers
{
    using FitnessApp.Common;
    using FitnessApp.Dto.Users;
    using FitnessApp.Models;
    using FitnessApp.Services.Data;
    using FitnessApp.Services.Security;
    using FitnessApp.Services.ServiceConstants;
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
        public UsersController(
            UserManager<ApplicationUser> userManager,
            IUsersService usersService,
            IJwtService JwtService)
        {
            this.userManager = userManager;
            this.usersService = usersService;
            this.jwtService = JwtService;
        }
        /**
         * @params (search -> seach parameter, page -> current page, count -> data per page)
         */
        [HttpGet]
        public UsersPageDTO Users(string search, int page, int count)
        {
            // GetUsers(string searchParams, int? take = null, int skip = 0)
            // If we are on page 2 we should skip page - 1's data * data (count per page)
            int skip = page != 1 ? (page - 1) * count : 0;

            var users = usersService.GetUsers(search, take:count, skip);

            var dto = new UsersPageDTO()
            {
                Users = users.ToList(),
                PagesCount = search != null
                ? usersService.GetCountBySearchParams(search)
                : usersService.GetCount()
            };

            return dto;
        }
        [HttpGet("{id}")]
        public async Task<UserDetailsDTO> UserDetails(string id)
        {
            var user =  await usersService.GetUserByIdAsync(id);
            return user;
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
                UserName = userInput.Email
            };

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
        public async Task<IActionResult> Put(UpdateUserDetailsInputModel user)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if(userId == null)
            {
                return Unauthorized();
            }

            await usersService.UpdateUserDetailsAsync(user, userId);
            
            return this.NoContent();
        }

        [HttpPut("changepassword")]
        public async Task<IActionResult> Put(ChangePasswordInputModel model)
        {
            var user = await userManager.GetUserAsync(User);

            await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            return this.NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await usersService.DeleteUserAsync(id);
            return this.NoContent();
        }
    }
}