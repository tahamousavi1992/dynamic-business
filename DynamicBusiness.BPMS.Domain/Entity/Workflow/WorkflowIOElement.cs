using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public class WorkflowIOElement : WorkflowElement
    {
        public List<string> Outgoings { get; set; }

        public List<string> Incomings { get; set; }
        public WorkflowIOElement(string id, string name = "", List<string> outgoings = null, List<string> incomings = null)
            : base(id, name)
        {
            this.Outgoings = outgoings;
            this.Incomings = incomings;
        }
    }
}
