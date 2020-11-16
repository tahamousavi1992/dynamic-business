using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DynamicBusiness.BPMS.Domain;
namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class XmlWorkflowStartEventConvertor : XmlWorkflowElementConvertor<WorkflowStartEvent>
    {
        public override WorkflowStartEvent ConvertFromXml(XElement element)
        {
          
            WorkflowStartEvent startEvent = new WorkflowStartEvent(
                WorkflowStartEvent.ConvertElementToType(element).ToString(),
                base.GetIdAttributeValue(element),
                base.GetNameAttributeValue(element),
                GetOutgoings(element),
                GetIncomings(element)
                );

            return startEvent;
        }

        public override XElement ConvertToXml(WorkflowStartEvent workflowElement)
        {
            XElement element = new XElement(XMLNodeName.startEvent.ToString());
            element.AddAttributeToElement(XMLAttributeName.id.ToString(), workflowElement.ID);
            element.AddAttributeToElement(XMLAttributeName.name.ToString(), workflowElement.Name);

            base.AddIncomingNodes<WorkflowStartEvent>(workflowElement.Incomings, element);
            base.AddOutgoingNodes<WorkflowStartEvent>(workflowElement.Outgoings, element);

            return element;
        }
    }
}
