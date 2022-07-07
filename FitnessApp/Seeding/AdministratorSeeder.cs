namespace FitnessApp.Seeding
{
    using FitnessApp.Common;
    using FitnessApp.Data;
    using FitnessApp.Models;
    using Microsoft.AspNetCore.Identity;

    public class AdministratorSeeder : ISeeder
    {
        private const string AdminEmail = "admin@sample.com";
        private const string AdminUsername = "admin";
        private const string AdminPassword = "admin123";
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var adminUsers = await userManager.GetUsersInRoleAsync(GlobalConstants.AdministratorRoleName);

            if(adminUsers.Count == 0)
            {
                var user = new ApplicationUser
                {
                    Email = AdminEmail,
                    UserName = AdminUsername,
                };

                var result = await userManager.CreateAsync(user, AdminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, GlobalConstants.AdministratorRoleName);
                }
                
            }
        }
    }
}
