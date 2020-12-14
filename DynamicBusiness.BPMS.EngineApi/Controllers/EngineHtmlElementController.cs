using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DynamicBusiness.BPMS.BusinessLogic;
using DynamicBusiness.BPMS.Domain;
using DynamicBusiness.BPMS.SharedPresentation;
using System.Net.Http;
using System.Net;

namespace DynamicBusiness.BPMS.EngineApi.Controllers
{
    //this controller is used when we want to send html generatd by razor to client.
    public class EngineHtmlElementController : BpmsEngineApiControlBase
    {
        [BpmsAuth]
        [HttpGet]
        public HttpResponseMessage GetDataGridElement(Guid formId, string controlId, Guid? threadId = null)
        {
            sysBpmsDynamicForm dynamicForm = new DynamicFormService().GetInfo(formId);
            DataGridHtml control = (DataGridHtml)dynamicForm.FindControl(controlId);

            EngineSharedModel engineSharedModel = dynamicForm.ProcessId.HasValue ?
                new EngineSharedModel(threadId, dynamicForm.ProcessId, base.MyRequest.GetList(base.IsEncrypted, base.ApiSessionId).ToList(), base.ClientUserName, base.ApiSessionId) :
                new EngineSharedModel(dynamicForm.ApplicationPageID.Value, base.MyRequest.GetList(base.IsEncrypted, base.ApiSessionId).ToList(), base.ClientUserName, base.ApiSessionId);

            control.Helper = HtmlElementHelper.MakeModel(engineSharedModel, new UnitOfWork(), HtmlElementHelperModel.e_FormAction.Onload, dynamicForm);
            control.FillData();

            return Request.CreateResponse(HttpStatusCode.OK, control);
        }

        [BpmsAuth]
        [HttpGet]
        public HttpResponseMessage GetChartElement(Guid formId, string controlId, Guid? threadId = null)
        {
            sysBpmsDynamicForm dynamicForm = new DynamicFormService().GetInfo(formId);
            ChartHtml control = (ChartHtml)dynamicForm.FindControl(controlId);

            EngineSharedModel engineSharedModel = dynamicForm.ProcessId.HasValue ?
                new EngineSharedModel(threadId, dynamicForm.ProcessId, base.MyRequest.GetList(base.IsEncrypted, base.ApiSessionId).ToList(), base.ClientUserName, base.ApiSessionId) :
                new EngineSharedModel(dynamicForm.ApplicationPageID.Value, base.MyRequest.GetList(base.IsEncrypted, base.ApiSessionId).ToList(), base.ClientUserName, base.ApiSessionId);

            control.Helper = HtmlElementHelper.MakeModel(engineSharedModel, new UnitOfWork(), HtmlElementHelperModel.e_FormAction.Onload, dynamicForm);
            control.FillData();

            return Request.CreateResponse(HttpStatusCode.OK, control);
        }

        [BpmsAuth]
        [HttpGet]
        public object GetValue(Guid formId, string controlId, Guid? threadId = null)
        {
            using (DynamicFormService dynamicFormService = new DynamicFormService())
            {
                sysBpmsDynamicForm dynamicForm = dynamicFormService.GetInfo(formId);
                BindingElementBase control = (BindingElementBase)dynamicForm.FindControl(controlId);
                EngineSharedModel engineSharedModel = dynamicForm.ProcessId.HasValue ?
                    new EngineSharedModel(threadId, dynamicForm.ProcessId, base.MyRequest.GetList(base.IsEncrypted, base.ApiSessionId).ToList(), base.ClientUserName, base.ApiSessionId) :
                    new EngineSharedModel(dynamicForm.ApplicationPageID.Value, base.MyRequest.GetList(base.IsEncrypted, base.ApiSessionId).ToList(), base.ClientUserName, base.ApiSessionId);
                UnitOfWork unitOfWork = new UnitOfWork();
                control.Helper = HtmlElementHelper.MakeModel(engineSharedModel, unitOfWork, HtmlElementHelperModel.e_FormAction.Onload, dynamicForm);
                control.FillData();
                return (control.Type == "HTMLCODE" || control.Type == "TITLE") ? control.Label : control.Value;
            }
        }

    }
}