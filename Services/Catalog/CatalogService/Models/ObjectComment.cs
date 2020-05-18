using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogService.Models
{
    public class ObjectComment
    {
        public Guid CommentId { get; set; }

        public string Comment { get; set; }

        public User User { get; set; }

        public string UserId { get; set; }

    }
}
