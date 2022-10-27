using FitnessApp.Dto.ExerciseInWorkoutDay;
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

        [HttpPost]
        public async Task<IActionResult> Add(AddExerciseToExistingDayDTO dto)
        {
            var result = await exerciseInWorkoutDayservice.AddToExistingWorkoutDayAsync(dto);
            return Ok(result);
        }

        [HttpDelete("exercise/{exerciseId}/workoutDay/{workoutdayId}")]
        [Authorize]
        public async Task<IActionResult> Delete(int exerciseId, string workoutdayId)
        {
            await exerciseInWorkoutDayservice.DeleteAsync(exerciseId, workoutdayId);
            return Ok();
        }

        [HttpPut("updateRange")]
        public async Task<IActionResult> UpdateRange(ICollection<UpdateExerciseInWorkoutDayDTO> dtos)
        {
            await exerciseInWorkoutDayservice.UpdateRangeAsync(dtos);
            return Ok();
        }
    }
}