using DynamicBusiness.BPMS.BusinessLogic;
using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DynamicBusiness.BPMS.SharedPresentation
{
    [DataContract]
    public class TaskDTO
    {
        public TaskDTO() { }
        public TaskDTO(sysBpmsTask task)
        {
            this.ID = task.ID;
            this.ElementID = task.ElementID;
            this.ProcessID = task.ProcessID;
            this.RoleName = task.RoleName;
            this.OwnerTypeLU = task.OwnerTypeLU;
            this.UserID = task.UserID;
            this.Rule = task.Rule;
            this.MarkerTypeLU = task.MarkerTypeLU;
            if (task.OwnerTypeLU == (int)sysBpmsTask.e_OwnerTypeLU.Role)
                this.SpecificDepartmentID = this.UserTaskRuleModel.SpecificDepartmentId;
            else
                this.SpecificDepartmentID = null;
        }
        [DataMember]
        public System.Guid ID { get; set; }
        [DataMember]
        public string ElementID { get; set; }
        [DataMember]
        public int TypeLU { get; set; }
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public Nullable<int> MarkerTypeLU { get; set; }
        [DataMember]
        public Nullable<int> OwnerTypeLU { get; set; }
        [DataMember]
        public string RoleName { get; set; }
        [DataMember]
        public string Rule { get; set; }
        [DataMember]
        public string UserID { get; set; }
        [DataMember]
        public System.Guid ProcessID { get; set; }
        [DataMember]
        public Guid? SpecificDepartmentID { get; set; }
        [DataMember]
        public UserTaskRuleModel UserTaskRuleModel
        {
            get
            {
                return this.Rule.ParseXML<UserTaskRuleModel>();
            }
            private set { }
        }
        [DataMember]
        public string GetDepartmentRoles
        {
            get
            {
                return string.Join(",", (this.RoleName.Split(',').Where(c => !string.IsNullOrWhiteSpace(c)).Select(c =>
               new Tuple<Guid?, string>(c.Split(':').FirstOrDefault().ToGuidObjNull(), c.Split(':').LastOrDefault())).ToList().Select(c => c.Item2)));
            }
            private set { }
        }
    }

}