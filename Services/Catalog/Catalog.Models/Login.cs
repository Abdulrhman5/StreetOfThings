using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.Models
{
    public class Login : IEntity<Guid>
    {
        public Guid LoginId { get; set; }

        public string Token { get; set; }

        public string TokenId { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }

        public List<ObjectLoan> Loans { get; set; }

        public List<ObjectImpression> Impressions { get; set; }

        public List<ObjectComment> Comments { get; set; }

        public List<OfferedObject> OwnedObjects { get; set; }

        public List<ObjectLike> Likes { get; set; }

        public Guid Id => LoginId;
    }
}
