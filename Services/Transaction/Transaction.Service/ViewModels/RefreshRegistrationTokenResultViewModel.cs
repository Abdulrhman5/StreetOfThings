using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Transaction.Service.ViewModels
{
    public class RefreshRegistrationTokenResultViewModel
    {
        public string RegistrationToken { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime UseBeforeUtc { get; set; }

    }
}
