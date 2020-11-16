using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DynamicBusiness.BPMS.Domain
{
    public class WorkflowIntermediateThrowEvent : WorkflowIOElement
    {
        public BPMNIntermediateThrowType IntermediateThrowType { get; set; }

        public static BPMNIntermediateThrowType ConvertElementToType(XElement element)
        {
            return element.Element("messageEventDefinition") != null ? WorkflowIntermediateThrowEvent.BPMNIntermediateThrowType.Message :
                element.Element("escalationEventDefinition") != null ? WorkflowIntermediateThrowEvent.BPMNIntermediateThrowType.Escalation :
                element.Element("linkEventDefinition") != null ? WorkflowIntermediateThrowEvent.BPMNIntermediateThrowType.Link :
                element.Element("compensateEventDefinition") != null ? WorkflowIntermediateThrowEvent.BPMNIntermediateThrowType.Compensation :
                element.Element("signalEventDefinition") != null ? WorkflowIntermediateThrowEvent.BPMNIntermediateThrowType.Signal :
                  WorkflowIntermediateThrowEvent.BPMNIntermediateThrowType.None;
        }

        public enum BPMNIntermediateThrowType
        {
            None = 0,
            Message = 1,
            Escalation = 2,
            Link = 3,
            Compensation = 4,
            Signal = 5,
        }
        public WorkflowIntermediateThrowEvent(string subtype, string id, string name, List<string> outgoings, List<string> incomings)
            : base(id, name, outgoings, incomings)
        {
            Enum.TryParse(subtype, out BPMNIntermediateThrowType bpmnIntermediateThrowType);
            IntermediateThrowType = bpmnIntermediateThrowType;
        }
    }
}
