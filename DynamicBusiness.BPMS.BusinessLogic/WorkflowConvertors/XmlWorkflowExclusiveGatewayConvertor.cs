using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DynamicBusiness.BPMS.Domain;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class XmlWorkflowExclusiveGatewayConvertor : XmlWorkflowElementConvertor<WorkflowExclusiveGateway>
    {
        public override WorkflowExclusiveGateway ConvertFromXml(XElement element)
        {
            string subType = DomainUtility.GetXAttributeValue(element, "subType");

            WorkflowExclusiveGateway exclusiveGateway = new WorkflowExclusiveGateway(
                element.TryGetAttributeFromElement("default"),
                base.GetIdAttributeValue(element),
                base.GetNameAttributeValue(element),
                GetOutgoings(element),
                GetIncomings(element)
                );

            return exclusiveGateway;
        }

        public override XElement ConvertToXml(WorkflowExclusiveGateway workflowElement)
        {
            XElement element = new XElement(XMLNodeName.exclusiveGateway.ToString());
            element.AddAttributeToElement(XMLAttributeName.id.ToString(), workflowElement.ID);
            element.AddAttributeToElement(XMLAttributeName.name.ToString(), workflowElement.Name);
            if (!string.IsNullOrWhiteSpace(workflowElement.Default))
                element.AddAttributeToElement("default", workflowElement.Default);
             
            base.AddIncomingNodes<WorkflowStartEvent>(workflowElement.Incomings, element);
            base.AddOutgoingNodes<WorkflowStartEvent>(workflowElement.Outgoings, element);

            return element;
        }
    }
}
