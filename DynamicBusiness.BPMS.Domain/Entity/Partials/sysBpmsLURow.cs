
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicBusiness.BPMS.Domain
{
    [Serializable]
    public partial class sysBpmsLURow
    {
        public sysBpmsLURow Update(Guid luTableID, string nameOf,
            string codeOf, int displayOrder, bool isSystemic, bool isActive)
        {
            this.LUTableID = luTableID;
            this.NameOf = nameOf;
            this.CodeOf = codeOf;
            this.DisplayOrder = displayOrder;
            this.IsSystemic = isSystemic;
            this.IsActive = isActive;
            return this;
        }

        public sysBpmsLURow Update(bool isActive)
        {
            this.IsActive = isActive;
            return this;
        }

        public void Load(sysBpmsLURow LURow)
        {
            this.ID = LURow.ID;
            this.LUTableID = LURow.LUTableID;
            this.NameOf = LURow.NameOf;
            this.CodeOf = LURow.CodeOf;
            this.DisplayOrder = LURow.DisplayOrder;
            this.IsSystemic = LURow.IsSystemic;
            this.IsActive = LURow.IsActive;
        }
 
    }
}
