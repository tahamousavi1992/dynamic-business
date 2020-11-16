using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public sealed class NodeAttributeName : Attribute
    {
        public NodeAttributeName(string name)
        {
            this.Name = name;
        }
        public string Name { get; set; }
    }

    public sealed class NodeAttribute : Attribute
    {
        public NodeAttribute(string tagName)
        {
            this.TagName = tagName;
        }
        public string TagName { get; set; }
    }

    public sealed class NodeChildAttribute : Attribute
    {
        public NodeChildAttribute(string name)
        {
            this.Name = name;
        }
        public string Name { get; set; }
    }

    public sealed class NodeChildListAttribute : Attribute
    {
        public NodeChildListAttribute(string name)
        {
            this.Name = name;
        }
        public string Name { get; set; }
    }
}
