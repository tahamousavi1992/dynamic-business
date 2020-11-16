using System.Collections.Generic;

namespace DynamicBusiness.BPMS.Domain
{
    public class WorkflowFormScripts : WorkflowElement
    {
        private readonly List<WorkflowFormScript> _formScripts;
        public IEnumerable<WorkflowFormScript> FormScripts => _formScripts;       

        public WorkflowFormScripts(List<WorkflowFormScript> formScripts, string id)
            : base(id)
        {
            _formScripts = formScripts;
        }
    }
}