using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.ApplicationCore.Dtos
{
    public class AddCommentDto
    {
        public int ObjectId { get; set; }

        public string Comment { get; set; }
    }
}
