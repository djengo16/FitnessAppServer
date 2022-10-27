using Microsoft.AspNetCore.Mvc.Testing;

namespace FitnessApp.Tests.Helper
{
    public static class HttpClientHelper
    {
        public static HttpClient GetDefaultHttpClinet()
        {
            var webAppFactory = new WebApplicationFactory<Program>();
            var httpClient = webAppFactory.CreateDefaultClient();

            return httpClient;
        }
    }
}
