using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DynamicBusiness.BPMS.Domain;
namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class XmlWorkflowEndEventConvertor : XmlWorkflowElementConvertor<WorkflowEndEvent>
    {
        public override WorkflowEndEvent ConvertFromXml(XElement element)
        {

            WorkflowEndEvent endEvent = new WorkflowEndEvent(
                WorkflowEndEvent.ConvertElementToType(element).ToString(),
                base.GetIdAttributeValue(element),
                base.GetNameAttributeValue(element),
                GetOutgoings(element),
                GetIncomings(element)
                );

            return endEvent;
        }

        public override XElement ConvertToXml(WorkflowEndEvent workflowElement)
        {
            XElement element = new XElement(XMLNodeName.endEvent.ToString());
            element.AddAttributeToElement(XMLAttributeName.id.ToString(), workflowElement.ID);
            element.AddAttributeToElement(XMLAttributeName.name.ToString(), workflowElement.Name);

            base.AddIncomingNodes<WorkflowStartEvent>(workflowElement.Incomings, element);
            base.AddOutgoingNodes<WorkflowStartEvent>(workflowElement.Outgoings, element);

            return element;
        }
    }
}
