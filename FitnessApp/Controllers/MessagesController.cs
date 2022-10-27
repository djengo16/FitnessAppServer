namespace FitnessApp.Controllers
{
    using FitnessApp.Services.SocketService;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly IWebSocketService webSocketService;

        //private readonly WebSocketHandler socketHandler;

        public MessagesController(IWebSocketService webSocketService)
        {
            this.webSocketService = webSocketService;
        }
        [HttpGet("{id}")]
        public async Task Get(string id)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await webSocketService.OnConnectAsync(id, webSocket);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

    }
}
