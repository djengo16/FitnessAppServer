using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace FitnessApp.Services.SocketService
{
    public class WebSocketService : IWebSocketService
    {
        private ConcurrentDictionary<string, List<WebSocket>> _sockets;

        public WebSocketService()
        {
            _sockets = new ConcurrentDictionary<string,List<WebSocket>>();
        }

        public async Task OnConnect(string socketId, WebSocket socket)
        {
            if(socketId == null)
            {
                socketId = Guid.NewGuid().ToString();
            }

            var currKey = _sockets.Keys.Where(x => x == socketId).FirstOrDefault();

            if(currKey == null)
            {
                var currSockets = new List<WebSocket>() { socket };
                _sockets.TryAdd(socketId, currSockets);
            }
            else
            {
                _sockets[currKey].Add(socket);
            }

            await Comunicate(socketId, socket);
        }

        private async Task Comunicate(string socketId, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            var receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!receiveResult.CloseStatus.HasValue)
            {
                foreach (var socket in _sockets[socketId])
                {
                        await socket.SendAsync(
                        new ArraySegment<byte>(buffer, 0, receiveResult.Count),
                        receiveResult.MessageType,
                        receiveResult.EndOfMessage,
                        CancellationToken.None);
                }

                receiveResult = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(
                receiveResult.CloseStatus.Value,
                receiveResult.CloseStatusDescription,
                CancellationToken.None);
        }
    }
}
