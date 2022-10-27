using FitnessApp.Dto.Conversation;
using FitnessApp.Models;
using FitnessApp.Models.Repositories;
using AutoMapper.QueryableExtensions;
using AutoMapper;

namespace FitnessApp.Services.Data
{
    public class ConversationService : IConversationService
    {
        private readonly IRepository<Conversation> conversationStorage;
        private readonly IMapper mapper;

        public ConversationService(IRepository<Conversation> conversationStorage, IMapper mapper)
        {
            this.conversationStorage = conversationStorage;
            this.mapper = mapper;
        }

        /// <summary>
        /// Finds and returns the conversation if it exists othervise the method creates a new conversation
        /// and returns it.
        /// </summary>
        /// <param name="currentParticipantId"> The participant who will open the chat, the current logged user. </param>
        /// <param name="targetParticipantId"> The tartger or the user whom the chat is opened with. </param>
        /// <returns> Task of ConversationDetailsDTO </returns>
        public async Task<ConversationDetailsDTO> GetOrCreateAsync(string currentParticipantId, string targetParticipantId)
        {
            // Gets the conversation where the current and target user participates
            var conversation = conversationStorage
                .All()
                .Where(x => 
                       x.UserConversations.Any(x => x.UserId == currentParticipantId) 
                    && x.UserConversations.Any(x => x.UserId == targetParticipantId))
                .ProjectTo<ConversationDetailsDTO>(this.mapper.ConfigurationProvider)
                .FirstOrDefault();
            
            //If the conversation is not null -> return it
            if(conversation != null)
            {
                return conversation;
            }

            //If the conversation is null create a new one -> then return it
            string createdConvoId = await this.CreateAsync(currentParticipantId, targetParticipantId);
            conversation = mapper.Map<ConversationDetailsDTO>(GetById(createdConvoId));

            return conversation;
        }

        /// <summary>
        /// Creates a new conversation and adds the two participants to it.
        /// </summary>
        /// <param name="currentParticipantId"> The participant who will open the chat, the current logged user. </param>
        /// <param name="targetParticipantId"> The tartger or the user whom the chat is opened with. </param>
        /// <returns>The id of the new created conversation</returns>
        public async Task<string> CreateAsync(string currentParticipantId, string targetParticipantId)
        {
            var conversation = new Conversation();

            //the first user
            conversation.UserConversations.Add(new UserConversations(){
                UserId = currentParticipantId,
                ConversationId = conversation.Id,
            });

            //the second user
            conversation.UserConversations.Add(new UserConversations()
            {
                UserId = targetParticipantId,
                ConversationId = conversation.Id,
            });

           await conversationStorage.AddAsync(conversation);
           await conversationStorage.SaveChangesAsync();

            return conversation.Id;
        }

        public Conversation GetById(string id)
        {
            return conversationStorage.GetById(id);
        }

    }
}
