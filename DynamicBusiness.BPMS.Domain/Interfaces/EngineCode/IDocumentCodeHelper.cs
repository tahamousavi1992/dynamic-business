using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface IDocumentCodeHelper
    {
        void Download(string base64, string fileName);

        void DownloadByStream(System.IO.MemoryStream streamFile, string fileName);

        void DownloadByByte(byte[] bytes, string fileName);

        List<sysBpmsDocument> GetList(Guid? documentDefID, Guid? documentFolderID, Guid? entityID, Guid? threadId);

        bool CheckMandatory(Guid? documentDefID, Guid? documentFolderID, Guid? entityID, Guid? threadId);
    }
}
