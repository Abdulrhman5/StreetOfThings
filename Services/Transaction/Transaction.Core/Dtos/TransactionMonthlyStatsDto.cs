using System;
using System.Collections.Generic;
using System.Text;

namespace Transaction.Core.Dtos
{
    public class TransactionMonthlyStatsDto
    {
        public int Count { get; set; }

        public DateTime Day { get; set; }
    }
}
