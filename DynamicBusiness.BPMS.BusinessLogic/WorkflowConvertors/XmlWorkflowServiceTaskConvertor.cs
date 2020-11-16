using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DynamicBusiness.BPMS.Domain;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class XmlWorkflowServiceTaskConvertor : XmlWorkflowElementConvertor<WorkflowServiceTask>
    {
        public XmlWorkflowServiceTaskConvertor()
        {

        }

        public override WorkflowServiceTask ConvertFromXml(XElement element)
        {
            XElement multiInstance = element.Element("multiInstanceLoopCharacteristics");

            WorkflowServiceTask serviceTask = new WorkflowServiceTask(
               base.GetIdAttributeValue(element),
               base.GetNameAttributeValue(element),
               GetOutgoings(element),
               GetIncomings(element),
               multiInstance != null ? (multiInstance.Attribute("isSequential")?.Value.ToBoolObj() ?? false ?
               Domain.sysBpmsTask.e_MarkerTypeLU.Sequential : Domain.sysBpmsTask.e_MarkerTypeLU.NonSequential)
               : (Domain.sysBpmsTask.e_MarkerTypeLU?)null
               );

            return serviceTask;
        }

        public override XElement ConvertToXml(WorkflowServiceTask serviceTask)
        {
            XElement element = new XElement(XMLNodeName.serviceTask.ToString()
                    , new XAttribute(XMLAttributeName.id.ToString(), serviceTask.ID)
                    , new XAttribute(XMLAttributeName.name.ToString(), serviceTask.Name)
                );

            base.AddIncomingNodes<WorkflowStartEvent>(serviceTask.Incomings, element);
            base.AddOutgoingNodes<WorkflowStartEvent>(serviceTask.Outgoings, element);

            return element;
        }
    }
}
