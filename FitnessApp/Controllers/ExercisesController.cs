using FitnessApp.Common;
using FitnessApp.Dto.Exercises;
using FitnessApp.Services.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExercisesController : ControllerBase
    {
        private readonly IExercisesService exercisesService;

        public ExercisesController(IExercisesService exercisesService)
        {
            this.exercisesService = exercisesService;
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult<GetExerciseDetailsDTO>> Get(int id)
        {
            var exercise = exercisesService.GetExerciseDetails(id);

            if (exercise == null)
            {
                return BadRequest("Exercise not found!");
            }
            return Ok(exercise);
        }

        [HttpPost("create")]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> Create(CreateOrUpdateExerciseDTO exerciseInput)
        {
            if (ModelState.IsValid)
            {
                await exercisesService.CreateExerciseAsync(exerciseInput);
                return this.Ok();
            }
            else
            {
                return BadRequest(this.ModelState.Select(x => x.Value.Errors).ToList());
            }
        }
        [HttpPost("update")]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> Update(int exerciseId, CreateOrUpdateExerciseDTO exerciseInput)
        {
            if (!ModelState.IsValid)
            {
                await exercisesService.UpdateExerciseAsync(exerciseId, exerciseInput);
                return this.Ok();
            }
            else
            {
                return BadRequest(this.ModelState.Select(x => x.Value.Errors).ToList());
            }
        }
    }
}
