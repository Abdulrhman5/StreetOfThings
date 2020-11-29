using NetTopologySuite.Geometries;
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

        public DateTime LoggedAt { get; set; }

        public DateTime? LoggedOutAt { get; set; }

        public Point? LoginLocation { get; set; }

        public string Imei { get; set; }

        public string UserId { get; set; }
        public AppUser User { get; set; }

        public Guid Id { get => LoginId; }
    }
}
