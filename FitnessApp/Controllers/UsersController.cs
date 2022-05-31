namespace FitnessApp.Controllers
{
    using FitnessApp.Dto;
    using FitnessApp.Models;
    using FitnessApp.Services;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUsersService usersService;
        public UsersController(UserManager<ApplicationUser> userManager, IUsersService usersService)
        {
            this.userManager = userManager;
            this.usersService = usersService;
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
    }
}