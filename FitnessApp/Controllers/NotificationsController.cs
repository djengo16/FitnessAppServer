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

        [HttpGet("all")]
        public async Task<IActionResult> GetAll(string id)
        {
          var notifications = notificationsService.GetAllByRecipientId(id);
            return Ok(notifications);
        }

        [HttpPut("view/{id:int}")]
        public async Task<IActionResult> View(int id)
        {
           await notificationsService.ViewNotificationAsync(id);
            return Ok();
        }

        [HttpPost("setup/trainingDay/{userId}/{activePlanId}")]
        public async Task<IActionResult> SetupTrainingDayNotification(string userId, string activePlanId)
        {
            await notificationsService.SetupTrainingDayNotification(userId, activePlanId);
            return Ok();
        }

    }
}
