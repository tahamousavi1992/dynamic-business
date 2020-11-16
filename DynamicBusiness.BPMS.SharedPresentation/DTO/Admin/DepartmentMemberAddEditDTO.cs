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
    public class DepartmentMemberAddEditDTO
    {
        public DepartmentMemberAddEditDTO()
        {
            //this.Roles = new List<int>();
        }
        public DepartmentMemberAddEditDTO(Guid departmentID, Guid? userId)
        {
            this.DepartmentID = departmentID;
            this.UserID = userId ?? Guid.Empty;
            using (LURowService luRowService = new LURowService())
                this.Roles = userId.HasValue ?
                    new DepartmentMemberService().GetList(this.DepartmentID, null, this.UserID)
                    .Select(c => new QueryModel(c.RoleLU.ToString(), luRowService.GetNameOfByAlias("DepartmentRoleLU", c.RoleLU.ToString()))).ToList() : new List<QueryModel>();
        } 
        [DataMember]
        public Guid DepartmentID { get; set; }
         
        [DataMember]
        public Guid UserID { get; set; }
         
        [DataMember]
        public List<QueryModel> Roles { get; set; }

        [DataMember]
        public List<UserDTO> ListUsers { get; set; }

        [DataMember]
        public List<LURowDTO> ListRoles { get; set; }
    }
}