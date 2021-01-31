using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    /// <summary>
    /// This is used for binding
    /// </summary>
    [Serializable]
    [DataContract]
    public class UserTaskRuleModel
    { 
        [Required]
        [DataMember]
        public int AccessType { get; set; }
         
        [DataMember]
        public bool GoUpDepartment { get; set; }
         
        [DataMember]
        public int? UserType { get; set; }
         
        [DataMember]
        public string Variable { get; set; }
         
        [DataMember]
        public string RoleCode { get; set; }
         
        [DataMember]
        public Guid? SpecificDepartmentId { get; set; }

        public enum e_UserAccessType
        {
            [Description("Variable")]
            Variable = 1,
            [Description("Static")]
            Static = 2,
        }
        public enum e_RoleAccessType
        {
            [Description("Variable")]
            Variable = 1,
            [Description("Static")]
            Static = 2,
            [Description("CorrespondentRole")]
            CorrespondentRole = 3,
        }

        public enum e_UserType
        {
            [Description("CurrentUserID")]
            CurrentUserID = 1,
            [Description("ThreadUserID")]
            ThreadUserID = 2,
        }
    }
}
