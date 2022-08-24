using FitnessApp.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Models
{
    public class Message : BaseModel<int>
    {
        [Required]
        public string SenderId { get; set; }
        public ApplicationUser Sender { get; set; }

        [Required]
        public string RecipientId { get; set; }
        public ApplicationUser Recipient { get; set; }

        [Required]
        public string Body { get; set; }
    }
}
