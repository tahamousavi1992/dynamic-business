using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class XmlWorkflowChildLaneSetConvertor : XmlWorkflowElementConvertor<WorkflowChildLaneSet>
    {
        private IWorkflowElementConvertor<WorkflowLane> _WorkflowLaneConvertor { get { return XmlWorkflowElementConvertorFactory<WorkflowLane>.Create(); } }
        public XmlWorkflowChildLaneSetConvertor()
        {

        }

        public override WorkflowChildLaneSet ConvertFromXml(XElement element)
        {
            WorkflowChildLaneSet LaneSet = new WorkflowChildLaneSet(
                element.TryGetAttributeFromElement("type"),
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

        public override XElement ConvertToXml(WorkflowChildLaneSet workflowElement)
        {
            XElement element = new XElement(XMLNodeName.childLaneSet.ToString(),
                    new XAttribute(XMLAttributeName.id.ToString(), workflowElement.ID),
                    new XAttribute("type", workflowElement.Type)
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
