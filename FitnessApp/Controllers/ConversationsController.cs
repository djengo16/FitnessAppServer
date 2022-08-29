namespace FitnessApp.Controllers
{
    using FitnessApp.Dto.Conversation;
    using FitnessApp.Services.Data;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("[controller]")]
    public class ConversationsController : ControllerBase
    {
        private readonly IConversationService conversationService;

        public ConversationsController(IConversationService conversationService)
        {
            this.conversationService = conversationService;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentUserId"> The current logged user that opened the chat/conversation. </param>
        /// <param name="targetUserId"> The other participant in the conversation, that will receive the messages. </param>
        /// <returns></returns>
        [HttpGet("currentUser/{currentId}/targetUser/{targetId}")]
        public async Task<ActionResult<ConversationDetailsDTO>> GetConversation(string currentId, string targetId)
        {
            var conversation = await conversationService.GetOrCreateAsync(currentId, targetId);

            return Ok(conversation);
        }
    }
}
