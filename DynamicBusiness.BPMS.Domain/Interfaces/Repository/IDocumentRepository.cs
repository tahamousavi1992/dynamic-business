using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface IDocumentRepository
    {
        void Add(sysBpmsDocument document);
        void Update(sysBpmsDocument document);
        void Delete(Guid Guid);
        sysBpmsDocument GetInfo(Guid Guid);
        List<sysBpmsDocument> GetList(Guid? DocumentDefID, Guid? EntityDefID, Guid? EntityID, string CaptionOf, bool? IsDeleted, Guid? DocumentFolderID, Guid? threadId);
    }
}
