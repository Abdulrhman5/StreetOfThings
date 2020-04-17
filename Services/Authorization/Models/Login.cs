using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class Login: IEntity<Guid>
    {
        public Guid LoginId { get; set; }

        public string Token { get; set; }

        public bool IsValid { get; set; }

        public string IPAddress { get; set; }

        public string ClientAgent { get; set; }

        public string AdditionalInformation { get; set; }

        public DateTime IssuedAt { get; set; }

        public DateTime ExpiresAt { get; set; }

        public string UserId { get; set; }

        public Guid Id { get => LoginId; }
    }
}
