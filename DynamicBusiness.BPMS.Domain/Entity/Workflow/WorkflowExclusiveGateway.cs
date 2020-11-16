using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public class WorkflowExclusiveGateway : WorkflowIOElement
    {
        public string Default { get; private set; }
        public WorkflowExclusiveGateway(string _default, string id, string name, List<string> outgoings, List<string> incomings)
            : base(id, name, outgoings, incomings)
        {
            this.Default = _default;
        }
    }
}
