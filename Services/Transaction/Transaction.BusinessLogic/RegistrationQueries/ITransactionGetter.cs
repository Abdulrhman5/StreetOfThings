using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Transaction.BusinessLogic.RegistrationQueries
{
    public interface ITransactionGetter
    {
        public Task<List<TransactionDto>> GetUserTransactions(string userId);


    }
}
