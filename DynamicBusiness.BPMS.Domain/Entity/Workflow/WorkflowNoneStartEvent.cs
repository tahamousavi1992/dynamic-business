using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public class WorkflowNoneStartEvent : WorkflowStartEvent
    {
        public WorkflowNoneStartEvent(string startEventType, string id, string name, List<string> outgoings, List<string> incomings) 
            : base(BPMNStartEventType.None.ToString(), id, name, outgoings, incomings)
        {
        }
    }
}
