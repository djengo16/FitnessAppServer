namespace FitnessApp.Dto.Users
{
    public class UpdateUserDetailsInputModel
    {
        public string? Username { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? Email { get; set; }
        public string? Description { get; set; }
        public string? PhoneNumber { get; set; }
    }
}