namespace FitnessApp.Dto
{
    using System.ComponentModel.DataAnnotations;
    public class UserRegisterInputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Username should be at least 6 characters long!", MinimumLength = 6)]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(50, ErrorMessage = "Password should be at least 6 characters long!", MinimumLength = 6)]
        public string? Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match!")]
        public string ConfirmPassword { get; set; }
    }
}