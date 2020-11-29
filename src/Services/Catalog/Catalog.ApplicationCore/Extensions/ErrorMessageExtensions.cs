using Catalog.ApplicationCore.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.ApplicationCore.Extensions
{
    public static class ErrorMessageExtensions
    {
        public static CommandResult<T> ToCommand<T>(this ErrorMessage errorMessage)
        {
            return new CommandResult<T>(errorMessage);
        }

        public static CommandResult ToCommand(this ErrorMessage errorMessage)
        {
            return new CommandResult(errorMessage);
        }
    }
}
