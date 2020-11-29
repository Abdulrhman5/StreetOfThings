using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Transaction.Core.Dtos
{
    public class RegistrationDto
    {
        public Guid RegistrationId { get; set; }

        public Guid? ReceivingId { get; set; }

        public Guid? ReturnId { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public TransactionType TranscationType { get; set; }

        public DateTime RegistredAtUtc { get; set; }

        public DateTime? ReceivedAtUtc { get; set; }

        public DateTime? ReturenedAtUtc { get; set; }

        public TimeSpan? ShouldReturnAfter { get; set; }

        public float? HourlyCharge { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public TransactionStatus TransactionStatus { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
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
