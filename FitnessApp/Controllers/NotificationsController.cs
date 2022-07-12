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

        [HttpGet]
        public IActionResult Get(string id)
        {
            return Ok(notificationsService.IsTrainingDayNotification(id));
        }

    }
}
