using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Transactions;

namespace MobileApiGateway.Services.TransactionServices
{
    public class TransactionDownstreamDto
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

        public UserDto Owner { get; set; }

        public UserDto Receiver { get; set; }

        public TransactionObjectDto Object { get; set; }
    }

    public class TransactionObjectDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }


    public class TransactionUpstreamDto
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

        public string ReceiverId { get; set; }

        public string OwnerId { get; set; }
        public int ObjectId { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TransactionType
    {
        Free,
        Lending,
        Renting
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TransactionStatus
    {
        RegisteredOnly,
        Received,
        Returned,
        Canceled,
        Deleted
    };
}
