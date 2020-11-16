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
    public class XmlWorkflowUserTaskConvertor : XmlWorkflowElementConvertor<WorkflowUserTask>
    {
        public XmlWorkflowUserTaskConvertor()
        {
        }

        public override WorkflowUserTask ConvertFromXml(XElement element)
        {
            XElement multiInstance = element.Element("multiInstanceLoopCharacteristics");

            WorkflowUserTask userTask = new WorkflowUserTask(
               base.GetIdAttributeValue(element),
               base.GetNameAttributeValue(element),
               GetOutgoings(element),
               GetIncomings(element),
               multiInstance != null ? (multiInstance.Attribute("isSequential")?.Value.ToBoolObj() ?? false ?
               Domain.sysBpmsTask.e_MarkerTypeLU.Sequential : Domain.sysBpmsTask.e_MarkerTypeLU.NonSequential)
               : (Domain.sysBpmsTask.e_MarkerTypeLU?)null
               );

            return userTask;
        }

        public override XElement ConvertToXml(WorkflowUserTask userTask)
        {
            XElement element = new XElement(XMLNodeName.userTask.ToString()
                    , new XAttribute(XMLAttributeName.id.ToString(), userTask.ID)
                    , new XAttribute(XMLAttributeName.name.ToString(), userTask.Name)
                );

            base.AddIncomingNodes<WorkflowStartEvent>(userTask.Incomings, element);
            base.AddOutgoingNodes<WorkflowStartEvent>(userTask.Outgoings, element);

            return element;
        }

    }
}
