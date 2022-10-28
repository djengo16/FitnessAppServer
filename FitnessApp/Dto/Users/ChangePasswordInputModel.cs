using FitnessApp.Services.ServiceConstants;
using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Dto.Users
{
    public class ChangePasswordInputModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required]
        [MinLength(6)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = ErrorMessages.PasswordsDoNotMatch)]
        public string ConfirmPassword { get; set; }
    }
}