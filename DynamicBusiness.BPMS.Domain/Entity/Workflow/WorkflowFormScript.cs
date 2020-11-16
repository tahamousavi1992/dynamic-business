namespace DynamicBusiness.BPMS.Domain
{
    public class WorkflowFormScript: WorkflowElement
    {
        public FormScriptType Type { get; private set; }
        public enum FormScriptType
        {
            Client,
            Server
        }

        public WorkflowFormScript(FormScriptType type, string id): base(id)
        {
            Type = type;
        }
    }
}