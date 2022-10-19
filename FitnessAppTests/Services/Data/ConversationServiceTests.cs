namespace FitnessApp.Tests.Services.Data
{
    using FitnessApp.Data;
    using FitnessApp.Services.Data;
    using FitnessApp.Tests.Helper;
    using Microsoft.EntityFrameworkCore;
    using NUnit;
    public  class ConversationServiceTests
    {
        private IConversationService conversationService;
        private ApplicationDbContext db;

        private const string testUserId = "123";
        private const string anotherTestUserId = "456";

        [SetUp]
        public void Setup()
        {
            this.conversationService = ServiceHelper.GetRequiredService<IConversationService>();
            this.db = ServiceHelper.GetRequiredService<ApplicationDbContext>();
        }

        [Test]
        public async Task GetOrCreateAsyncWillCreateConversationIfItDoesntExist()
        {
            var conversation = await this.conversationService.GetOrCreateAsync(testUserId, anotherTestUserId);
            var result = this.conversationService.GetById(conversation.Id);

            Assert.IsNotNull(result);
            Assert.That(result.UserConversations.Any(user => user.UserId == testUserId));
            Assert.That(result.UserConversations.Any(user => user.UserId == anotherTestUserId));
        }

        [Test]
        public async Task GetOrCreateAsyncWillReturnExistingConversationWhenItsAlreadyCreated()
        {
            //first call
            await this.conversationService.GetOrCreateAsync(testUserId, anotherTestUserId);

            //second call
            var conversation = await this.conversationService.GetOrCreateAsync(testUserId, anotherTestUserId);
            var result = db.Conversations
                .Include(x => x.UserConversations)
                .Where(x => x.Id == conversation.Id)
                .FirstOrDefault();//this.conversationService.GetById(conversation.Id);

            //will have 1 conversation created
            var allConversations = db.Conversations.ToList();

            Assert.IsNotNull(conversation);
            Assert.That(allConversations.Count() == 1);
            Assert.That(result.UserConversations.Any(user => user.UserId == testUserId));
            Assert.That(result.UserConversations.Any(user => user.UserId == anotherTestUserId));
        }
    }
}
