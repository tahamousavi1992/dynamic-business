using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DynamicBusiness.BPMS.Domain;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class XmlWorkflowIntermediateThrowEventConvertor : XmlWorkflowElementConvertor<WorkflowIntermediateThrowEvent>
    {
        public override WorkflowIntermediateThrowEvent ConvertFromXml(XElement element)
        {

            WorkflowIntermediateThrowEvent endEvent = new WorkflowIntermediateThrowEvent(
                WorkflowIntermediateThrowEvent.ConvertElementToType(element).ToString(),
                base.GetIdAttributeValue(element),
                base.GetNameAttributeValue(element),
                GetOutgoings(element),
                GetIncomings(element)
                );

            return endEvent;
        }

        public override XElement ConvertToXml(WorkflowIntermediateThrowEvent workflowElement)
        {
            XElement element = new XElement(XMLNodeName.intermediateThrowEvent.ToString());
            element.AddAttributeToElement(XMLAttributeName.id.ToString(), workflowElement.ID);
            element.AddAttributeToElement(XMLAttributeName.name.ToString(), workflowElement.Name);

            base.AddIncomingNodes<WorkflowStartEvent>(workflowElement.Incomings, element);
            base.AddOutgoingNodes<WorkflowStartEvent>(workflowElement.Outgoings, element);

            return element;
        }
    }
}
