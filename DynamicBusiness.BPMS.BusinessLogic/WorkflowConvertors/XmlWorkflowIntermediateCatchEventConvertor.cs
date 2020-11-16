using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DynamicBusiness.BPMS.Domain;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class XmlWorkflowIntermediateCatchEventConvertor : XmlWorkflowElementConvertor<WorkflowIntermediateCatchEvent>
    {
        public override WorkflowIntermediateCatchEvent ConvertFromXml(XElement element)
        {

            WorkflowIntermediateCatchEvent endEvent = new WorkflowIntermediateCatchEvent(
                WorkflowIntermediateCatchEvent.ConvertElementToType(element).ToString(),
                base.GetIdAttributeValue(element),
                base.GetNameAttributeValue(element),
                GetOutgoings(element),
                GetIncomings(element)
                );

            return endEvent;
        }

        public override XElement ConvertToXml(WorkflowIntermediateCatchEvent workflowElement)
        {
            XElement element = new XElement(XMLNodeName.intermediateCatchEvent.ToString());
            element.AddAttributeToElement(XMLAttributeName.id.ToString(), workflowElement.ID);
            element.AddAttributeToElement(XMLAttributeName.name.ToString(), workflowElement.Name);

            base.AddIncomingNodes<WorkflowStartEvent>(workflowElement.Incomings, element);
            base.AddOutgoingNodes<WorkflowStartEvent>(workflowElement.Outgoings, element);

            return element;
        }
    }
}
