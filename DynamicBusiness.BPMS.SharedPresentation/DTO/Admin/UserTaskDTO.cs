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
    public class UserTaskDTO
    {
        public UserTaskDTO() { }
        public UserTaskDTO(sysBpmsTask task)
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
        public Guid ID { get; set; }
        [DataMember]
        public string ElementID { get; set; }
        [DataMember]
        public Guid ProcessID { get; set; }
        /// <summary>
        /// if OwnerTypeLU is Role, SpecificDepartmentID is set .
        /// </summary> 
        [DataMember]
        public Guid? SpecificDepartmentID { get; set; }
        //RoleName is ,departmentId:roleId,departmentId:roleId . 
        [DataMember]
        public string RoleName { get; set; } 
        [DataMember]
        public int? OwnerTypeLU { get; set; } 
        [DataMember]
        public string UserID { get; set; } 
        [DataMember]
        public string Rule { get; set; }
        [DataMember]
        public Nullable<int> MarkerTypeLU { get; set; }
        [DataMember]
        public List<Tuple<Guid?, string>> GetDepartmentRoles
        {
            get
            {
                return this.RoleName.Split(',').Where(c => !string.IsNullOrWhiteSpace(c)).Select(c =>
                new Tuple<Guid?, string>(c.Split(':').FirstOrDefault().ToGuidObjNull(), c.Split(':').LastOrDefault())).ToList();
            }
            private set { }
        }
        [DataMember]
        public UserTaskRuleModel UserTaskRuleModel
        {
            get
            {
                return this.Rule.ParseXML<UserTaskRuleModel>();
            }
            private set { }
        }

    }

}