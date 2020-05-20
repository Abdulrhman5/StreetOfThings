using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.ApplicationLogic.ObjectCommands
{
    public class AddObjectDto
    {
        public string ObjectName { get; set; }

        public string Description { get; set; }

        public List<string> Tags { get; set; }

        public ObjectType Type { get; set; }

    }

    public enum ObjectType
    {
        Lending,
        Free
    }
}
