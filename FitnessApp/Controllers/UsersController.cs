namespace FitnessApp.Controllers
{
    using FitnessApp.Dto;
    using FitnessApp.Models;
    using FitnessApp.Services;
    using FitnessApp.Services.Security;
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

        [HttpGet]
        public IEnumerable<UserDTO> Users()
        {
            var users = usersService.GetUsers();

            return users;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterInputModel userInput)
        {
            var user = new ApplicationUser()
            {
                Email = userInput.Email,
                UserName = userInput.Username
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
                return Unauthorized();
            }

            var token = jwtService.GenerateToken(user);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
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

        //TODO: Fix
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await usersService.DeleteUserAsync(id);
            return this.NoContent();
        }
    }
}