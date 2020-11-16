using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DynamicBusiness.BPMS.Domain;
namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class XmlWorkflowSequenceFlowConvertor : XmlWorkflowElementConvertor<WorkflowSequenceFlow>
    {
        public override WorkflowSequenceFlow ConvertFromXml(XElement element)
        {
            WorkflowSequenceFlow Property = new WorkflowSequenceFlow(
                element.TryGetAttributeFromElement("targetRef"),
                element.TryGetAttributeFromElement("sourceRef"),
                base.GetIdAttributeValue(element),
                base.GetNameAttributeValue(element)
                );
            return Property;
        }

        public override XElement ConvertToXml(WorkflowSequenceFlow WorkflowSequenceFlow)
        {
            XElement element = new XElement(XMLNodeName.sequenceFlow.ToString(),
                    new XAttribute(XMLAttributeName.id.ToString(), WorkflowSequenceFlow.ID),
                    new XAttribute(XMLAttributeName.name.ToString(), WorkflowSequenceFlow.Name),
                    new XAttribute("targetRef", WorkflowSequenceFlow.TargetRef),
                    new XAttribute("sourceRef", WorkflowSequenceFlow.SourceRef)
                );
            return element;
        }
    }
}
