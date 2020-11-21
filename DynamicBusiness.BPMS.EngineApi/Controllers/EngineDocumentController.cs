using DynamicBusiness.BPMS.BusinessLogic;
using DynamicBusiness.BPMS.Domain;
using DynamicBusiness.BPMS.SharedPresentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DynamicBusiness.BPMS.EngineApi.Controllers
{
    public class EngineDocumentController : BpmsEngineApiControlBase
    {
        [BpmsAuth]
        [HttpGet]
        public void GetDownload(string guid)
        {
            using (DocumentService documentService = new DocumentService())
            {

                base.Download(documentService.GetInfo(StringCipher.DecryptFormValues(guid, base.ApiSessionId.ToString(), base.IsEncrypted).ToGuidObj()));
            }
        }

        [BpmsAuth]
        [HttpGet]
        public object GetDelete(string guid)
        {
            using (DocumentService documentService = new DocumentService())
            {
                ResultOperation resultOperation = documentService.InActive(StringCipher.DecryptFormValues(guid, base.ApiSessionId.ToString(), base.IsEncrypted).ToGuidObj());
                if (resultOperation.IsSuccess)
                    return new { message = "حذف فایل موفقیت آمیز بود.", isSuccess = true };
                else
                    return new { message = "خطایی در حذف فایل رخ داده است.", isSuccess = false };
            }
        }
    }
}