namespace FitnessApp.Services.SocketService
{
    using FitnessApp.Dto.Message;
    using FitnessApp.Models;
    using FitnessApp.Services.Data;
    using Newtonsoft.Json;
    using System.Collections.Concurrent;
    using System.Net.WebSockets;
    using System.Text;

    public class WebSocketService : IWebSocketService
    {
        private readonly IServiceProvider serviceProvider;
        private ConcurrentDictionary<string, List<WebSocket>> _sockets;

        // key: communication Id / value: notifications
        private Dictionary<string, List<Notification>> _notifications;

        public WebSocketService(IServiceProvider serviceProvider)
        {
            _notifications = new Dictionary<string, List<Notification>>();
            _sockets = new ConcurrentDictionary<string, List<WebSocket>>();
            this.serviceProvider = serviceProvider;
        }

        public async Task OnConnect(string socketId, WebSocket socket)
        {
            if (socketId == null)
            {
                socketId = Guid.NewGuid().ToString();
            }

            var currKey = _sockets.Keys.Where(x => x == socketId).FirstOrDefault();

            if (currKey == null)
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
                await HandleMessageCreation(message);


                //Waiting for a new message to continue the while cycle
                receiveResult = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            //Remove socket from the list and close the connection
            await OnClose(socketId, webSocket, receiveResult);
        }

        private async Task HandleMessageCreation(MessageResponseDTO message)
        {
            using var scope = serviceProvider.CreateScope();

            //Create message
            var messagesService = scope.ServiceProvider.GetRequiredService<IMessagesService>();
            await messagesService.CreateAsync(message);

            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationsService>();


            var socket = this._sockets[message.ConversationId];

            //They both opened the chat, we don't have to notify none of them
            if(socket.Count > 1)
            {
                return;
            }

            //Check if we have notification created in _notifications field
            if (this.CheckIfNotificationExists(message.ConversationId, message.RecipientId))
            {
                return;
            }

            //Check the database
            if (notificationService.CheckUnreadMessageNotificationExistence(message.SenderId, message.RecipientId))
            {
                return;
            }

            //Since we passed the checks above and got here, this means that the notification does not
            //exists neither in the _notifications field nor in the DB and we have to create one
            
            //Get value if there is entry with this key othervise set null to notifications
            this._notifications.TryGetValue(message.ConversationId, out List<Notification> notifications);

            //If it's null add new list
            if(notifications == null)
            {
                this._notifications.TryAdd(message.ConversationId, new List<Notification>());
            }

            //Create new notification and add it to _notifications collection
            var unreadMessageNotification = await notificationService.CreateUnreadMessageNotification(message.RecipientId, message.SenderId);
            this._notifications[message.ConversationId].Add(unreadMessageNotification);
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

            _notifications.Remove(socketId);
        }

        /// <summary>
        /// Checks if notification exists in this _notifications field
        /// </summary>
        private bool CheckIfNotificationExists(string conversationId, string recipientId)
        {
            // gets the list of notification if record with conversationId exists

            // not at all
            // When the first message is sent and there is still no entry in the _notifications collection, return false
            if (!_notifications.ContainsKey(conversationId))
            {
                return false;
            }

            //We get the List of notification for this conversation
            var notifications = _notifications[conversationId];

            // not notification for the current recipient
            // When there is entry for this communication in the _notificaitons field
            // but this time the recipient is the other participant, return false
            foreach (var notification in notifications)
            {
                if (recipientId != notification.RecipientId)
                {
                    return false;
                }
            }

            //If the above checks are passed, this means that the 
            //notification already exists this class's _notifications field
            return true;
        }
    }
}
