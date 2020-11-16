using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DynamicBusiness.BPMS.Domain
{
    public class WorkflowEndEvent : WorkflowIOElement
    {
        public BPMNEndEventType EndEventType { get; set; }

        public static BPMNEndEventType ConvertElementToType(XElement element)
        {
            return element.Element("messageEventDefinition") != null ? WorkflowEndEvent.BPMNEndEventType.Message :
                element.Element("escalationEventDefinition") != null ? WorkflowEndEvent.BPMNEndEventType.Escalation :
                element.Element("errorEventDefinition") != null ? WorkflowEndEvent.BPMNEndEventType.Error :
                element.Element("compensateEventDefinition") != null ? WorkflowEndEvent.BPMNEndEventType.Compensation :
                element.Element("signalEventDefinition") != null ? WorkflowEndEvent.BPMNEndEventType.Signal :
                element.Element("terminateEventDefinition") != null ? WorkflowEndEvent.BPMNEndEventType.Termination :
                WorkflowEndEvent.BPMNEndEventType.None;
        }

        public enum BPMNEndEventType
        {
            None = 0,
            Message = 1,
            Signal = 2,
            Error = 3,
            Escalation = 4,
            Termination = 6,
            Compensation = 7,
        }

        public WorkflowEndEvent(string subtype, string id, string name, List<string> outgoings, List<string> incomings)
            : base(id, name, outgoings, incomings)
        {
            Enum.TryParse(subtype, out BPMNEndEventType bpmnEndEventType);
            EndEventType = bpmnEndEventType;
        }
    }
}
