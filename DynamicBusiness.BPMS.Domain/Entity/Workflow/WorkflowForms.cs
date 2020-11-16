using System.Collections.Generic;

namespace DynamicBusiness.BPMS.Domain
{
    public class WorkflowForms: WorkflowElement
    {
        private readonly IEnumerable<WorkflowFormGroup> _formGroups;

        public WorkflowForms(IEnumerable<WorkflowFormGroup> formGroups, string id)
            : base(id)
        {
            _formGroups = formGroups;
        }
    }
}