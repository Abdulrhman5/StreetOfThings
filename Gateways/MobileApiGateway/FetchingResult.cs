using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileApiGateway
{
    public class FetchingResult<T>
    {
        private bool _successeded { get; set; }

        public bool IsSuccessful => _successeded;

        public T Result { get; } = default;

        public ErrorMessage Error { get; }



        public FetchingResult(T result)
        {
            Result = result ?? throw new ArgumentNullException();
            _successeded = true;
        }

        public FetchingResult(ErrorMessage message)
        {
            Error = message ?? throw new ArgumentNullException();
            _successeded = false;
        }
    }

    public class FetchingResult
    {
        private bool _successeded { get; set; }

        public bool IsSuccessful => _successeded;

        public ErrorMessage Error { get; }

        public FetchingResult()
        {
            _successeded = true;
        }

        public FetchingResult(ErrorMessage message)
        {
            Error = message ?? throw new ArgumentNullException();
            _successeded = false;
        }
    }

}
