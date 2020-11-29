using Catalog.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.ApplicationLogic.CommentsCommands
{
    public class AddCommentDto
    {
        public int ObjectId { get; set; }

        public string Comment { get; set; }
    }
}
