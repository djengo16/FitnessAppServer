using FitnessApp.Services.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExerciseInWorkoutDayController : ControllerBase
    {
        private readonly IExerciseInWorkoutDayService exerciseInWorkoutDayservice;

        public ExerciseInWorkoutDayController(IExerciseInWorkoutDayService exerciseInWorkoutDayservice)
        {
            this.exerciseInWorkoutDayservice = exerciseInWorkoutDayservice;
        }
        [HttpDelete("exercise/{exerciseId}/workoutDay/{workoutdayId}")]
        [Authorize]
        public async Task<IActionResult> Delete(int exerciseId, string workoutdayId)
        {
            await exerciseInWorkoutDayservice.DeleteExerciseInWorkoutDay(exerciseId, workoutdayId);
            return Ok();
        }
    }
}
