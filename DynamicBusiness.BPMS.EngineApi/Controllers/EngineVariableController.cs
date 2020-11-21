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
    public class EngineVariableController : BpmsEngineApiControlBase
    {
        [BpmsAuth]
        [HttpGet]
        public List<ComboSearchItem> GetListElement(Guid formId, string controlId, Guid? threadId = null)
        {
            using (DynamicFormService dynamicFormService = new DynamicFormService())
            {
                sysBpmsDynamicForm dynamicForm = dynamicFormService.GetInfo(formId);
                ListItemElementBase control = (ListItemElementBase)dynamicForm.FindControl(controlId);
                EngineSharedModel engineSharedModel = dynamicForm.ProcessId.HasValue ?
                    new EngineSharedModel(threadId, dynamicForm.ProcessId, base.MyRequest.GetList(base.IsEncrypted, base.ApiSessionId).ToList(), base.ClientUserName, base.ApiSessionId) :
                    new EngineSharedModel(dynamicForm.ApplicationPageID.Value, base.MyRequest.GetList(base.IsEncrypted, base.ApiSessionId).ToList(), base.ClientUserName, base.ApiSessionId);
                UnitOfWork unitOfWork = new UnitOfWork();
                control.Helper = HtmlElementHelper.MakeModel(engineSharedModel, unitOfWork, HtmlElementHelperModel.e_FormAction.Onload, dynamicForm);
                control.FillData();
                return control.Options.Select(item => new ComboSearchItem()
                {
                    text = item.Label,
                    id = item.Value,
                }).ToList();
            }
        }

    }
}