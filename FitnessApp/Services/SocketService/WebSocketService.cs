namespace FitnessApp.Services.SocketService
{
    using FitnessApp.Dto.Message;
    using FitnessApp.Services.Data;
    using Newtonsoft.Json;
    using System.Collections.Concurrent;
    using System.Net.WebSockets;
    using System.Text;

    public class WebSocketService : IWebSocketService
    {
        private readonly IServiceProvider serviceProvider;
        private ConcurrentDictionary<string, List<WebSocket>> _sockets;

        public WebSocketService(IServiceProvider serviceProvider)
        {
            _sockets = new ConcurrentDictionary<string,List<WebSocket>>();
            this.serviceProvider = serviceProvider;
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

                //from byte array -> json -> to c# object     
                var message = DeserializeMessage(buffer, receiveResult.Count);

                //foreach the socket list with socketId key
                foreach (var socket in _sockets[socketId])
                {        
                    await socket.SendAsync(
                        new ArraySegment<byte>(buffer, 0, receiveResult.Count),
                        receiveResult.MessageType,
                        receiveResult.EndOfMessage,
                        CancellationToken.None);
                }

                //After sending the message to all the subscribed participants
                //we save the message to the db
                using (var scope = serviceProvider.CreateScope())
                {
                    var messagesService = scope.ServiceProvider.GetRequiredService<IMessagesService>();
                    await messagesService.CreateAsync(message);
                }

                //Waiting for a new message to continue the while cycle
                receiveResult = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            //Remove socket from the list and close the connection
            await OnClose(socketId, webSocket, receiveResult);
        }

        private MessageResponseDTO DeserializeMessage(byte[] buffer, int resultCount)
        {
            char[] chars = new char[resultCount];
            Decoder d = Encoding.UTF8.GetDecoder();
            int charLen = d.GetChars(buffer, 0, resultCount, chars, 0);
            string json = new string(chars);

            MessageResponseDTO messageData = JsonConvert.DeserializeObject<MessageResponseDTO>(json);
            return messageData;
        }

        private async Task OnClose(string socketId, WebSocket webSocket, WebSocketReceiveResult receiveResult) 
        {
            _sockets[socketId].Remove(webSocket);

            await webSocket.CloseAsync(
                receiveResult.CloseStatus.Value,
                receiveResult.CloseStatusDescription,
                CancellationToken.None);
        }
    }
}
