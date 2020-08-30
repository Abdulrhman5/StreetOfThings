using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Transaction.BusinessLogic.RegistrationQueries
{
    public interface ITransactionGetter
    {
        Task<AllTransactionsListDto> GetAllTransactions();
        public Task<List<TransactionDto>> GetUserTransactions(string userId);


    }
}
