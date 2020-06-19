using Catalog.Models;
using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.ApplicationLogic.CommentsCommands
{
    public interface IObjectCommentAdder
    {
        Task<CommandResult<ObjectComment>> AddComment(AddCommentDto comment);
    }
}
