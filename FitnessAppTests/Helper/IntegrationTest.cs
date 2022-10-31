namespace FitnessApp.Tests.Helper
{
    using System.Net.Http.Json;
    using System.Security.Claims;
    using System.Net.Http.Headers;
    using System.IdentityModel.Tokens.Jwt;

    using FitnessApp.Dto.Users;
    using FitnessApp.Tests.Constants;
    using FitnessApp.Tests.Helper.Enum;
    using FitnessApp.Tests.Helper.Models;
    using FitnessApp.Services.ServiceConstants;
    using Microsoft.AspNetCore.Mvc.Testing;

    /// <summary>
    /// Provides common functionalities, which integration
    /// tests will use.
    /// </summary>
    public class IntegrationTest
    {
        /// <summary>
        /// Should be initialized in the subclass's setup method so every
        /// test can use different client. (this way we can decouple the tests)
        /// </summary>
        protected HttpClient _TestHttpClient;

        /// <summary>
        /// Logs user gets his token and registers this token to the
        /// authentication header of the httpclient.
        /// </summary>
        /// <param name="type">User could be normal user or administrator.</param>
        /// <returns>Authenticated user's id (string).</returns>
        protected async Task<string> AutheticateAsync(RoleType type)
        {
            var loginResponseModel = await GetLoginResponseModelAsync(type);

            _TestHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", loginResponseModel.TokenAsString);

            var userId = GetUserIdFromAuthToken(loginResponseModel.TokenAsString);

            return userId;
        }

        /// <summary>
        /// Makes post request to login endpoint, gets the result after success,
        /// returns login response model, which contains user authentication token,
        /// and expiration date.
        /// </summary>
        /// <param name="type">
        /// User could be normal user, administrator.
        /// Role of type TestUser indicates that the user will be deleted later.</param>
        /// <returns>Object of type LoginResponseModel</returns>
        /// <exception cref="ArgumentException">When we have unsuccessfull response.</exception>
        /// <exception cref="NullReferenceException">When user is not found.</exception>
        protected async Task<LoginResponse> GetLoginResponseModelAsync(RoleType type)
        {
            var user = new UserLoginInputModel();
            user.Password = TestConstants.DefaultPassword;

            switch (type)
            {
                case RoleType.User:
                    user.Email = RandomizationHelper.GetRandomUserEmail();
                    break;
                case RoleType.Administrator:
                    user.Email = TestConstants.AdministratorEmail;
                    break;
                case RoleType.TestUser:
                    user.Email = TestConstants.TestUserEmail;
                    break;
            }

            var res = await _TestHttpClient.PostAsJsonAsync("/users/login", user);

            if (!res.IsSuccessStatusCode)
            {
                throw new ArgumentException(ErrorMessages.SomethingWentWrong);
            }

            var loginResponseModel = await res.Content.ReadFromJsonAsync<LoginResponse>();

            if (loginResponseModel == null)
            {
                throw new NullReferenceException(ErrorMessages.UserNotFound);
            }

            return loginResponseModel;
        }

        /// <summary>
        /// Makes request to login endpoint gets user's claims
        /// and returns his id.
        /// </summary>
        /// <param name="type">User could be normal user or administrator.</param>
        /// <returns>User id (string)</returns>
        protected async Task<string> LogAndGetUserId(RoleType type)
        {
            var loginModel = await GetLoginResponseModelAsync(type);
            var userId = GetUserIdFromAuthToken(loginModel.TokenAsString);

            return userId;
        }

        /// <summary>
        /// Gets user claims as list based on auth token.
        /// </summary>
        /// <param name="authToken">Jwt token</param>sasd
        /// <returns>List of Claim</returns>
        protected List<Claim> GetUserClaims(string authToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(authToken);

            return jwtSecurityToken.Claims.ToList();
        }
        
        /// <summary>
        /// Returns user id based on auth token.
        /// </summary>
        /// <param name="authToken">Jwt token</param>
        /// <returns>User id (string)</returns>
        private string GetUserIdFromAuthToken(string authToken)
        {
            var claims = GetUserClaims(authToken);
            var userId = claims[1].Value;
            return userId;
        }
    }
}