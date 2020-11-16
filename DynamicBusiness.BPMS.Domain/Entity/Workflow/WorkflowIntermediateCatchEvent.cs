using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DynamicBusiness.BPMS.Domain
{
    public class WorkflowIntermediateCatchEvent : WorkflowIOElement
    {
        public BPMNIntermediateCatchType IntermediateCatchType { get; set; }

        public static BPMNIntermediateCatchType ConvertElementToType(XElement element)
        {
            return element.Element("messageEventDefinition") != null ? WorkflowIntermediateCatchEvent.BPMNIntermediateCatchType.Message :
                element.Element("conditionalEventDefinition") != null ? WorkflowIntermediateCatchEvent.BPMNIntermediateCatchType.Conditional :
                element.Element("linkEventDefinition") != null ? WorkflowIntermediateCatchEvent.BPMNIntermediateCatchType.Link :
                element.Element("timerEventDefinition") != null ? WorkflowIntermediateCatchEvent.BPMNIntermediateCatchType.Timer :
                element.Element("signalEventDefinition") != null ? WorkflowIntermediateCatchEvent.BPMNIntermediateCatchType.Signal :
                  WorkflowIntermediateCatchEvent.BPMNIntermediateCatchType.None;
        }

        public enum BPMNIntermediateCatchType
        {
            None = 0,
            Message = 1,
            Link = 2,
            Signal = 3,
            Conditional = 4,
            Timer = 5,
        }

        public WorkflowIntermediateCatchEvent(string subtype, string id, string name, List<string> outgoings, List<string> incomings)
            : base(id, name, outgoings, incomings)
        {
            Enum.TryParse(subtype, out BPMNIntermediateCatchType bpmnIntermediateCatchType);
            IntermediateCatchType = bpmnIntermediateCatchType;
        }
    }
}
