using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.Models
{
    public class User : IEntity<Guid>
    {
        public Guid UserId { get; set; }  

        public string UserName { get; set; }

        public string OriginalUserId { get; set; }

        public UserStatus Status { get; set; }

        public List<Login> Logins { get; set; }

        public List<Transaction> MeAsReceipient { get; set; }
        public Guid Id => UserId;
    }

    public enum UserStatus
    {
        Available,
        Deleted,
    }
}
