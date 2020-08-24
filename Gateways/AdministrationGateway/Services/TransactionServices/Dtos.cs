using System;

namespace AdministrationGateway.Services.TransactionServices
{
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

        public TransactionUserDto Owner { get; set; }

        public TransactionUserDto Receiver { get; set; }

        public TransactionObjectDto Object { get; set; }
    }

    public class TransactionObjectDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }


    public class TransactionUserDto
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string PictureUrl { get; set; }
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
}
