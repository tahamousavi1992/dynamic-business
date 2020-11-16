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
    [AllowAnonymous]
    public class EngineOperationController : BpmsApiControlBase
    {
        [BpmsAuth]
        [HttpPost]
        public object Execute(Guid operationId, Guid? threadId = null, Guid? applicationPageId = null)
        {
            using (OperationService operationService = new OperationService())
            {
                using (DynamicFormService dynamicFormService = new DynamicFormService())
                {
                    EngineSharedModel engineSharedModel = threadId.HasValue ?
                    new EngineSharedModel(threadId, new ThreadService().GetInfo(threadId.Value).ProcessID, base.MyRequest.GetList(base.IsEncrypted, base.ApiSessionId).ToList(), base.ClientUserName, base.ApiSessionId) :
                    applicationPageId.HasValue ? new EngineSharedModel(applicationPageId.Value, base.MyRequest.GetList(base.IsEncrypted, base.ApiSessionId).ToList(), base.ClientUserName, base.ApiSessionId) :
                    new EngineSharedModel(base.MyRequest.GetList(base.IsEncrypted, base.ApiSessionId).ToList(), base.ClientUserName, base.ApiSessionId);
                    using (OperationEngine operationEngine = new OperationEngine(engineSharedModel))
                    {
                        sysBpmsOperation operation = operationService.GetInfo(operationId);
                        if (operation != null)
                        {
                            return operationEngine.RunQuery(operation).ToIntObj() > 0;
                        }
                        else return false;
                    }
                }
            }
        }
    }
}