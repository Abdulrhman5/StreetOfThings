﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Transaction.Service.Dtos
{
    public class AddReceivingResultDto
    {
        public Guid RegistrationId { get; set; }

        public DateTime ReceivedAtUtc { get; set; }

        public int ObjectId { get; set; }

        public TimeSpan? ShouldBeReturnedAfterReceving { get; set; }
    }
}
