using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DynamicBusiness.BPMS.Domain;
namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class XmlWorkflowLaneSetConvertor : XmlWorkflowElementConvertor<WorkflowLaneSet>
    {
        private IWorkflowElementConvertor<WorkflowLane> _WorkflowLaneConvertor { get { return XmlWorkflowElementConvertorFactory<WorkflowLane>.Create(); } }
        public XmlWorkflowLaneSetConvertor()
        {

        }

        public override WorkflowLaneSet ConvertFromXml(XElement element)
        {
            WorkflowLaneSet LaneSet = new WorkflowLaneSet(
                base.GetIdAttributeValue(element)
                );

            //Lane
            IEnumerable<XElement> WorkflowLaneList = element.Descendants(XMLNodeName.lane.ToString());
            foreach (XElement xLane in WorkflowLaneList)
            {
                WorkflowLane Lane = _WorkflowLaneConvertor.ConvertFromString(xLane.ToString());
                LaneSet.AddLane(Lane);
            }

            return LaneSet;
        }

        public override XElement ConvertToXml(WorkflowLaneSet workflowElement)
        {
            XElement element = new XElement(XMLNodeName.laneSet.ToString(),
                    new XAttribute(XMLAttributeName.id.ToString(), workflowElement.ID)
                );

            //Lane
            foreach (WorkflowLane Lane in workflowElement.Lanes)
            {
                string strLane = _WorkflowLaneConvertor.ConvertToString(Lane);
                element.Add(XElement.Parse(strLane));
            }

            return element;
        }
    }
}
