using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface IDocumentEngine
    {
        /// <summary>
        /// It is used for Forms inside forms to change appPageID of parent form for retrieving variables.
        /// </summary>
        void SetApplicationPageID(Guid appPageID);
        ResultOperation IsValid(FileUploadHtml fileUploadHtml, sysBpmsVariable variable, Guid? entityId, Guid? entitydefid, string currentUserName);
        ResultOperation IsValid(Guid? EntityDefID, Guid? EntityID, System.IO.Stream InputStream, string FileName, Guid DocumentDefID);
        ResultOperation SaveFile(FileUploadHtml fileUploadHtml, sysBpmsVariable variable, Guid? entityId, Guid? entitydefid, string captionOf, string currentUserName);
        ResultOperation SaveFile(System.IO.Stream inputStream, string fileName, Guid? entityID, Guid? entityDefID, Guid documentDefID, string captionOF, bool replace);
        List<sysBpmsDocument> GetList(Guid? DocumentDefId, Guid? VariableId, Guid? documentFolderId);
    }
}
