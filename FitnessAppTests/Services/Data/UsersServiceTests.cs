using FitnessApp.Data;
using FitnessApp.Models;
using FitnessApp.Services.Data;
using FitnessApp.Tests.Helper;

namespace FitnessApp.Tests.Services.Data
{
    public class UsersServiceTests
    {
        private IUsersService usersService;
        private ApplicationDbContext dbContext;

        [SetUp]
        public void Setup()
        {
            this.usersService = ServiceHelper.GetRequiredService<IUsersService>();
            this.dbContext = ServiceHelper.GetRequiredService<ApplicationDbContext>();
            
            // Clearing the db because one db is used for all the test
            // and unexpected data from another test class could broke the tests
            this.dbContext.Database.EnsureDeleted();
            SeedUsers();
        }

        [Test]
        public async Task GetUserByIdAsyncWillGetTheCorrectUser()
        {
            //get random user from dbContext, then use the service
            var userToGet = dbContext.Users.OrderBy(x => Guid.NewGuid()).First();

            var resultUser = await this.usersService.GetUserByIdAsync(userToGet.Id);

            Assert.True(resultUser.Id == userToGet.Id);
            Assert.True(resultUser.FirstName == userToGet.FirstName);
            Assert.True(resultUser.Email == userToGet.Email);
        }

        [Test]
        public async Task GetUserByIdAsyncWillThrowExceptionOnInvalidUser()
        {
            //get random user from dbContext, then use the service
            Assert.ThrowsAsync<ArgumentException>(async () => await usersService.GetUserByIdAsync("invalid"));
        }

        [Test]
        public async Task GetUsersAsyncWillReturnUsersComplyingWithTheSearchParams()
        {
            //Db has users with firstnamae User [1-20],
            //Should match User 2, User 20
            string searchParams = "User 2";
            int take = 10;
            int skip = 0;
            int expectedCount = 2;

            var users = await usersService.GetUsersAsync(searchParams, take, skip);

            Assert.True(users.Count() == expectedCount);
            Assert.True(users.All(x => x.Name.Contains(searchParams)));
        }

        [Test]
        public async Task GetUsersAsyncWillReturnUsersComplyingWithTheSearchParamsAndPagination()
        {
            //Expect total 11 matches (User 1, User 10-19)
            //When we skip 7, the method should return the remaining 4 entities.
            string searchParams = "User 1";
            int take = 10;
            int skip = 7;
            int expectedCount = 4;

            var users = await usersService.GetUsersAsync(searchParams, take, skip);

            Assert.True(users.Count() == expectedCount);
            Assert.True(users.All(x => x.Name.Contains(searchParams)));
        }

        [Test]
        public void GetCountBySearchParamsWillReturnTheTotalCountOfUsersForThisSearchParams()
        {
            //Expect total 11 matches (User 1, User 10-19)
            string searchParams = "User 1";
            int expectedCount = 11;

            var resultCount = usersService.GetCountBySearchParams(searchParams);

            Assert.True(resultCount == expectedCount);
        }

        private void SeedUsers()
        {
            var users = new List<ApplicationUser>();
            for (int i = 1; i <= 20; i++)
            {
                ApplicationUser user = new ApplicationUser()
                {
                    Id = Guid.NewGuid().ToString(),
                    FirstName = $"User {i} firstname",
                    LastName = $"User {i} lastname",
                    Email = $"user{i}@sample.com"
                };
                users.Add(user);
            }

            dbContext.Users.AddRangeAsync(users);
            dbContext.SaveChanges();
        }

    }
}
