using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdministrationGateway.Services.TransactionServices
{
    public class TransactionProfile : Profile
    {
        public TransactionProfile()
        {
            CreateMap<TransactionUpstreamDto, TransactionDownstreamDto>();
            CreateMap<AllTransactionsUpstreamListDto, AllTransactionsDownstreamListDto>();
        }
    }
}
