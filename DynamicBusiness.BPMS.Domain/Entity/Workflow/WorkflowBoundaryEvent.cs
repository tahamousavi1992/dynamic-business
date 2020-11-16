using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace DynamicBusiness.BPMS.Domain
{
    public class WorkflowBoundaryEvent : WorkflowIOElement
    {
        public bool CancelActivity { get; set; }
        public BPMNBoundaryType BoundaryType { get; set; }
        public string AttachedToRef { get; set; }
        public static BPMNBoundaryType ConvertElementToType(XElement element)
        {
            return element.Element("messageEventDefinition") != null ? WorkflowBoundaryEvent.BPMNBoundaryType.Message :
                element.Element("escalationEventDefinition") != null ? WorkflowBoundaryEvent.BPMNBoundaryType.Escalation :
                element.Element("linkEventDefinition") != null ? WorkflowBoundaryEvent.BPMNBoundaryType.Link :
                element.Element("compensateEventDefinition") != null ? WorkflowBoundaryEvent.BPMNBoundaryType.Compensation :
                element.Element("signalEventDefinition") != null ? WorkflowBoundaryEvent.BPMNBoundaryType.Signal :
                element.Element("timerEventDefinition") != null ? WorkflowBoundaryEvent.BPMNBoundaryType.timer :
                element.Element("cancelEventDefinition") != null ? WorkflowBoundaryEvent.BPMNBoundaryType.cancel :
                element.Element("errorEventDefinition") != null ? WorkflowBoundaryEvent.BPMNBoundaryType.error :
                  WorkflowBoundaryEvent.BPMNBoundaryType.None;
        }

        public enum BPMNBoundaryType
        {
            None = 0,
            Message = 1,
            Escalation = 2,
            Link = 3,
            Compensation = 4,
            Signal = 5,
            timer = 6,
            cancel = 7,
            error = 8,
        }
        
        public WorkflowBoundaryEvent(string subtype, string id, string name,bool cancelActivity, string attachedToRef, List<string> outgoings, List<string> incomings)
            : base(id, name, outgoings, incomings)
        {
            Enum.TryParse(subtype, out BPMNBoundaryType bpmnBoundaryType);
            this.BoundaryType = bpmnBoundaryType;
            this.CancelActivity = cancelActivity;
            this.AttachedToRef = attachedToRef;
        }
    }
}
