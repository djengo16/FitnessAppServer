using FitnessApp.Dto.Message;
using FitnessApp.Models;
using FitnessApp.Models.Repositories;

namespace FitnessApp.Services.Data
{
    public class MessagesService : IMessagesService
    {
        private readonly IRepository<Message> messagesStorage;

        public MessagesService(IRepository<Message> messagesStorage)
        {
            this.messagesStorage = messagesStorage;
        }
        public async Task CreateAsync(MessageResponseDTO responseMessage)
        {
            var message = new Message()
            {
                SenderId = responseMessage.SenderId,
                RecipientId = responseMessage.RecipientId,
                Body = responseMessage.Body,
                ConversationId = responseMessage.ConversationId,
            };

            await messagesStorage.AddAsync(message);
            await messagesStorage.SaveChangesAsync();

        }
    }
}
