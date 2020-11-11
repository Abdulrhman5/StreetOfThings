using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.ApplicationCore.Dtos
{
    public class AddTagDto
    {
        public string TagName { get; set; }

        public string Discreption { get; set; }

        public IFormFile Photo { get; set; }

    }
}
