using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface IDocumentFolderRepository
    {
        void Add(sysBpmsDocumentFolder DocumentFolder);
        void Update(sysBpmsDocumentFolder DocumentFolder);
        void Delete(Guid DocumentFolderId);
        sysBpmsDocumentFolder GetInfo(Guid ID);
        List<sysBpmsDocumentFolder> GetList(Guid? ParentDocumentFolderID, string NameOf, string DisplayName, bool? isActive);
    }
}
