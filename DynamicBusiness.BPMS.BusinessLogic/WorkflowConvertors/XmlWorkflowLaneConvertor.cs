using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DynamicBusiness.BPMS.Domain;
namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class XmlWorkflowLaneConvertor : XmlWorkflowElementConvertor<WorkflowLane>
    {
        private   IWorkflowElementConvertor<WorkflowChildLaneSet> _workflowChildLaneSetConvertor { get { return XmlWorkflowElementConvertorFactory<WorkflowChildLaneSet>.Create(); } }
        public XmlWorkflowLaneConvertor()
        {
            
        }

        public override WorkflowLane ConvertFromXml(XElement element)
        {
            WorkflowLane Lane = new WorkflowLane(
                base.GetIdAttributeValue(element),
                base.GetNameAttributeValue(element)
                );

            //flowNodeRef
            IEnumerable<XElement> WorkflowflowNodeRefList = element.Descendants(XMLNodeName.flowNodeRef.ToString());
            if (WorkflowflowNodeRefList != null)
            {
                foreach (XElement xflowNodeRef in WorkflowflowNodeRefList)
                {
                    Lane.AddFlowNodeRef(xflowNodeRef.Value);
                }
            }

            //childLaneSet
            IEnumerable<XElement> WorkflowChildLaneSetList = element.Descendants(XMLNodeName.childLaneSet.ToString());
            if (WorkflowChildLaneSetList != null)
            {
                foreach (XElement xflowNodeRef in WorkflowChildLaneSetList)
                {
                    WorkflowChildLaneSet workflowChildLaneSet = _workflowChildLaneSetConvertor.ConvertFromString(xflowNodeRef.ToString());
                    Lane.AddChildLaneSet(workflowChildLaneSet);
                }
            }
            return Lane;
        }

        public override XElement ConvertToXml(WorkflowLane workflowElement)
        {
            XElement element = new XElement(XMLNodeName.lane.ToString(),
                    new XAttribute(XMLAttributeName.id.ToString(), workflowElement.ID),
                    new XAttribute(XMLAttributeName.name.ToString(), workflowElement.Name)
                );

            //flowNodeRef
            if (workflowElement.FlowNodeRefs != null)
                foreach (string FlowNodeRef in workflowElement.FlowNodeRefs)
                {
                    element.Add(new XElement(XMLNodeName.flowNodeRef.ToString(), FlowNodeRef));
                }

            //ChildLaneSet
            foreach (WorkflowChildLaneSet WorkflowChildLaneSet in workflowElement.ChildLaneSets)
            {
                string strEvent = _workflowChildLaneSetConvertor.ConvertToString(WorkflowChildLaneSet);
                element.Add(XElement.Parse(strEvent));
            }

            return element;
        }
    }
}
