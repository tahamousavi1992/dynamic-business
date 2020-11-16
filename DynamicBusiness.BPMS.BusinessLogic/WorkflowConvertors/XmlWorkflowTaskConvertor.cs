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
    public class XmlWorkflowTaskConvertor : XmlWorkflowElementConvertor<WorkflowTask>
    {
        public XmlWorkflowTaskConvertor()
        {

        }

        public override WorkflowTask ConvertFromXml(XElement element)
        {
            XElement multiInstance = element.Element("multiInstanceLoopCharacteristics");

            WorkflowTask task = new WorkflowTask(
               base.GetIdAttributeValue(element),
               base.GetNameAttributeValue(element),
               GetOutgoings(element),
               GetIncomings(element),
               multiInstance != null ? (multiInstance.Attribute("isSequential")?.Value.ToBoolObj() ?? false ?
               Domain.sysBpmsTask.e_MarkerTypeLU.Sequential : Domain.sysBpmsTask.e_MarkerTypeLU.NonSequential)
               : (Domain.sysBpmsTask.e_MarkerTypeLU?)null
               );

            return task;
        }

        public override XElement ConvertToXml(WorkflowTask task)
        {
            XElement element = new XElement(XMLNodeName.task.ToString()
                    , new XAttribute(XMLAttributeName.id.ToString(), task.ID)
                    , new XAttribute(XMLAttributeName.name.ToString(), task.Name)
                );

            base.AddIncomingNodes<WorkflowStartEvent>(task.Incomings, element);
            base.AddOutgoingNodes<WorkflowStartEvent>(task.Outgoings, element);

            return element;
        }
    }
}
