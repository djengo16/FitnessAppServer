namespace FitnessApp.Controllers
{
    using FitnessApp.Dto.Users;
    using FitnessApp.Dto.Workouts;
    using FitnessApp.Models;
    using FitnessApp.Services.Data;
    using FitnessApp.Services.ServiceConstants;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("[controller]")]
    public class WorkoutsController : ControllerBase
    {
        private readonly IWorkoutsService workoutsService;
        private readonly UserManager<ApplicationUser> userManager;

        public WorkoutsController(UserManager<ApplicationUser> userManager, IWorkoutsService workoutsService)
        {
            this.userManager = userManager;
            this.workoutsService = workoutsService;
        }

        [HttpGet("myplans")]
        public ICollection<UserWorkoutPlanInAllUserPlansDTO> MyPlans(string userId)
        {
            var plans = workoutsService.GetUserWorkoutPlans(userId);
            return plans;
        }

        [HttpPost("personalize")]
        [Authorize]
        public async Task<IActionResult> Personalize(WorkoutGenerationInputModel userInput)
        {
            var user = await userManager.FindByIdAsync(userInput.UserId);
            if(user == null)
            {
                throw new ArgumentException(ErrorMessages.UserWithIdDoNoNotExists);
            }
            workoutsService.GenerateWorkoutPlans(userInput);

            var workouts = workoutsService.GetGeneratedWorkoutPlans();

            return Ok(workouts);
        }

        [HttpPost("personalize/assign")]
        public async Task<IActionResult> AssignWorkout(GeneratedWorkoutPlanDTO generatedWorkoutPlanDTO)
        {
            var planId = await workoutsService.SaveWorkoutPlanAsync(generatedWorkoutPlanDTO);
            return Ok(planId);
        }
    }
}