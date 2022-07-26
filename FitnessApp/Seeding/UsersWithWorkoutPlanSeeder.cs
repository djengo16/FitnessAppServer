namespace FitnessApp.Seeding
{
    using FitnessApp.Data;
    using FitnessApp.Dto.Workouts;
    using FitnessApp.Models;
    using FitnessApp.Models.Enums;
    using FitnessApp.Seeding.Dto;
    using FitnessApp.Services.Data;
    using Microsoft.AspNetCore.Identity;

    public class UsersWithWorkoutPlanSeeder : ISeeder
    {
        private readonly ICollection<UserTrainingProgramCombinationDTO> combinations;

        private const string DefaultPassword = "asd123";

        public UsersWithWorkoutPlanSeeder()
        {
            combinations = new List<UserTrainingProgramCombinationDTO>();

        }
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var workoutsService = serviceProvider.GetRequiredService<IWorkoutsService>();


            if (dbContext.Users.Any())
            {
                return;
            }

            setTrainingProgramPersonalizeCombinations();

            int userCounter = 1;

            foreach (var combination in combinations)
            {
                var user = new ApplicationUser()
                {
                    Email = $"user{userCounter}@sample.com",
                    UserName = $"user{userCounter}",
                };

                var result = await userManager.CreateAsync(user, DefaultPassword);

                workoutsService.GenerateWorkoutPlans(new WorkoutGenerationInputModel
                {
                    Count = 1,
                    Days = combination.Days,
                    Difficulty = combination.Difficulty,
                    Goal = combination.Goal,
                    UserId = user.Id,
                });

                /**
                 * Since we generate and store training programs in WorkoutsService collection field
                 * we should get the one generated for the current user with [userCounter - 1]
                 * because we don't clear the collection
                 * */
                var workoutId = await workoutsService.SaveWorkoutPlanAsync(
                    workoutsService.GetGeneratedWorkoutPlans().ToList()[userCounter - 1]);

                //add plan to user
                var createdUser = dbContext.Users.FirstOrDefault(x => x.Id == user.Id);
                createdUser.WorkoutPlanId = workoutId;
                dbContext.Update(createdUser);
                await dbContext.SaveChangesAsync();

                userCounter++;
            }
        }
        /** Generates combinations of (3 possible options for each property) 
         * days,goal and difficulty => 3^3 -> 27
         * */
        private void setTrainingProgramPersonalizeCombinations()
        {
            foreach (var goal in Enum.GetNames(typeof(Goal)))
            {
                foreach (var difficulty in Enum.GetNames(typeof(Difficulty)))
                {
                    for (int days = 3; days <= 5; days++)
                    {
                        combinations.Add(new UserTrainingProgramCombinationDTO
                        {
                            Goal = Enum.Parse<Goal>(goal),
                            Difficulty = Enum.Parse<Difficulty>(difficulty),
                            Days = days,
                        });
                    }
                }
            }
        }
    }
}