namespace FitnessApp.Seeding
{
    using FitnessApp.Data;
    public interface ISeeder
    {
        Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider);
    }
}
