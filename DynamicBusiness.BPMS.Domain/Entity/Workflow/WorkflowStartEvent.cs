using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DynamicBusiness.BPMS.Domain
{
    public class WorkflowStartEvent : WorkflowIOElement
    {
        public BPMNStartEventType StartEventType { get; set; }
        public static BPMNStartEventType ConvertElementToType(XElement element)
        {
            return element.Element("messageEventDefinition") != null ? WorkflowStartEvent.BPMNStartEventType.Message :
                element.Element("timerEventDefinition") != null ? WorkflowStartEvent.BPMNStartEventType.Timer :
                element.Element("conditionalEventDefinition") != null ? WorkflowStartEvent.BPMNStartEventType.Conditional :
                element.Element("signalEventDefinition") != null ? WorkflowStartEvent.BPMNStartEventType.Signal :
                WorkflowStartEvent.BPMNStartEventType.None;
        }
        public enum BPMNStartEventType
        {
            None = 0,
            Message = 1,
            Timer = 2,
            Conditional = 3,
            Signal = 4,
        }

        public WorkflowStartEvent(string startEventType, string id, string name, List<string> outgoings, List<string> incomings)
            : base(id, name, outgoings, incomings)

        {
            if (Enum.TryParse(startEventType, true, out BPMNStartEventType bpmnStartEventType))
                StartEventType = bpmnStartEventType;
        }
    }
}
