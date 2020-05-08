using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models
{
    public class ConfirmationToken : IEntity<Guid>
    {

        public Guid ConfirmationTokenId { get; set; }

        public string ConfirmationCode { get; set; }
        
        public AppUser User { get; set; }

        [Required]
        public string UserId { get; set; }
            
        public string ConfirmationType { get; set; }
        public DateTime IssuedAtUtc { get; set; }

        public DateTime ExpiresAtUtc { get; set; }

        public Guid Id => ConfirmationTokenId;


        public const string ConfirmationCodeTypeEmail = "Email";
    }
}
