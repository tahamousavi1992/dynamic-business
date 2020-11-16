using DynamicBusiness.BPMS.Domain;
using DynamicBusiness.BPMS.BusinessLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace DynamicBusiness.BPMS.SharedPresentation
{
    [DataContract]
    public class DepartmentMemberListDTO
    {
        public DepartmentMemberListDTO() { }
        public DepartmentMemberListDTO(sysBpmsDepartmentMember departmentMember)
        {
            if (departmentMember != null)
            {
                this.ID = departmentMember.ID;
                this.DepartmentID = departmentMember.DepartmentID;
                this.UserID = departmentMember.UserID;
            }
        }
        [DataMember]
        public Guid ID { get; set; } 
        [DataMember]
        public Guid DepartmentID { get; set; } 
        [DataMember]
        public Guid UserID { get; set; } 
        [DataMember]
        public string RoleNames { get { return string.Join(",", new DepartmentMemberService().GetList(this.DepartmentID, null, this.UserID).Select(c => new LURowService().GetNameOfByAlias("DepartmentRoleLU", c.RoleLU.ToString()))); } private set { } }
 
        [DataMember]
        public string UserFullName
        {
            get
            {
                sysBpmsUser user = new UserService().GetInfo(this.UserID);
                return user.FirstName + " " + user.LastName;
            }
            private set { }
        }
    }
}