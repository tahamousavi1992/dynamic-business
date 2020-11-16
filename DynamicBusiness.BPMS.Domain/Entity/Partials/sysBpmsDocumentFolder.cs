using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [Serializable]
    public partial class sysBpmsDocumentFolder
    {
        public void Update(Guid? documentFolderID, string nameOf, string displayName, bool isActive)
        {
            this.DocumentFolderID = documentFolderID.HasValue ? documentFolderID : (Guid?)null;
            this.NameOf = nameOf ?? string.Empty;
            this.DisplayName = displayName ?? string.Empty;
            this.IsActive = isActive;
        }

        public void InActive()
        {
            this.IsActive = false;
        }

        public void Load(sysBpmsDocumentFolder documentFolder)
        {
            this.ID = documentFolder.ID;
            this.DocumentFolderID = documentFolder.DocumentFolderID;
            this.NameOf = documentFolder.NameOf;
            this.DisplayName = documentFolder.DisplayName;
            this.IsActive = documentFolder.IsActive;
        }
    }
}
