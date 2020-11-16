
using System;

namespace DynamicBusiness.BPMS.Domain
{
    public class WorkflowForm : WorkflowElement
    {
        public int Version { get; set; }
        public string CreatedByUsername { get; set; }
        public System.DateTime CreatedOnDate { get; set; }
        public WorkflowFormScripts FormScripts { get; set; }

        public WorkflowForm(int version, string createdByUsername, DateTime created, string id)
            : base(id)
        {
            Version = version;
            CreatedByUsername = createdByUsername;
            CreatedOnDate = created;
        }
    }
}