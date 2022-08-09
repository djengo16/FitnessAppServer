namespace FitnessApp.Controllers
{
    using FitnessApp.Services.Data;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationsService notificationsService;

        public NotificationsController(INotificationsService notificationsService)
        {
            this.notificationsService = notificationsService;
        }

        [HttpGet("trainingDay")]
        public async Task<IActionResult> Get(string id)
        {
            var notification = await notificationsService.GetTrainingNotificationAsync(id);
            return Ok(notification);
        }

        [HttpPut("view/{id:int}")]
        public async Task<IActionResult> View(int id)
        {
           await notificationsService.ViewNotificationAsync(id);
            return Ok();
        }

    }
}
