using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [Serializable]
    public partial class sysBpmsDocument
    {
        public void Load(sysBpmsDocument Document)
        {
            this.GUID = Document.GUID;
            this.DocumentDefID = Document.DocumentDefID;
            this.IsDeleted = Document.IsDeleted;
            this.EntityDefID = Document.EntityDefID;
            this.EntityID = Document.EntityID;
            this.AtachDateOf = Document.AtachDateOf;
            this.FileExtention = Document.FileExtention;
            this.CaptionOf = Document.CaptionOf;
            this.ThreadID = Document.ThreadID;
        }
    }
}
