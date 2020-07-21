using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Transaction.Models
{
    public class Login : IEntity<Guid>
    {
        public Guid LoginId { get; set; }

        public string Token { get; set; }

        public string TokenId { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }

        public List<ObjectRegistration> RegistrationRecepiants { get; set; }

        public List<ObjectReceiving> ObjectReceivingRecepiants { get; set; }

        public List<ObjectReceiving> ObjectReceivingGivers { get; set; }

        public List<ObjectReturning> ObjectReturningLoaners { get; set; }

        public List<ObjectReturning> ObjectReturningLoanees { get; set; }

        public List<TransactionToken> IssuedTokens { get; set; }

        public Guid Id => LoginId;
    }
}
