using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface IDocumentDefRepository
    {
        void Add(sysBpmsDocumentDef documentDef);
        void Update(sysBpmsDocumentDef documentDef);
        void Delete(Guid DocumentDefId);
        sysBpmsDocumentDef GetInfo(Guid ID);
        List<sysBpmsDocumentDef> GetList(Guid? DocumentFolderID, string NameOf, string DisplayName, bool? IsActive, bool? IsSystemic, PagingProperties currentPaging);
    }
}
