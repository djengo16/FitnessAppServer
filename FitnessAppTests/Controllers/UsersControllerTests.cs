namespace FitnessApp.Tests.Controllers
{
    using System.Net;
    using System.Net.Http.Headers;
    using System.Net.Http.Json;
    using System.Security.Claims;
    using FitnessApp.Common;
    using FitnessApp.Dto.Users;
    using FitnessApp.Tests.Helper;

    [TestFixture]
    public class UsersControllerTests
    {
        private const string adminEmail = "admin@sample.com";
        private const string defaultPassword = "asd123";
        private HttpClient httpClient;

        [SetUp]
        public void Setup()
        {
            httpClient = HttpClientHelper.GetDefaultHttpClinet();
        }

        [Test]
        public async Task GetUsersWillRespondWithStatusCodeOkWhenCorrectParametersAreGiven()
        {
            int page = 1;
            int dataCount = 10;
            var response = await httpClient.GetAsync($"/users?page={page}&count={dataCount}");

            //var stringResult = await response.Content.ReadAsStringAsync();

            Assert.That(response.Content.Headers.ContentType.ToString(), Is.EqualTo("application/json; charset=utf-8"));
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
        }

        [Test]
        public async Task GetUsersWillRespondWithInternalServerErrorCodeWhenIncorrectParametersAreGiven()
        {
            int page = -1;
            int dataCount = 10;
            var response = await httpClient.GetAsync($"/users?page={page}&count={dataCount}");

            //var stringResult = await response.Content.ReadAsStringAsync();

            Assert.IsFalse(response.IsSuccessStatusCode);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.InternalServerError);
        }

        [Test]
        public async Task GetUserWorkoutPlanDetailsWillRespondSuccessfullyWhenTheRequestComesFromThePlanOwner()
        {
            var claims = await GetRandomUserClaimsAsync(httpClient, true);
            string userId = claims[1].Value;

            var planIdsResponse = await httpClient.GetAsync($"/users/workoutPlanIds?userId={userId}");

            //expected collection of strings (workout plan ids)
            var idsDTO = await planIdsResponse.Content.ReadFromJsonAsync<ICollection<string>>();

            //randomly get one of the ids
            var randomPlanId = idsDTO.ToList()[new Random().Next(0, idsDTO.Count)];
            var response = await httpClient.GetAsync($"/users/workoutPlan?userId={userId}&planId={randomPlanId}");

            Assert.IsTrue(response.IsSuccessStatusCode);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
        }

        [Test]
        public async Task GetUserWorkoutPlanDetailsWillRespondSuccessfullyWhenTheRequestComesFromTheAdministrator()
        {
            var authHelper = new UserAuthHelper(httpClient);

            // Register Administrator's token to the http request's headers
            // so we can simulate that he tries to get the resources from this endpoint
            await GetRandomUserClaimsAsync(httpClient, true, GlobalConstants.AdministratorRoleName);

            // Workout plan owner's id (another user).
            // We don't register his token to the headers,
            // just need to get his id, administrator is already registered.
            var planOwnerClaims = await GetRandomUserClaimsAsync(httpClient, false);
            string userId = planOwnerClaims[1].Value;

            var planIdsResponse = await httpClient.GetAsync($"/users/workoutPlanIds?userId={userId}");

            //expected collection of strings (workout plan ids)
            var idsDTO = await planIdsResponse.Content.ReadFromJsonAsync<ICollection<string>>();

            //randomly get one of the ids
            var randomPlanId = idsDTO.ToList()[new Random().Next(0, idsDTO.Count)];
            var response = await httpClient.GetAsync($"/users/workoutPlan?userId={userId}&planId={randomPlanId}");

            Assert.IsTrue(response.IsSuccessStatusCode);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
        }

        [Test]
        public async Task GetUserWorkoutPlanDetailsWillReturnUnauthorized()
        {
            string testUserId = "123";
            string testPlanId = "456";

            var response = await httpClient.GetAsync($"/users/workoutPlan?userId={testUserId}&planId={testPlanId}");

            Assert.IsFalse(response.IsSuccessStatusCode);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task GetUserActivePlanIdWillReturnUnauthorized()
        {
            string testUserId = "123";
            var response = await httpClient.GetAsync($"/users/activePlanId?userId={testUserId}");

            Assert.IsFalse(response.IsSuccessStatusCode);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task GetUserActivePlanIdWillRespondSuccessfullyWhenClientIsAuthorized()
        {
            var claims = await GetRandomUserClaimsAsync(httpClient, true);
            string userId = claims[1].Value;

            var response = await httpClient.GetAsync($"/users/activePlanId?userId={userId}");

            Assert.IsTrue(response.IsSuccessStatusCode);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
        }

        [Test]
        public async Task LoginWillLogUserWithTheRightCredentials()
        {
            string username = RandomizationHelper.GetRandomUserEmail();
            string password = defaultPassword;

            var authHelper = new UserAuthHelper(httpClient);

            var loginResponseModel = await authHelper.GetLoginResponseAsync(username, password);

            Assert.NotNull(loginResponseModel);
            Assert.That(loginResponseModel.TokenAsString.Length > 0);
         }

        [Test]
        public async Task LoginWilNotLogUserWithWrongCredentials()
        {
            var user = new UserLoginInputModel
            {
                Email = RandomizationHelper.GetRandomUserEmail(),
                Password = "wrongpass"
            };
            var response = await httpClient.PostAsJsonAsync("/users/login", user);

            Assert.False(response.IsSuccessStatusCode);
            Assert.That(response.StatusCode == HttpStatusCode.Unauthorized);
        }



        /// <summary>
        /// Gets and returns random user claims for testing cases.
        /// Registers Authorization Header complied with the given user.
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="role">Optional role parameter.</param>
        /// <returns></returns>
        private async Task<List<Claim>> GetRandomUserClaimsAsync(HttpClient httpClient, bool doRegisterToHeaders, string role = null)
        {
            UserAuthHelper authHelper = new UserAuthHelper(httpClient);

            string password = defaultPassword;
            string username;
            if (role == GlobalConstants.AdministratorRoleName)
            {
                username = adminEmail;
            }
            else
            {
                username = RandomizationHelper.GetRandomUserEmail();
            }

            // token & expiration date expected
            var loginResponseModel = await authHelper.GetLoginResponseAsync(username, password);

            if (doRegisterToHeaders)
            {
                // add token to headers
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", loginResponseModel.TokenAsString);
            }

            var claims = authHelper.GetClaims(loginResponseModel.TokenAsString);

            return claims;
        }
    }
}