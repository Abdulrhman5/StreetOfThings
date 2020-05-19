using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.Models
{
    public class Login
    {
        public Guid LoginId { get; set; }

        public string Token { get; set; }

        public string TokenId { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public List<ObjectLoan> Loans { get; set; }
    }
}
