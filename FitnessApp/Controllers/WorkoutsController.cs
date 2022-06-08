namespace FitnessApp.Controllers
{
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

        [HttpPost("generate")]
        public async Task<IActionResult> Generate(WorkoutGenerationInputModel userInput)
        {
            workoutsService.GenerateWorkoutPlan(userInput);
            return NoContent();
        }
    }
}