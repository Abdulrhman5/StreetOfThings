using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.ApplicationCore.Dtos
{
    public class LoginDataDto
    {
        public string TokenId { get; set; }

        public string UserId { get; set; }

        public string AccessToken { get; set; }
    }
}
