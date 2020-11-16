using System.Linq;
using System.Collections.Generic;

namespace DynamicBusiness.BPMS.Domain
{
    public class WorkflowLaneSet : WorkflowElement
    {
        private readonly List<WorkflowLane> _Lanes;
        public IEnumerable<WorkflowLane> Lanes => _Lanes;

        public WorkflowLaneSet(string id)
            : base(id)
        {
            _Lanes = new List<WorkflowLane>();
        }

        public void AddLane(WorkflowLane Lane)
        {
            _Lanes.Add(Lane);
        }
    }
}