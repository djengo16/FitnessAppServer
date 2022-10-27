namespace FitnessApp.Tests.Services.Data
{
    using FitnessApp.Data;
    using FitnessApp.Models;
    using FitnessApp.Services.Data;
    using FitnessApp.Tests.Helper;
    using Microsoft.EntityFrameworkCore;
    using NUnit;
    public class ConversationServiceTests
    {
        private IConversationService conversationService;
        private ApplicationDbContext dbContext;

        [SetUp]
        public void Setup()
        {
            this.conversationService = ServiceHelper.GetRequiredService<IConversationService>();
            this.dbContext = ServiceHelper.GetRequiredService<ApplicationDbContext>();

            //Clearing the db because one db is used and unexpected data could broke the tests
            dbContext.Database.EnsureDeleted();
            SeedUsers();
        }

        [Test]
        public async Task GetOrCreateAsyncWillCreateConversationIfItDoesntExist()
        {
            string testUserId;
            string anotherTestUserId;

            LoadTwoRandomUserIds(out testUserId, out anotherTestUserId);

            var conversation = await this.conversationService.GetOrCreateAsync(testUserId, anotherTestUserId);
            var result = this.conversationService.GetById(conversation.Id);

            Assert.IsNotNull(result);
            Assert.That(result.UserConversations.Any(user => user.UserId == testUserId));
            Assert.That(result.UserConversations.Any(user => user.UserId == anotherTestUserId));
        }

        [Test]
        public async Task GetOrCreateAsyncWillReturnExistingConversationWhenItsAlreadyCreated()
        {
            string testUserId;
            string anotherTestUserId;

            LoadTwoRandomUserIds(out testUserId,out anotherTestUserId);

            //first call
            await this.conversationService.GetOrCreateAsync(testUserId, anotherTestUserId);

            //second call
            var conversation = await this.conversationService.GetOrCreateAsync(testUserId, anotherTestUserId);
            var result = dbContext.Conversations
                .Include(x => x.UserConversations)
                .Where(x => x.Id == conversation.Id)
                .FirstOrDefault();

            //will have 1 conversation created
            var allConversations = dbContext.Conversations
                .Where(x => x.UserConversations.Any(x => x.UserId == testUserId) && x.UserConversations.Any(x => x.UserId == anotherTestUserId))
                .ToList();

            Assert.IsNotNull(conversation);
            Assert.That(allConversations.Count() == 1);
            Assert.That(result.UserConversations.Any(user => user.UserId == testUserId));
            Assert.That(result.UserConversations.Any(user => user.UserId == anotherTestUserId));
        }

        private void LoadTwoRandomUserIds(out string userOneId, out string userTwoId)
        {
            userOneId = dbContext.Users
                .OrderBy(x => Guid.NewGuid())
                .FirstOrDefault()?.Id;

            //needed because lambda expressions cannot accepts out, ref etc.
            string firstId = userOneId;

             userTwoId = dbContext.Users
                .Where(x => x.Id != firstId).OrderBy(x => Guid.NewGuid())
                .FirstOrDefault()?.Id;
        }

        private void SeedUsers()
        {
            var users = new List<ApplicationUser>();
            for (int i = 0; i < 10; i++)
            {
                ApplicationUser user = new ApplicationUser()
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = $"User {i}",
                    Email = $"user{i}@sample.com"
                };
                users.Add(user);
            }

            dbContext.Users.AddRangeAsync(users);
            dbContext.SaveChanges();
        }
    }
}
