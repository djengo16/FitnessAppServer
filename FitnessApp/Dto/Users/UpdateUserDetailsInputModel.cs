namespace FitnessApp.Dto.Users
{
    public class UpdateUserDetailsInputModel
    {
#nullable enable
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Description { get; set; }
        public int? PhoneNumber { get; set; }
    }
}