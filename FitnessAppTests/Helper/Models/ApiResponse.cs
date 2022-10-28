namespace FitnessApp.Tests.Helper.Models
{
    using System.Net;
    public class ApiResponse
    {
        public string? Type { get; set; }

        public string? Title { get; set; }

        public HttpStatusCode Status { get; set; }

        public string? TraceId { get; set; }

        public Dictionary<string, string[]> Errors { get; set; }
    }
}
