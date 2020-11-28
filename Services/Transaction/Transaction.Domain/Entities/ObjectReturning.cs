using System;
using System.Collections.Generic;
using System.Text;

namespace Transaction.Domain.Entities
{
    public class ObjectReturning : BaseEntity<Guid>
    {
        public Guid ObjectReturningId { get; set; }

        public DateTime ReturnedAtUtc { get; set; }

        public Guid LoaneeLoginId { get; set; }
        public Login LoaneeLogin { get; set; }

        public Guid LoanerLoginId { get; set; }
        public Login LoanerLogin { get; set; }

        public Guid ObjectReceivingId { get; set; }
        public ObjectReceiving ObjectReceiving { get; set; }

        public override Guid Id => ObjectReturningId;
    }
}
