using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public class WorkflowProcess : WorkflowElement
    {
        private List<WorkflowStartEvent> _startEvents;
        private List<WorkflowEndEvent> _endEvents;
        private List<WorkflowIntermediateThrowEvent> _intermediateThrowEvents;
        private List<WorkflowIntermediateCatchEvent> _intermediateCatchEvents;
        private List<WorkflowBoundaryEvent> _boundaryEvents; 
        private List<WorkflowSequenceFlow> _Sequenceflows;
        private List<WorkflowForms> _forms;
        private List<WorkflowUserTask> _usertasks;
        private List<WorkflowServiceTask> _servicetasks;
        private List<WorkflowScriptTask> _scripttasks;
        private List<WorkflowTask> _tasks;
        private List<WorkflowExclusiveGateway> _workflowExclusiveGateway;
        private List<WorkflowInclusiveGateway> _workflowInclusiveGateway;
        private List<WorkflowParallelGateway> _workflowParallelGateway;
        public WorkflowProcess(string id, string name)
            : base(id, name)
        {
            _forms = new List<WorkflowForms>();
            _startEvents = new List<WorkflowStartEvent>();
            _endEvents = new List<WorkflowEndEvent>();
            _intermediateThrowEvents = new List<WorkflowIntermediateThrowEvent>();
            _intermediateCatchEvents = new List<WorkflowIntermediateCatchEvent>();
            _boundaryEvents = new List<WorkflowBoundaryEvent>();
            _Sequenceflows = new List<WorkflowSequenceFlow>();
            _usertasks = new List<WorkflowUserTask>();
            _servicetasks = new List<WorkflowServiceTask>();
            _scripttasks = new List<WorkflowScriptTask>();
            _tasks = new List<WorkflowTask>();
            _workflowExclusiveGateway = new List<WorkflowExclusiveGateway>();
            _workflowInclusiveGateway = new List<WorkflowInclusiveGateway>();
            _workflowParallelGateway = new List<WorkflowParallelGateway>();
        }

        public IEnumerable<WorkflowStartEvent> StartEvents => _startEvents;
        public IEnumerable<WorkflowEndEvent> EndEvents => _endEvents;
        public IEnumerable<WorkflowIntermediateThrowEvent> IntermediateThrowEvents => _intermediateThrowEvents;
        public IEnumerable<WorkflowIntermediateCatchEvent> IntermediateCatchEvents => _intermediateCatchEvents;
        public IEnumerable<WorkflowBoundaryEvent> BoundaryEvents => _boundaryEvents;
        public IEnumerable<WorkflowUserTask> UserTasks => _usertasks;
        public IEnumerable<WorkflowTask> Tasks => _tasks;
        public IEnumerable<WorkflowServiceTask> ServiceTasks => _servicetasks;
        public IEnumerable<WorkflowScriptTask> ScriptTasks => _scripttasks;
        public IEnumerable<WorkflowSequenceFlow> SequenceFlows => _Sequenceflows;
        public IEnumerable<WorkflowForms> Forms => _forms;
        public WorkflowLaneSet LaneSet { get; set; }
        public IEnumerable<WorkflowExclusiveGateway> ExclusiveGateways => _workflowExclusiveGateway;
        public IEnumerable<WorkflowInclusiveGateway> InclusiveGateways => _workflowInclusiveGateway;
        public IEnumerable<WorkflowParallelGateway> ParallelGateways => _workflowParallelGateway;
        public void AddTask(WorkflowUserTask task)
        {
            _usertasks.Add(task);
        }

        public void AddTask(WorkflowServiceTask task)
        {
            _servicetasks.Add(task);
        }

        public void AddTask(WorkflowScriptTask task)
        {
            _scripttasks.Add(task);
        }

        public void AddTask(WorkflowTask task)
        {
            _tasks.Add(task);
        }

        public void AddFlow(WorkflowSequenceFlow flow)
        {
            _Sequenceflows.Add(flow);
        }

        public void AddEvent(WorkflowStartEvent Event)
        {
            _startEvents.Add(Event);
        }
        public void AddEvent(WorkflowEndEvent Event)
        {
            _endEvents.Add(Event);
        }
        public void AddEvent(WorkflowIntermediateThrowEvent Event)
        {
            _intermediateThrowEvents.Add(Event);
        }
        public void AddEvent(WorkflowIntermediateCatchEvent Event)
        {
            _intermediateCatchEvents.Add(Event);
        }
        public void AddEvent(WorkflowBoundaryEvent Event)
        {
            _boundaryEvents.Add(Event);
        }
        public void AddGateWay(WorkflowExclusiveGateway GateWay)
        {
            _workflowExclusiveGateway.Add(GateWay);
        }
        public void AddGateWay(WorkflowInclusiveGateway GateWay)
        {
            _workflowInclusiveGateway.Add(GateWay);
        }
        public void AddGateWay(WorkflowParallelGateway GateWay)
        {
            _workflowParallelGateway.Add(GateWay);
        }
    }
}

