namespace FitnessApp.Dto.Users
{
    using System.ComponentModel.DataAnnotations;
    public class UserRegisterInputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        [StringLength(200, ErrorMessage = "Password should be at least 6 characters long!", MinimumLength = 6)]
        public string Password { get; set; }
    }
}