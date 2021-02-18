using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [Serializable]
    public partial class sysBpmsThreadTask
    {
        public enum e_StatusLU
        {
            [Description("New")]
            New = 1,
            [Description("Ongoing")]
            Ongoing = 2,
            [Description("Delayed")]
            Delayed = 3,
            [Description("Done")]
            Done = 4
        }

        public void Update(Guid threadID, Guid taskID, DateTime startDate, DateTime? endDate, string description,
             int? priorityLU, int statusLU)
        {
            this.ThreadID = threadID;
            this.TaskID = taskID;
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.Description = description;
            this.PriorityLU = priorityLU;
            this.StatusLU = statusLU;
        }

        public void Update(string description)
        {
            this.Description = description;
        }

        public void Update(DateTime? endDate, int statusLU, Guid? ownerUserID)
        {
            this.OwnerUserID = ownerUserID;
            this.EndDate = endDate;
            this.StatusLU = statusLU;
        }
 
        public void UpdateAccessInfo(Guid? ownerUserID, List<Tuple<Guid?, string>> ownerRole)
        {
            this.OwnerUserID = ownerUserID;
            if (ownerRole != null && ownerRole.Any())
                this.OwnerRole = string.Join(",", ownerRole.Select(c => (c.Item1?.ToString() ?? "0") + ":" + c.Item2));
            else
                this.OwnerRole = string.Empty;
        } 
        /// <param name="roleLU">DepartmentMember.e_RoleLU</param>
        public bool IsInRole(Guid? departmentId, int roleLU)
        {
            return this.GetDepartmentRoles.Any(c => (!departmentId.HasValue || c.Item1 == departmentId) && c.Item2.ToIntObj() == roleLU);
        }
        [NotMapped]
        public List<Tuple<Guid?, string>> GetDepartmentRoles
        {
            get
            {
                return this.OwnerRole.ToStringObj().Split(',').Where(c => !string.IsNullOrWhiteSpace(c)).Select(c =>
                new Tuple<Guid?, string>(c.Split(':').FirstOrDefault().ToGuidObjNull(), c.Split(':').LastOrDefault())).ToList();
            }
        }
    }
}
