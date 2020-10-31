using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Transaction.Service.ViewModels
{
    public class AddReceivingResultViewModel
    {
        public Guid RegistrationId { get; set; }

        public DateTime ReceivedAtUtc { get; set; }

        public int ObjectId { get; set; }

        public TimeSpan? ShouldBeReturnedAfterReceving { get; set; }
    }
}
