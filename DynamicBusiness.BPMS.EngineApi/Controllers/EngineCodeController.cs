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
    public class EngineCodeController : BpmsEngineApiControlBase
    {
        [BpmsAuth]
        [HttpGet]
        public bool GetConfirmResult(string controlId, Guid formId, bool isGridCommand, string gridId = "", Guid? threadId = null, Guid? applicationPageId = null)
        {
            using (DynamicFormService dynamicFormService = new DynamicFormService())
            {
                sysBpmsDynamicForm dynamicForm = dynamicFormService.GetInfo(formId);
                object control = dynamicForm.FindControl(isGridCommand ? gridId : controlId);
                DesignCodeModel designCode = null;

                if (control is ButtonHtml)
                {
                    designCode = ((ButtonHtml)control).ConfirmDesignCodeModel;
                }

                if (control is DataGridHtml)
                {
                    designCode = ((DataGridHtml)control).GetConfirmCode(controlId);
                }

                EngineSharedModel engineSharedModel = applicationPageId.HasValue ?
                new EngineSharedModel(applicationPageId.Value, HttpContext.Current.Request.GetList(base.IsEncrypted, base.ApiSessionId).ToList(), base.ClientUserName, base.ApiSessionId) :
                new EngineSharedModel(threadId, dynamicForm.ProcessId, HttpContext.Current.Request.GetList(base.IsEncrypted, base.ApiSessionId).ToList(), base.ClientUserName, base.ApiSessionId);
                UnitOfWork unitOfWork = new UnitOfWork();
                return new DynamicCodeEngine(engineSharedModel, unitOfWork).ExecuteBooleanCode(designCode);
            }
        }

        [BpmsAuth]
        [HttpPost]
        public object ExecuteCode(string controlId, Guid formId, string commandId, Guid? threadId = null, Guid? applicationPageId = null)
        {
            using (DynamicFormService dynamicFormService = new DynamicFormService())
            {
                sysBpmsDynamicForm dynamicForm = dynamicFormService.GetInfo(formId);
                object control = dynamicForm.FindControl(controlId);
                DesignCodeModel designCode = null;
                if (control is DataGridHtml)
                    designCode = ((DataGridHtml)control).GetCommandCode(commandId);

                EngineSharedModel engineSharedModel = applicationPageId.HasValue ?
                new EngineSharedModel(applicationPageId.Value, HttpContext.Current.Request.GetList(base.IsEncrypted, base.ApiSessionId).ToList(), base.ClientUserName, base.ApiSessionId) :
                new EngineSharedModel(threadId, dynamicForm.ProcessId, HttpContext.Current.Request.GetList(base.IsEncrypted, base.ApiSessionId).ToList(), base.ClientUserName, base.ApiSessionId);
                UnitOfWork unitOfWork = new UnitOfWork();
                DynamicCodeEngine dynamicCodeEngine = new DynamicCodeEngine(engineSharedModel, unitOfWork);
                var result = dynamicCodeEngine.ExecuteScriptCode(designCode, new CodeBaseSharedModel(null));
                //If in code any variable is set, it Will save them all at the end
                dynamicCodeEngine.SaveExternalVariable(result);

                return new
                {
                    result = result?.Result,
                    listMessage = result?.CodeBaseShared.MessageList.Select(c => new
                    {
                        DisplayMessageType = c.DisplayMessageType.ToString(),
                        c.Message
                    })
                };
            }
        }
    }
}