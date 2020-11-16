using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public abstract class XmlWorkflowElementConvertor<TWorkflowElement> : IWorkflowElementConvertor<TWorkflowElement>
        where TWorkflowElement : IWorkflowElement
    {
        public TWorkflowElement ConvertFromString(string data) => ConvertFromXml(XElement.Parse(data));

        public string ConvertToString(TWorkflowElement node) => ConvertToXml(node).ToString();

        public abstract TWorkflowElement ConvertFromXml(XElement element);

        public abstract XElement ConvertToXml(TWorkflowElement workflowElement);

        public string GetNameAttributeValue(XElement element)
        {
            return BPMSUtility.GetXAttributeValue(element, XMLAttributeName.name.ToString());
        }

        public string GetIdAttributeValue(XElement element)
        {
            return BPMSUtility.GetXAttributeValue(element, XMLAttributeName.id.ToString());
        }

        public enum XMLNodeName
        {
            process,
            steps,
            step,
            incoming,
            outgoing,
            connectionStrings,
            connectionString,
            databases,
            entities,
            entity,
            property,
            properties,
            sequenceFlow,
            item,
            variable,
            items,
            sqlQuery,
            variables,
            variableEntity,
            startEvent,
            endEvent,
            intermediateThrowEvent,
            boundaryEvent,
            userTask,
            laneSet,
            lane,
            flowNodeRef,
            exclusiveGateway,
            childLaneSet,
            serviceTask,
            scriptTask,
            task,
            inclusiveGateway,
            parallelGateway,
            intermediateCatchEvent,
        }

        public enum XMLAttributeName
        {
            id,
            name,
        }

        public XmlWorkflowElementConvertor()
        {

        }

        public List<string> GetIncomings(XElement element)
        {
            var incomingElements = element.Descendants(XMLNodeName.incoming.ToString());
            List<string> incomings = new List<string>();

            foreach (XElement incomingElement in incomingElements)
            {
                string incoming = incomingElement.Value;
                incomings.Add(incoming);
            }
            return incomings;
        }
        public List<string> GetOutgoings(XElement element)
        {
            var outgoingElements = element.Descendants(XMLNodeName.outgoing.ToString());
            List<string> outgoings = new List<string>();

            foreach (XElement outgoingElement in outgoingElements)
            {
                string outgoing = outgoingElement.Value;
                outgoings.Add(outgoing);
            }
            return outgoings;
        }

        public void AddIncomingNodes<T>(List<string> Incomings, XElement element) where T : WorkflowIOElement
        {
            foreach (string _item in Incomings)
            {
                element.Add(new XElement(XMLNodeName.incoming.ToString(), _item));
            }
        }

        public void AddOutgoingNodes<T>(List<string> Outgoings, XElement element) where T : WorkflowIOElement
        {
            foreach (string _item in Outgoings)
            {
                element.Add(new XElement(XMLNodeName.outgoing.ToString(), _item));
            }
        }
    }
}
