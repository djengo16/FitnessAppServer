namespace FitnessApp.Dto.Users
{
    public class UserDetailsDTO
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string? Description { get; set; }
        public string? ProfilePicture { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? RegisteredOn { get; set; }
        public string? PhoneNumber { get; set; }
    }
}