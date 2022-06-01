namespace FitnessApp.Controllers
{
    using FitnessApp.Dto;
    using FitnessApp.Models;
    using FitnessApp.Services;
    using FitnessApp.Settings;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;

    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUsersService usersService;
        private readonly IOptions<JwtSettings> jwtSettings;
        public UsersController(
            UserManager<ApplicationUser> userManager,
            IUsersService usersService,
            IOptions<JwtSettings> jwtSettings)
        {
            this.userManager = userManager;
            this.usersService = usersService;
            this.jwtSettings = jwtSettings;
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
            // Authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(this.jwtSettings.Value.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email, userLoginModel.Email.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(
                                 new SymmetricSecurityKey(key),
                                 SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

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

            await usersService.UpdateUserDetailsAsync(user, userId);
            
            return this.NoContent();
        }

        //TODO: Fix
        [HttpPut("changepassword")]
        public async Task<IActionResult> Put(ChangePasswordInputModel model)
        {
            var user = await userManager.GetUserAsync(User);

            if(user == null)
            {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

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