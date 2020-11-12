using System;
using System.Collections.Generic;
using System.Text;

namespace Transaction.Service.Dtos
{
    public class AllTransactionsListDto
    {
        public int ReservedTransactionsCount { get; set; }

        public int DeliveredTransactionsCount { get; set; }

        public int ReturnedTransactionsCount { get; set; }

        public List<RegistrationDto> Transactions { get; set; }
    }
}
