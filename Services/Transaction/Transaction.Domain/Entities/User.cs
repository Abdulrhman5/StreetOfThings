using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Transaction.Domain.Entities
{
    public class User : BaseEntity<Guid>
    {
        public Guid UserId { get; set; }  

        public string UserName { get; set; }

        public UserStatus Status { get; set; }

        public List<Login> Logins { get; set; }

        public override Guid Id => UserId;
    }

    public enum UserStatus
    {
        Available,
        Deleted,
    }
}
