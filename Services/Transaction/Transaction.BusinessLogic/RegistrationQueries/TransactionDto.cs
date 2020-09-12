using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Transaction.BusinessLogic.RegistrationQueries
{
    public class TransactionDto
    {
        public Guid RegistrationId { get; set; }

        public Guid? ReceivingId { get; set; }

        public Guid? ReturnId { get; set; }

        public TransactionType TranscationType { get; set; }

        public DateTime RegistredAtUtc { get; set; }

        public DateTime? ReceivedAtUtc { get; set; }

        public DateTime? ReturenedAtUtc { get; set; }

        public TimeSpan? ShouldReturnAfter { get; set; }

        public float? HourlyCharge { get; set; }

        public TransactionStatus TransactionStatus { get; set; }

        public ReturnStatus ReturnStatus { get; set; }
        public string ReceiverId { get; set; }

        public string OwnerId { get; set; }
        public int ObjectId { get; set; }
    }

    public enum TransactionType
    {
        Free,
        Lending,
        Renting
    }

    public enum TransactionStatus
    {
        RegisteredOnly,
        Received,
        Returned,
        Canceled,
        Deleted
    };

    public enum ReturnStatus
    {
        NotTakenYet,
        NotDueYet,
        Returned,
        Delayed,
        PossibleTheft
    };
}
