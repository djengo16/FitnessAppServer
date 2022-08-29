﻿namespace FitnessApp.Services.SocketService
{
    using FitnessApp.Dto.Message;
    using FitnessApp.Services.Data;
    using Newtonsoft.Json;
    using System.Collections.Concurrent;
    using System.Net.WebSockets;
    using System.Text;

    public class WebSocketService : IWebSocketService
    {
        private readonly IMessagesService messagesService;
        private ConcurrentDictionary<string, List<WebSocket>> _sockets;

        public WebSocketService(IMessagesService messagesService)
        {
            _sockets = new ConcurrentDictionary<string,List<WebSocket>>();
            this.messagesService = messagesService;
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
                    //from byte array -> json -> to c# object
                    var message = DeserializeMessage(buffer, receiveResult.Count);
                    await messagesService.CreateAsync(message);

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

        private MessageResponseDTO DeserializeMessage(byte[] buffer, int resultCount)
        {
            char[] chars = new char[resultCount];
            Decoder d = Encoding.UTF8.GetDecoder();
            int charLen = d.GetChars(buffer, 0, resultCount, chars, 0);
            string json = new string(chars);

            MessageResponseDTO messageData = JsonConvert.DeserializeObject<MessageResponseDTO>(json);
            return messageData;
        }
    }
}
