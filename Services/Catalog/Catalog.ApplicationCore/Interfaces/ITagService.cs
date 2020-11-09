using Catalog.ApplicationCore.Dtos;
using Catalog.ApplicationCore.Entities;
using Catalog.ApplicationCore.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.ApplicationCore.Interfaces
{
    public interface ITagService
    {
        public Task<CommandResult<Tag>> AddTag(AddTagDto tag);

    }
}
