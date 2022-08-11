using FitnessApp.Dto.ExerciseInWorkoutDay;
using FitnessApp.Services.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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
        [HttpPut]
        public async Task<IActionResult> Add(AddExerciseInWorkoutDayDTO dto)
        {
            var result = await exerciseInWorkoutDayservice.AddExerciseInWorkoutDayAsync(dto);
            return Ok(result);
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
