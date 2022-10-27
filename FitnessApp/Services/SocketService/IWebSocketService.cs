using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace FitnessApp.Services.SocketService
{
    public interface IWebSocketService
    {
        Task OnConnectAsync(string socketId, WebSocket socket);
    }
}
