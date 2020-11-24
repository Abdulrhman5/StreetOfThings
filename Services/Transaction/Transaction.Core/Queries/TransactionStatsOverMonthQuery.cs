using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Transaction.Core.Dtos;

namespace Transaction.Core.Queries
{
    class TransactionStatsOverMonthQuery : IRequest<List<TransactionMonthlyStatsDto>>
    {
    }
}
