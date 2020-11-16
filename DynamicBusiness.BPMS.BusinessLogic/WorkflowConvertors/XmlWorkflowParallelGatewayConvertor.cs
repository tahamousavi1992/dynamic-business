using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DynamicBusiness.BPMS.Domain;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class XmlWorkflowParallelGatewayConvertor : XmlWorkflowElementConvertor<WorkflowParallelGateway>
    {
        public override WorkflowParallelGateway ConvertFromXml(XElement element)
        {
            string subType = BPMSUtility.GetXAttributeValue(element, "subType");

            WorkflowParallelGateway parallelGateway = new WorkflowParallelGateway(
                element.TryGetAttributeFromElement("default"),
                base.GetIdAttributeValue(element),
                base.GetNameAttributeValue(element),
                GetOutgoings(element),
                GetIncomings(element)
                );

            return parallelGateway;
        }

        public override XElement ConvertToXml(WorkflowParallelGateway workflowElement)
        {
            XElement element = new XElement(XMLNodeName.parallelGateway.ToString());
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
