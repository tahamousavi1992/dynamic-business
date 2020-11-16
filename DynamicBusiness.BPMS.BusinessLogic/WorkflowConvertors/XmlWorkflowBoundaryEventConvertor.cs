using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DynamicBusiness.BPMS.Domain;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class XmlWorkflowBoundaryEventConvertor : XmlWorkflowElementConvertor<WorkflowBoundaryEvent>
    {
        public override WorkflowBoundaryEvent ConvertFromXml(XElement element)
        {

            WorkflowBoundaryEvent endEvent = new WorkflowBoundaryEvent(
                WorkflowBoundaryEvent.ConvertElementToType(element).ToString(),
                base.GetIdAttributeValue(element),
                base.GetNameAttributeValue(element),
                element.Attribute("cancelActivity") == null || element.Attribute("cancelActivity").Value == "true",
                element.Attribute("attachedToRef").Value,
                GetOutgoings(element),
                GetIncomings(element)
                );

            return endEvent;
        }

        public override XElement ConvertToXml(WorkflowBoundaryEvent workflowElement)
        {
            XElement element = new XElement(XMLNodeName.intermediateThrowEvent.ToString());
            element.AddAttributeToElement(XMLAttributeName.id.ToString(), workflowElement.ID);
            element.AddAttributeToElement(XMLAttributeName.name.ToString(), workflowElement.Name);
            element.AddAttributeToElement("cancelActivity", workflowElement.CancelActivity.ToStringObj().ToLower());
            element.AddAttributeToElement("attachedToRef", workflowElement.AttachedToRef);

            base.AddIncomingNodes<WorkflowStartEvent>(workflowElement.Incomings, element);
            base.AddOutgoingNodes<WorkflowStartEvent>(workflowElement.Outgoings, element);

            return element;
        }
    }
}
