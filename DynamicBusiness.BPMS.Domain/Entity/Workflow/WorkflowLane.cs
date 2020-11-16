using System.Collections.Generic;
using System.Linq;

namespace DynamicBusiness.BPMS.Domain
{
    public class WorkflowLane : WorkflowElement
    {
        public List<string> FlowNodeRefs { get; private set; }
        public List<WorkflowChildLaneSet> ChildLaneSets { get; private set; }
        public WorkflowLane(string id, string name)
            : base(id, name)
        {
            this.FlowNodeRefs = new List<string>();
            this.ChildLaneSets = new List<WorkflowChildLaneSet>();
        }
        public void AddFlowNodeRef(string _FlowNodeRef)
        {
            if (!this.FlowNodeRefs.Contains(_FlowNodeRef))
                this.FlowNodeRefs.Add(_FlowNodeRef);
        }
        public void AddChildLaneSet(WorkflowChildLaneSet _WorkflowChildLaneSet)
        {
            if (!this.ChildLaneSets.Contains(_WorkflowChildLaneSet))
                this.ChildLaneSets.Add(_WorkflowChildLaneSet);
        }

        public static List<WorkflowLane> GetAllLanes(WorkflowLaneSet workflowLaneSet)
        {
            List<WorkflowLane> Allitems = new List<WorkflowLane>();
            if (workflowLaneSet != null)
                foreach (WorkflowLane item in workflowLaneSet.Lanes)
                {
                    WorkflowLane.getChildren(item, Allitems);
                }
            return Allitems;
        }

        private static void getChildren(WorkflowLane current, List<WorkflowLane> Allitems)
        {
            if (!Allitems.Any(c => c.ID == current.ID))
                Allitems.Add(current);
            if (current.ChildLaneSets != null)
                foreach (WorkflowChildLaneSet Childitem in current.ChildLaneSets)
                {
                    if (Childitem.Lanes != null)
                        foreach (WorkflowLane item in Childitem.Lanes)
                        {
                            WorkflowLane.getChildren(item, Allitems);
                        }
                }
        }
    }
}