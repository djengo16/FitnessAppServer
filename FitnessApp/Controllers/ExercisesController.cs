using FitnessApp.Common;
using FitnessApp.Dto.Exercises;
using FitnessApp.Services.Data;
using FitnessApp.Services.ServiceConstants;
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
        [HttpGet]
        public ExercisesPageDTO Exercises(string search, int page, int count)
        {
            int skip = page != 1 ? (page - 1) * count : 0;
            var exercises = exercisesService.GetExercises(search, take: count, skip);

            var dto = new ExercisesPageDTO()
            {
                Exercises = exercises.ToList(),
                PagesCount = search != null
                    ? exercisesService.GetCountBySearchParams(search)
                    : exercisesService.GetCount()
            };
            return dto;
        }
        [HttpGet("{id:int}")]
        [Authorize]
        public ActionResult<ExerciseDTО> Get(int id)
        {
            var exercise = exercisesService.GetExerciseDetails(id);

            if (exercise == null)
            {
                return BadRequest(ErrorMessages.ExerciseNotFound);
            }
            return Ok(exercise);
        }

        [HttpPost("create")]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> Create(ExerciseInputDTO exercise)
        {
            if (ModelState.IsValid)
            {
                await exercisesService.CreateAsync(exercise);
                return this.Ok();
            }
            else
            {
                return BadRequest(this.ModelState.Select(x => x.Value.Errors).ToList());
            }
        }
        [HttpPut("update")]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> Update(ExerciseUpdateDTO exercise)
        {
            if (ModelState.IsValid)
            {
                await exercisesService.UpdateAsync(exercise);
                return this.Ok();
            }
            else
            {
                return BadRequest(this.ModelState.Select(x => x.Value.Errors).ToList());
            }
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> Delete(int id)
        {
            await exercisesService.Delete(id);
            return Ok();
        }
    }
}
