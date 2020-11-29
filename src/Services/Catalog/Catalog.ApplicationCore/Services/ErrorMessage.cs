using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Catalog.ApplicationCore.Services
{
    public class ErrorMessage
    {
        public string ErrorCode { get; set; }

        public string Message { get; set; }

        public HttpStatusCode StatusCode { get; set; }
    }

    public class CommandResult<T>
    {
        private bool _successeded { get; set; }

        public bool IsSuccessful => _successeded;

        public T Result { get; } = default;

        public ErrorMessage Error { get; }



        public CommandResult(T result)
        {
            Result = result ?? throw new ArgumentNullException();
            _successeded = true;
        }

        public CommandResult(ErrorMessage message)
        {
            Error = message ?? throw new ArgumentNullException();
            _successeded = false;
        }
    }

    public class CommandResult
    {
        private bool _successeded { get; set; }

        public bool IsSuccessful => _successeded;

        public ErrorMessage Error { get; }

        public CommandResult()
        {
            _successeded = true;
        }

        public CommandResult(ErrorMessage message)
        {
            Error = message ?? throw new ArgumentNullException();
            _successeded = false;
        }
    }
}
