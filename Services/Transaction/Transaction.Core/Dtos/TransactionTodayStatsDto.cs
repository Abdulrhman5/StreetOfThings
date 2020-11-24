using System;
using System.Collections.Generic;
using System.Text;

namespace Transaction.Core.Dtos
{
    public class TransactionTodayStatsDto
    {
        public List<int> TransactionsOverToday { get; set; }

        public int LateReturn { get; set; }

        public int OnTimeReturn { get; set; }

        public int NotReturnedYet { get; set; }
    }
}
