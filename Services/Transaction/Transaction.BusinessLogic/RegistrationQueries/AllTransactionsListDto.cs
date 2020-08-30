using System;
using System.Collections.Generic;
using System.Text;

namespace Transaction.BusinessLogic.RegistrationQueries
{
    public class AllTransactionsListDto
    {
        public int ReservedTransactionsCount { get; set; }

        public int DeliveredTransactionsCount { get; set; }

        public int ReturnedTransactionsCount { get; set; }

        public List<TransactionDto> Transactions { get; set; }
    }
}
