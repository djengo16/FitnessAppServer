using FitnessApp.Models.Common;
using FitnessApp.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessApp.Models
{
    public class Notification : IAuditInfo
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public bool IsViewed { get; set; }
        public NotificationType Type { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }

        /// <summary>
        /// Optional property, in case we need it for some page redirection.
        /// Example: We got a notification for unread message from random User,
        /// we need this id to redirect the current user to messages page with the user from whom we received the messages
        /// </summary>
        public string RedirectId { get; set; }

        [ForeignKey("Recipient")]
        public string RecipientId { get; set; }
        public ApplicationUser Recipient { get; set; }
    }
}
