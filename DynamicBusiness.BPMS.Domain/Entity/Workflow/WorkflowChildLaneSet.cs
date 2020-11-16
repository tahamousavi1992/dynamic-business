using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public class WorkflowChildLaneSet : WorkflowElement
    {
        public string Type { get; private set; }
        private readonly List<WorkflowLane> _Lanes;
        public IEnumerable<WorkflowLane> Lanes => _Lanes;

        public WorkflowChildLaneSet(string type, string id)
            : base(id, "")
        {
            _Lanes = new List<WorkflowLane>();
            this.Type = type;
        }

        public void AddLane(WorkflowLane Lane)
        {
            _Lanes.Add(Lane);
        }
    }
}
