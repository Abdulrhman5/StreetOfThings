using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Transaction.Core.Queries
{
    public class TransactionStatsOverYearQuery : IRequest<List<int>>
    {
    }
}
