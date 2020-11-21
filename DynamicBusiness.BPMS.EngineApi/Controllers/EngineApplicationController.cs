using DotNetNuke.Web.Api;
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
    public class EngineApplicationController : BpmsEngineApiControlBase
    {
        [BpmsAuth]
        [HttpGet]
        public GetFormResponseModel GetForm(Guid? applicationPageId = null, Guid? formID = null)
        {
            using (DynamicFormService dynamicFormService = new DynamicFormService())
            {
                applicationPageId = applicationPageId ?? dynamicFormService.GetInfo(formID.Value).ApplicationPageID;
                List<QueryModel> baseQueryModel = base.MyRequest.GetList(base.IsEncrypted, base.ApiSessionId).ToList();
                using (ApplicationPageEngine applicationPageEngine = new ApplicationPageEngine(new EngineSharedModel(applicationPageId.Value, baseQueryModel, base.ClientUserName, base.ApiSessionId)))
                {
                    return applicationPageEngine.GetForm();
                }
            }
        }

        [BpmsAuth]
        [HttpPost]
        public PostFormResponseModel PostForm(Guid applicationPageId, string controlId = "")
        {
            using (DynamicFormService dynamicFormService = new DynamicFormService())
            {
                List<QueryModel> baseQueryModel = base.MyRequest.GetList(base.IsEncrypted, base.ApiSessionId).ToList();
                using (ApplicationPageEngine applicationPageEngine = new ApplicationPageEngine(new EngineSharedModel(applicationPageId, baseQueryModel, base.ClientUserName, base.ApiSessionId)))
                {
                    return applicationPageEngine.PostForm(controlId);
                }
            }
        }

        [BpmsAuth]
        [HttpGet]
        public List<ComboSearchItem> GetList(string query = "")
        {
            using (DynamicFormService dynamicFormService = new DynamicFormService())
            {
                return dynamicFormService.GetList(null, null, true, query, null, null).Select(c => new ComboSearchItem(c.ApplicationPageID.ToString(), c.Name)).ToList();
            }
        }

        [BpmsAuth]
        [HttpGet]
        public DynamicFormDTO GetInfo(Guid applicationPageId)
        {
            using (DynamicFormService dynamicFormService = new DynamicFormService())
            {
                return new DynamicFormDTO(dynamicFormService.GetInfoByPageID(applicationPageId));
            }
        }
    }
}