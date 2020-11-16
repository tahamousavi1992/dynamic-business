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
    public class ApplicationPageAccessDTO
    {
        public ApplicationPageAccessDTO() { }
        public ApplicationPageAccessDTO(sysBpmsApplicationPageAccess applicationPageAccess)
        {
            this.ID = applicationPageAccess.ID;
            this.ApplicationPageID = applicationPageAccess.ApplicationPageID;
            this.DepartmentID = applicationPageAccess.DepartmentID;
            this.RoleLU = applicationPageAccess.RoleLU;
            this.UserID = applicationPageAccess.UserID;
            this.AllowAdd = applicationPageAccess.AllowAdd;
            this.AllowEdit = applicationPageAccess.AllowEdit;
            this.AllowDelete = applicationPageAccess.AllowDelete;
            this.AllowView = applicationPageAccess.AllowView;
        }
        [DataMember]
        public System.Guid ID { get; set; }
        [DataMember]
        public System.Guid ApplicationPageID { get; set; } 
        [DataMember]
        public Nullable<System.Guid> DepartmentID { get; set; } 
        [DataMember]
        public Nullable<int> RoleLU { get; set; } 
        [DataMember]
        public Nullable<System.Guid> UserID { get; set; } 
        [DataMember]
        public bool AllowAdd { get; set; } 
        [DataMember]
        public bool AllowEdit { get; set; } 
        [DataMember]
        public bool AllowDelete { get; set; } 
        [DataMember]
        public bool AllowView { get; set; } 
        [DataMember]
        public string DepartmentName { get { return this.DepartmentID.HasValue ? new DepartmentService().GetInfo(this.DepartmentID.Value).Name : ""; } private set { } }
 
        [DataMember]
        public string RoleName { get { return this.RoleLU.HasValue ? new LURowService().GetNameOfByAlias(sysBpmsLUTable.e_LUTable.DepartmentRoleLU.ToString(), this.RoleLU.ToString()) : ""; } private set { } }
        [DataMember]
        public string UserFullName { get { return this.UserID.HasValue ? new UserService().GetInfo(this.UserID.Value).FullName : ""; } private set { } }
    }
}