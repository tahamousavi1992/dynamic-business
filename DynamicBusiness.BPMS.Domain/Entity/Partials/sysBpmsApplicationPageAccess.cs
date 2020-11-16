using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [Serializable]
    public partial class sysBpmsApplicationPageAccess
    {
        public sysBpmsApplicationPageAccess() { }

        public sysBpmsApplicationPageAccess Update(Guid id,Guid applicationPageID, Guid? departmentID, int? roleLU, Guid? userID, bool allowAdd, bool allowEdit, bool allowDelete, bool allowView)
        {
            this.ID = id;
            this.ApplicationPageID = applicationPageID;
            this.DepartmentID = departmentID;
            this.RoleLU = roleLU;
            this.UserID = userID;
            this.AllowAdd = allowAdd;
            this.AllowEdit = allowEdit;
            this.AllowDelete = allowDelete;
            this.AllowView = allowView;
            return this;
        }

        public void Load(sysBpmsApplicationPageAccess applicationPageAccess)
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
    }
}
