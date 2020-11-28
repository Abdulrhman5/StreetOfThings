using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Transaction.Domain.Entities;

namespace Transaction.Infrastructure.Services
{
    public interface IRemotlyObjectGetter 
    {
        Task<OfferedObject?> GetObject(int objectId);
    }
}
