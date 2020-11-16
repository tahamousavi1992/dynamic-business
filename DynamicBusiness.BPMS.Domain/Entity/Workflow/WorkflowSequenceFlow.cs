using System.Collections.Generic;

namespace DynamicBusiness.BPMS.Domain
{
    public class WorkflowSequenceFlow : WorkflowElement
    {
        public string TargetRef { get; private set; }
        public string SourceRef { get; private set; }
        public WorkflowSequenceFlow(string targetRef, string sourceRef, string id, string name = "")
            : base(id, name)
        {
            TargetRef = targetRef;
            SourceRef = sourceRef;
        }
    }
}