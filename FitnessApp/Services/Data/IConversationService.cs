using FitnessApp.Dto.Conversation;
using FitnessApp.Models;

namespace FitnessApp.Services.Data
{
    public interface IConversationService
    {
        Task<ConversationDetailsDTO> GetOrCreateAsync(string currentParticipantId, string targetParticipantId);
        Task<string> CreateAsync(string currentParticipantId, string targetParticipantId);
        Conversation GetById(string id);
    }
}
