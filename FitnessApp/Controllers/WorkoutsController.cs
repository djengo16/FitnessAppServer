namespace FitnessApp.Controllers
{
    using FitnessApp.Dto.Users;
    using FitnessApp.Dto.Workouts;
    using FitnessApp.Services.Data;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("[controller]")]
    public class WorkoutsController : ControllerBase
    {
        private readonly IWorkoutsService workoutsService;

        public WorkoutsController(IWorkoutsService workoutsService)
        {
            this.workoutsService = workoutsService;
        }

        [HttpPost("personalize")]
        public async Task<IActionResult> Personalize(WorkoutGenerationInputModel userInput)
        {
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