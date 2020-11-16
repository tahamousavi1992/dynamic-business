using System;
using System.Collections.Generic;

namespace DynamicBusiness.BPMS.Domain
{
    public class WorkflowFormGroup : WorkflowElement
    {

        private readonly List<WorkflowForm> _forms;
        public string Caption { get; private set; }
        public string Description { get; private set; }

        public WorkflowFormGroup(List<WorkflowForm> forms, string id)
             : base(id)
        {
            _forms = forms;
        }

        public void AddWorkflowForm(int version, string createdByUsername, string id)
        {
            _forms.Add(new WorkflowForm(version, createdByUsername, DateTime.Now, id));

        }
    }
}