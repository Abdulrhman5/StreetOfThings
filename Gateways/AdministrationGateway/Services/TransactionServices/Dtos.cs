using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Transactions;

namespace AdministrationGateway.Services.TransactionServices
{
    public class TransactionForUserDownstreamListDto
    {
        public int TheUserReservationsCount { get; set; }

        public int OtherUsersReservationsCount { get; set; }

        public List<TransactionDownstreamDto> Transactions { get; set; }
    }
    public class TransactionDownstreamDto
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

        public UserDto Owner { get; set; }

        public UserDto Receiver { get; set; }

        public TransactionObjectDto Object { get; set; }
    }

    public class TransactionObjectDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string PhotoUrl { get; set; }
    }

    public class AllTransactionsUpstreamListDto
    {
        public int ReservedTransactionsCount { get; set; }

        public int DeliveredTransactionsCount { get; set; }

        public int ReturnedTransactionsCount { get; set; }

        public List<TransactionUpstreamDto> Transactions { get; set; }
    }

    public class AllTransactionsDownstreamListDto
    {
        public int ReservedTransactionsCount { get; set; }

        public int DeliveredTransactionsCount { get; set; }

        public int ReturnedTransactionsCount { get; set; }

        public List<TransactionDownstreamDto> Transactions { get; set; }
    }


    public class TransactionUpstreamDto
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

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ReturnStatus
    {
        NotTakenYet,
        NotDueYet,
        Returned,
        Delayed,
        PossibleTheft
    };
}
