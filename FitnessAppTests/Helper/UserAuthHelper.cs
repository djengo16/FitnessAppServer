namespace FitnessApp.Tests.Helper
{
    using FitnessApp.Dto.Users;
    using FitnessApp.Tests.Helper.Models;

    using System.Net.Http.Json;
    using System.Security.Claims;
    using System.IdentityModel.Tokens.Jwt;

    /// <summary>
    /// Initializes a new instance of UserClaimsHelp
    /// </summary>
    public class UserAuthHelper
    {
        private readonly HttpClient httpClient;

        public UserAuthHelper(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        /// <summary>
        /// Gets username and password, logs user with the external httpClient using specific
        /// address and returns object of type LoginResponse.
        /// </summary>
        /// <param name="userName">username</param>
        /// <param name="password">password</param>
        /// <returns>object of type LoginResponse</returns>
        public async Task<LoginResponse> GetLoginResponseAsync(string userName, string password)
        {
            var user = new UserLoginInputModel
            {
                Email = userName,
                Password = password
            };

            var res = await httpClient.PostAsJsonAsync("/users/login", user);

            if (!res.IsSuccessStatusCode) return null;

            var loginResponseModel = await res.Content.ReadFromJsonAsync<LoginResponse>();

            return loginResponseModel;
        }

        /// <summary>
        /// Decodes jwt token and returns list of claims.
        /// </summary>
        /// <param name="token"></param>
        /// <returns>List of Claim</returns>
        public List<Claim> GetClaims(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);

            return jwtSecurityToken.Claims.ToList();
        }
    }
}
