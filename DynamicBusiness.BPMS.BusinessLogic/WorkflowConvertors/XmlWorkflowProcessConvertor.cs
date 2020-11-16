using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class XmlWorkflowProcessConvertor : XmlWorkflowElementConvertor<WorkflowProcess>
    {
        private readonly IWorkflowElementConvertor<WorkflowUserTask> _workflowProcessUserTasksConvertor;
        private readonly IWorkflowElementConvertor<WorkflowServiceTask> _workflowProcessServiceTasksConvertor;
        private readonly IWorkflowElementConvertor<WorkflowScriptTask> _workflowProcessScriptTasksConvertor;
        private readonly IWorkflowElementConvertor<WorkflowTask> _workflowProcessTasksConvertor;
        private readonly IWorkflowElementConvertor<WorkflowLaneSet> _workflowProcessLaneSetConvertor;
        private readonly IWorkflowElementConvertor<WorkflowSequenceFlow> _workflowFlowsConvertor;
        private readonly IWorkflowElementConvertor<WorkflowStartEvent> _workflowStartEventConvertor; 
        private readonly IWorkflowElementConvertor<WorkflowEndEvent> _workflowEndEventConvertor;
        private readonly IWorkflowElementConvertor<WorkflowIntermediateThrowEvent> _workflowIntermediateThrowEventConvertor;
        private readonly IWorkflowElementConvertor<WorkflowIntermediateCatchEvent> _workflowIntermediateCatchEventConvertor;
        private readonly IWorkflowElementConvertor<WorkflowBoundaryEvent> _workflowBoundaryEventConvertor;
        private readonly IWorkflowElementConvertor<WorkflowExclusiveGateway> _workflowExclusiveGatewayConvertor;
        private readonly IWorkflowElementConvertor<WorkflowInclusiveGateway> _workflowInclusiveGatewayConvertor;
        private readonly IWorkflowElementConvertor<WorkflowParallelGateway> _workflowParallelGatewayConvertor;
        public XmlWorkflowProcessConvertor()
        {
            _workflowProcessUserTasksConvertor = XmlWorkflowElementConvertorFactory<WorkflowUserTask>.Create();
            _workflowProcessServiceTasksConvertor = XmlWorkflowElementConvertorFactory<WorkflowServiceTask>.Create();
            _workflowProcessScriptTasksConvertor = XmlWorkflowElementConvertorFactory<WorkflowScriptTask>.Create();
            _workflowProcessTasksConvertor = XmlWorkflowElementConvertorFactory<WorkflowTask>.Create();
            _workflowProcessLaneSetConvertor = XmlWorkflowElementConvertorFactory<WorkflowLaneSet>.Create();
            _workflowFlowsConvertor = XmlWorkflowElementConvertorFactory<WorkflowSequenceFlow>.Create();
            _workflowStartEventConvertor = XmlWorkflowElementConvertorFactory<WorkflowStartEvent>.Create();
            _workflowEndEventConvertor = XmlWorkflowElementConvertorFactory<WorkflowEndEvent>.Create();
            _workflowIntermediateThrowEventConvertor = XmlWorkflowElementConvertorFactory<WorkflowIntermediateThrowEvent>.Create();
            _workflowIntermediateCatchEventConvertor = XmlWorkflowElementConvertorFactory<WorkflowIntermediateCatchEvent>.Create();
            _workflowBoundaryEventConvertor = XmlWorkflowElementConvertorFactory<WorkflowBoundaryEvent>.Create();
            _workflowExclusiveGatewayConvertor = XmlWorkflowElementConvertorFactory<WorkflowExclusiveGateway>.Create();
            _workflowInclusiveGatewayConvertor = XmlWorkflowElementConvertorFactory<WorkflowInclusiveGateway>.Create();
            _workflowParallelGatewayConvertor = XmlWorkflowElementConvertorFactory<WorkflowParallelGateway>.Create();
        }

        public override WorkflowProcess ConvertFromXml(XElement element)
        {
            WorkflowProcess process = new WorkflowProcess(
                base.GetIdAttributeValue(element),
                base.GetNameAttributeValue(element));

            // LaneSet
            this.ConvertFromXMLForLaneSet(process, element);

            // Tasks
            this.ConvertFromXMLForTasks(process, element);

            // SequenceFlows
            this.ConvertFromXMLForFlows(process, element);

            //Event
            this.ConvertFromXMLForEvents(process, element);

            //GateWay
            this.ConvertFromXMLForGateWays(process, element);

            return process;
        }
        private void ConvertFromXMLForLaneSet(WorkflowProcess process, XElement element)
        {
            //Get list UserTask
            IEnumerable<XElement> laneSetList = element.Descendants(XMLNodeName.laneSet.ToString());
            foreach (XElement xlaneSetL in laneSetList)
            {
                // Convertor                
                process.LaneSet = _workflowProcessLaneSetConvertor.ConvertFromString(xlaneSetL.ToString());
            }
        }

        private void ConvertFromXMLForFlows(WorkflowProcess process, XElement element)
        {
            // Get list SequenceFlows
            IEnumerable<XElement> flowsList = element.Descendants(XMLNodeName.sequenceFlow.ToString());
            foreach (XElement xFlow in flowsList)
            {
                // Convertor                
                WorkflowSequenceFlow Sequenceflow = _workflowFlowsConvertor.ConvertFromString(xFlow.ToString());
                process.AddFlow(Sequenceflow);
            }
        }

        private void ConvertFromXMLForTasks(WorkflowProcess process, XElement element)
        {
            //Get list UserTask
            IEnumerable<XElement> userTaskList = element.Descendants(XMLNodeName.userTask.ToString());
            foreach (XElement xTask in userTaskList)
            {
                // Convertor                
                WorkflowUserTask userTask = _workflowProcessUserTasksConvertor.ConvertFromString(xTask.ToString());
                process.AddTask(userTask);
            }
            //Get list ServiceTask
            IEnumerable<XElement> serviceTaskList = element.Descendants(XMLNodeName.serviceTask.ToString());
            foreach (XElement xTask in serviceTaskList)
            {
                // Convertor                
                WorkflowServiceTask serviceTask = _workflowProcessServiceTasksConvertor.ConvertFromString(xTask.ToString());
                process.AddTask(serviceTask);
            }
            //Get list ScriptTask
            IEnumerable<XElement> scriptTaskList = element.Descendants(XMLNodeName.scriptTask.ToString());
            foreach (XElement xTask in scriptTaskList)
            {
                // Convertor                
                WorkflowScriptTask scriptTask = _workflowProcessScriptTasksConvertor.ConvertFromString(xTask.ToString());
                process.AddTask(scriptTask);
            }
            //Get list Task
            IEnumerable<XElement> taskList = element.Descendants(XMLNodeName.task.ToString());
            foreach (XElement xTask in taskList)
            {
                // Convertor                
                WorkflowTask task = _workflowProcessTasksConvertor.ConvertFromString(xTask.ToString());
                process.AddTask(task);
            }
        }

        private void ConvertFromXMLForEvents(WorkflowProcess process, XElement element)
        {
            // Get list Events
            IEnumerable<XElement> EventsList = element.Descendants(XMLNodeName.startEvent.ToString());
            foreach (XElement xEvent in EventsList)
            {
                WorkflowStartEvent Event = _workflowStartEventConvertor.ConvertFromString(xEvent.ToString());
                process.AddEvent(Event);
            }

            EventsList = element.Descendants(XMLNodeName.endEvent.ToString());
            foreach (XElement xEvent in EventsList)
            {
                WorkflowEndEvent Event = _workflowEndEventConvertor.ConvertFromString(xEvent.ToString());
                process.AddEvent(Event);
            }

            EventsList = element.Descendants(XMLNodeName.intermediateThrowEvent.ToString());
            foreach (XElement xEvent in EventsList)
            {
                WorkflowIntermediateThrowEvent Event = _workflowIntermediateThrowEventConvertor.ConvertFromString(xEvent.ToString());
                process.AddEvent(Event);
            }
             
            EventsList = element.Descendants(XMLNodeName.intermediateCatchEvent.ToString());
            foreach (XElement xEvent in EventsList)
            {
                WorkflowIntermediateCatchEvent Event = _workflowIntermediateCatchEventConvertor.ConvertFromString(xEvent.ToString());
                process.AddEvent(Event);
            }

            EventsList = element.Descendants(XMLNodeName.boundaryEvent.ToString());
            foreach (XElement xEvent in EventsList)
            {
                WorkflowBoundaryEvent Event = _workflowBoundaryEventConvertor.ConvertFromString(xEvent.ToString());
                process.AddEvent(Event);
            }
        }

        private void ConvertFromXMLForGateWays(WorkflowProcess process, XElement element)
        {
            // Get list GateWay
            IEnumerable<XElement> GatewayList = element.Descendants(XMLNodeName.exclusiveGateway.ToString());
            foreach (XElement xEvent in GatewayList)
            {
                WorkflowExclusiveGateway exclusiveGateway = _workflowExclusiveGatewayConvertor.ConvertFromString(xEvent.ToString());
                process.AddGateWay(exclusiveGateway);
            }

            GatewayList = element.Descendants(XMLNodeName.inclusiveGateway.ToString());
            foreach (XElement xEvent in GatewayList)
            {
                WorkflowInclusiveGateway inclusiveGateway = _workflowInclusiveGatewayConvertor.ConvertFromString(xEvent.ToString());
                process.AddGateWay(inclusiveGateway);
            }

            GatewayList = element.Descendants(XMLNodeName.parallelGateway.ToString());
            foreach (XElement xEvent in GatewayList)
            {
                WorkflowParallelGateway parallelGateway = _workflowParallelGatewayConvertor.ConvertFromString(xEvent.ToString());
                process.AddGateWay(parallelGateway);
            }
        }

        public override XElement ConvertToXml(WorkflowProcess workflowElement)
        {
            XElement element = new XElement(XMLNodeName.process.ToString(),
                new XAttribute(XMLAttributeName.id.ToString(), workflowElement.ID),
                new XAttribute(XMLAttributeName.name.ToString(), workflowElement.Name));

            //UserTask
            foreach (WorkflowUserTask UserTask in workflowElement.UserTasks.ToList())
            {
                string strUserTask = _workflowProcessUserTasksConvertor.ConvertToString(UserTask);
                element.Add(XElement.Parse(strUserTask));
            }

            //ServiceTask
            foreach (WorkflowServiceTask ServiceTask in workflowElement.ServiceTasks.ToList())
            {
                string strServiceTask = _workflowProcessServiceTasksConvertor.ConvertToString(ServiceTask);
                element.Add(XElement.Parse(strServiceTask));
            }

            //SequenceFlow
            foreach (WorkflowSequenceFlow Flow in workflowElement.SequenceFlows)
            {
                string strFlow = _workflowFlowsConvertor.ConvertToString(Flow);
                element.Add(XElement.Parse(strFlow));
            }

            //StartEvents
            foreach (WorkflowStartEvent Event in workflowElement.StartEvents)
            {
                string strEvent = _workflowStartEventConvertor.ConvertToString(Event);
                element.Add(XElement.Parse(strEvent));
            }
            //EndEvents
            foreach (WorkflowEndEvent Event in workflowElement.EndEvents)
            {
                string strEvent = _workflowEndEventConvertor.ConvertToString(Event);
                element.Add(XElement.Parse(strEvent));
            }
            //IntermediateEvents
            foreach (WorkflowIntermediateThrowEvent Event in workflowElement.IntermediateThrowEvents)
            {
                string strEvent = _workflowIntermediateThrowEventConvertor.ConvertToString(Event);
                element.Add(XElement.Parse(strEvent));
            }
            //BoundaryEvents
            foreach (WorkflowBoundaryEvent Event in workflowElement.BoundaryEvents)
            {
                string strEvent = _workflowBoundaryEventConvertor.ConvertToString(Event);
                element.Add(XElement.Parse(strEvent));
            }
            //exclusiveGateway
            foreach (WorkflowExclusiveGateway ExclusiveGateway in workflowElement.ExclusiveGateways)
            {
                string strExclusiveGateway = _workflowExclusiveGatewayConvertor.ConvertToString(ExclusiveGateway);
                element.Add(XElement.Parse(strExclusiveGateway));
            }
            //inclusiveGateway
            foreach (WorkflowInclusiveGateway InclusiveGateway in workflowElement.InclusiveGateways)
            {
                string strInclusiveGateway = _workflowInclusiveGatewayConvertor.ConvertToString(InclusiveGateway);
                element.Add(XElement.Parse(strInclusiveGateway));
            }
            //parallelGateway
            foreach (WorkflowParallelGateway ParallelGateway in workflowElement.ParallelGateways)
            {
                string strParallelGateway = _workflowParallelGatewayConvertor.ConvertToString(ParallelGateway);
                element.Add(XElement.Parse(strParallelGateway));
            }
            return element;
        }

    }
}
