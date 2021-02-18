using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [Serializable]
    public partial class sysBpmsDocumentDef
    {
        public void Update(Guid documentFolderID, string nameOf, string displayName, int? maxSize,
            string validExtentions, bool isMandatory, string description, bool isSystemic, bool isActive)
        {
            this.DocumentFolderID = documentFolderID;
            this.NameOf = nameOf.ToStringObj();
            this.DisplayName = displayName.ToStringObj();
            this.MaxSize = maxSize;
            this.ValidExtentions = validExtentions.ToStringObj();
            this.IsMandatory = isMandatory;
            this.Description = description;
            this.IsSystemic = isSystemic;
            this.IsActive = isActive;

        }
       
    }
}
