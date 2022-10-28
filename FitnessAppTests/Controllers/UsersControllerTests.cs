namespace FitnessApp.Tests.Controllers
{
    using System.Net;
    using System.Net.Http.Json;
    using FitnessApp.Dto.Users;
    using FitnessApp.Services.ServiceConstants;
    using FitnessApp.Tests.Constants;
    using FitnessApp.Tests.Helper;
    using FitnessApp.Tests.Helper.Enum;
    using FitnessApp.Tests.Helper.Models;
    using Microsoft.AspNetCore.Mvc.Testing;

    [TestFixture]
    public class UsersControllerTests : IntegrationTest
    {
        [SetUp]
        public void Setup()
        {
            var appFactory = new WebApplicationFactory<Program>();
            _TestHttpClient = appFactory.CreateDefaultClient();
        }

        [Test]
        public async Task GetUsers_WillRespond_WithStatusCodeOkW_henCorrectParametersAreGiven()
        {
            int page = 1;
            int dataCount = 10;
            var response = await _TestHttpClient.GetAsync($"/users?page={page}&count={dataCount}");

            Assert.That(response.Content.Headers.ContentType?.ToString(), Is.EqualTo("application/json; charset=utf-8"));
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
        }

        [Test]
        public async Task GetUsers_WillRespond_WithInternalServerErrorCode_WhenIncorrectParametersAreGiven()
        {
            int page = -1;
            int dataCount = 10;
            var response = await _TestHttpClient.GetAsync($"/users?page={page}&count={dataCount}");

            //var stringResult = await response.Content.ReadAsStringAsync();

            Assert.IsFalse(response.IsSuccessStatusCode);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.InternalServerError);
        }

        [Test]
        public async Task GetUserWorkoutPlanDetailsWill_RespondSuccessfully_WhenTheRequest_ComesFromThePlanOwner()
        {
            string userId = await AutheticateAsync(RoleType.User);

            var planIdsResponse = await _TestHttpClient.GetAsync($"/users/workoutPlanIds?userId={userId}");

            //expected collection of strings (workout plan ids)
            var idsDTO = await planIdsResponse.Content.ReadFromJsonAsync<ICollection<string>>();

            //randomly get one of the ids
            var randomPlanId = idsDTO.ToList()[new Random().Next(0, idsDTO.Count)];
            var response = await _TestHttpClient.GetAsync($"/users/workoutPlan?userId={userId}&planId={randomPlanId}");

            Assert.IsTrue(response.IsSuccessStatusCode);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
        }

        [Test]
        public async Task GetUserWorkoutPlanDetails_WillRespondSuccessfully_WhenTheRequest_ComesFromTheAdministrator()
        {
            // Register Administrator's token to the http request's headers
            // so we can simulate that he tries to get the resources from this endpoint
            await AutheticateAsync(RoleType.Administrator);

            // Workout plan owner's id (another user).
            // We don't register his token to the headers,
            // just need to get his id, administrator is already registered.
            string userId = await LogAndGetUserId(RoleType.User);

            var planIdsResponse = await _TestHttpClient.GetAsync($"/users/workoutPlanIds?userId={userId}");

            //expected collection of strings (workout plan ids)
            var idsDTO = await planIdsResponse.Content.ReadFromJsonAsync<ICollection<string>>();

            //randomly get one of the ids
            var randomPlanId = idsDTO.ToList()[new Random().Next(0, idsDTO.Count)];
            var response = await _TestHttpClient.GetAsync($"/users/workoutPlan?userId={userId}&planId={randomPlanId}");

            Assert.IsTrue(response.IsSuccessStatusCode);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
        }

        [Test]
        public async Task GetUserWorkoutPlanDetails_WillReturnUnauthorized()
        {
            string testUserId = "123";
            string testPlanId = "456";

            var response = await _TestHttpClient.GetAsync($"/users/workoutPlan?userId={testUserId}&planId={testPlanId}");

            Assert.IsFalse(response.IsSuccessStatusCode);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task GetUserActivePlanId_WillReturnUnauthorized()
        {
            string testUserId = "123";
            var response = await _TestHttpClient.GetAsync($"/users/activePlanId?userId={testUserId}");

            Assert.IsFalse(response.IsSuccessStatusCode);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task GetUserActivePlanId_WillRespondSuccessfully_WhenClientIsAuthorized()
        {
            string userId = await AutheticateAsync(RoleType.User);

            var response = await _TestHttpClient.GetAsync($"/users/activePlanId?userId={userId}");
            var responseContect = await response.Content.ReadAsStringAsync();

            Assert.IsTrue(response.IsSuccessStatusCode);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
            Assert.That(responseContect.Length > 0);
        }

        [Test]
        public async Task Login_WillLogUser_WithTheRightCredentials()
        {
            var loginResponseModel = await GetLoginResponseModelAsync(RoleType.User);

            Assert.NotNull(loginResponseModel);
            Assert.That(loginResponseModel.TokenAsString.Length > 0);
         }

        [Test]
        public async Task Login_WilNotLogUser_WithWrongCredentials()
        {
            var user = new UserLoginInputModel
            {
                Email = RandomizationHelper.GetRandomUserEmail(),
                Password = TestConstants.TestWrongPass
            };
            var response = await _TestHttpClient.PostAsJsonAsync("/users/login", user);

            Assert.False(response.IsSuccessStatusCode);
            Assert.That(response.StatusCode == HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task Register_WillRegisterUserSuccessfully()
        {
            UserRegisterInputModel userRegisterModel = GetTestUserRegisterModel();

            var response = await _TestHttpClient.PostAsJsonAsync("/users/register", userRegisterModel);
            var asString = await response.Content.ReadAsStringAsync();

            Assert.That(response.IsSuccessStatusCode);
            Assert.That(asString != null);

            await AutheticateAsync(RoleType.TestUser);
            await ClearUserData();
        }

        [Test]
        public async Task Register_WillNotRegisterUser_IfAccauntExists()
        {
            UserRegisterInputModel userRegisterModel = GetTestUserRegisterModel();

            // successfull registration
            await _TestHttpClient.PostAsJsonAsync("/users/register", userRegisterModel);

            // unauthorized with error msg
            var response = await _TestHttpClient.PostAsJsonAsync("/users/register", userRegisterModel);
            var responseMessage = await response.Content.ReadAsStringAsync();

            Assert.False(response.IsSuccessStatusCode);
            Assert.That(response.StatusCode == HttpStatusCode.Unauthorized);
            Assert.That(responseMessage == ErrorMessages.UserWithEmailAlreadyExists);

            await AutheticateAsync(RoleType.TestUser);
            await ClearUserData();
        }

        [Test]
        public async Task EditUser_WillRespondWithSuccess_WhenItsTheUserFromContext()
        {
            UserRegisterInputModel userRegisterModel = GetTestUserRegisterModel();

            // register
            await _TestHttpClient.PostAsJsonAsync("/users/register", userRegisterModel);

            // auth with the user from context
            await this.AutheticateAsync(RoleType.TestUser);

            // update 
            var response = await _TestHttpClient.PutAsJsonAsync("/users/edit", GetTestUserUpdateModel());

            Assert.That(response.IsSuccessStatusCode);
            Assert.That(response.StatusCode == HttpStatusCode.OK);
            await ClearUserData();
        }

        [Test]
        public async Task EditUser_WillRespondWithSuccess_WhenItsTheAdministrator()
        {
            UserRegisterInputModel userRegisterModel = GetTestUserRegisterModel();

            // register
            await _TestHttpClient.PostAsJsonAsync("/users/register", userRegisterModel);

            // auth with administrator
            await this.AutheticateAsync(RoleType.Administrator);

            // update 
            var response = await _TestHttpClient.PutAsJsonAsync("/users/edit", GetTestUserUpdateModel());

            Assert.That(response.IsSuccessStatusCode);
            Assert.That(response.StatusCode == HttpStatusCode.OK);

            await ClearUserData();
        }

        [Test]
        public async Task EditUser_WillNotRespondWithSuccess_WhenItsNotTheUser_FromTheContext()
        {
            UserRegisterInputModel userRegisterModel = GetTestUserRegisterModel();

            // register
            await _TestHttpClient.PostAsJsonAsync("/users/register", userRegisterModel);

            // auth with another user
            await this.AutheticateAsync(RoleType.User);

            // update 
            var response = await _TestHttpClient.PutAsJsonAsync("/users/edit", GetTestUserUpdateModel());

            Assert.False(response.IsSuccessStatusCode);
            Assert.That(response.StatusCode == HttpStatusCode.Forbidden);

            await this.AutheticateAsync(RoleType.TestUser);
            await ClearUserData();
        }

        [Test]
        public async Task ChangePassword_WillRespondWithSucces_WithValidData()
        {
            UserRegisterInputModel userRegisterModel = GetTestUserRegisterModel();

            // register
            await _TestHttpClient.PostAsJsonAsync("/users/register", userRegisterModel);
            await this.AutheticateAsync(RoleType.TestUser);

            var response = await _TestHttpClient.PutAsJsonAsync("/users/changepassword", new ChangePasswordInputModel()
            {
                OldPassword = TestConstants.DefaultPassword,
                NewPassword = "newpass123",
                ConfirmPassword = "newpass123",
            });

            Assert.That(response.IsSuccessStatusCode);
            Assert.That(response.StatusCode == HttpStatusCode.OK);

            await ClearUserData();
        }

        [Test]
        public async Task ChangePassword_WillNotRespondWithSucces_WithInvalidData()
        {
            UserRegisterInputModel userRegisterModel = GetTestUserRegisterModel();

            // register
            await _TestHttpClient.PostAsJsonAsync("/users/register", userRegisterModel);
            await this.AutheticateAsync(RoleType.TestUser);

            var response = await _TestHttpClient.PutAsJsonAsync("/users/changepassword", new ChangePasswordInputModel()
            {
                OldPassword = TestConstants.DefaultPassword,
                NewPassword = "newpass",
                ConfirmPassword = "newpass1",
            });

            var responseMessage = await response.Content.ReadFromJsonAsync<ApiResponse>();
            var errorMessage = responseMessage?.Errors["ConfirmPassword"][0];

            Assert.False(response.IsSuccessStatusCode);
            Assert.That(response.StatusCode == HttpStatusCode.BadRequest);
            Assert.That(errorMessage == ErrorMessages.PasswordsDoNotMatch);

            await ClearUserData();
        }

        public async Task ClearUserData()
        {
            await _TestHttpClient.DeleteAsync($"/users/{TestConstants.TestUserId}");
        }

        private UserRegisterInputModel GetTestUserRegisterModel()
        {
            return new UserRegisterInputModel()
            {
                Id = TestConstants.TestUserId,
                Email = TestConstants.TestUserEmail,
                Password = TestConstants.DefaultPassword,
                FirstName = TestConstants.TestFirstName,
                LastName = TestConstants.TestLastName,
            };
        }

        private UpdateUserDetailsInputModel GetTestUserUpdateModel() 
        {
            return new UpdateUserDetailsInputModel()
            {
                Id = TestConstants.TestUserId,
                Email = TestConstants.TestUserEmail,
                FirstName = TestConstants.TestFirstName,
                LastName = TestConstants.TestLastName,
                PhoneNumber = 1111111111,
            };
        }
    }
}