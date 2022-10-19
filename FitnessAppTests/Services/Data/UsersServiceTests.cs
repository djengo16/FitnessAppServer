using FitnessApp.Models;
using FitnessApp.Services.Data;
using FitnessApp.Tests.Helper;
using Microsoft.AspNetCore.Identity;

namespace FitnessApp.Tests.Services.Data
{
    public class UsersServiceTests
    {
        private IUsersService usersService;
        private UserManager<ApplicationUser> userManager;

        [SetUp]
        public void Setup()
        {
            this.usersService = ServiceHelper.GetRequiredService<IUsersService>();
            //this.userManager = ServiceHelper.GetRequiredService(UserManager<ApplicationUser>);
        }
    }
}
