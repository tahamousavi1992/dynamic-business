using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DynamicBusiness.BPMS.Domain;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class XmlWorkflowInclusiveGatewayConvertor : XmlWorkflowElementConvertor<WorkflowInclusiveGateway>
    {
        public override WorkflowInclusiveGateway ConvertFromXml(XElement element)
        {
            string subType = DomainUtility.GetXAttributeValue(element, "subType");

            WorkflowInclusiveGateway inclusiveGatewayGateway = new WorkflowInclusiveGateway(
                element.TryGetAttributeFromElement("default"),
                base.GetIdAttributeValue(element),
                base.GetNameAttributeValue(element),
                GetOutgoings(element),
                GetIncomings(element)
                );

            return inclusiveGatewayGateway;
        }

        public override XElement ConvertToXml(WorkflowInclusiveGateway workflowElement)
        {
            XElement element = new XElement(XMLNodeName.inclusiveGateway.ToString());
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
