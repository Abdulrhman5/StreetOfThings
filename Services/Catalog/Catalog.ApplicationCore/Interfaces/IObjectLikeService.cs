using Catalog.ApplicationCore.Dtos;
using Catalog.ApplicationCore.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.ApplicationCore.Interfaces
{
    public interface IObjectLikeService
    {
        Task<CommandResult> AddLike(AddLikeDto addLikeDto);

        Task<CommandResult> Unlike(AddLikeDto removeLikeDto);
    }
}
