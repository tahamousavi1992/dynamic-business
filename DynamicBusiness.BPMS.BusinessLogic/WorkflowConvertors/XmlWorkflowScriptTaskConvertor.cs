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
    public class XmlWorkflowScriptTaskConvertor : XmlWorkflowElementConvertor<WorkflowScriptTask>
    {
        public XmlWorkflowScriptTaskConvertor()
        {

        }

        public override WorkflowScriptTask ConvertFromXml(XElement element)
        {
            XElement multiInstance = element.Element("multiInstanceLoopCharacteristics");

            WorkflowScriptTask scriptTask = new WorkflowScriptTask(
               base.GetIdAttributeValue(element),
               base.GetNameAttributeValue(element),
               GetOutgoings(element),
               GetIncomings(element),
               multiInstance != null ? (multiInstance.Attribute("isSequential")?.Value.ToBoolObj() ?? false ?
               Domain.sysBpmsTask.e_MarkerTypeLU.Sequential : Domain.sysBpmsTask.e_MarkerTypeLU.NonSequential)
               : (Domain.sysBpmsTask.e_MarkerTypeLU?)null
               );

            return scriptTask;
        }

        public override XElement ConvertToXml(WorkflowScriptTask scriptTask)
        {
            XElement element = new XElement(XMLNodeName.scriptTask.ToString()
                    , new XAttribute(XMLAttributeName.id.ToString(), scriptTask.ID)
                    , new XAttribute(XMLAttributeName.name.ToString(), scriptTask.Name)
                );

            base.AddIncomingNodes<WorkflowStartEvent>(scriptTask.Incomings, element);
            base.AddOutgoingNodes<WorkflowStartEvent>(scriptTask.Outgoings, element);

            return element;
        }
    }
}
