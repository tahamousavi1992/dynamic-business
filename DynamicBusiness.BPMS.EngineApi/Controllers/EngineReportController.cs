using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DynamicBusiness.BPMS.BusinessLogic;
using DynamicBusiness.BPMS.Domain;
using DynamicBusiness.BPMS.SharedPresentation;
namespace DynamicBusiness.BPMS.EngineApi.Controllers
{
    public class EngineReportController : BpmsEngineApiControlBase
    {
        [BpmsAuth]
        [HttpGet]
        public void GetDataGridReport(Guid formId, string controlId, string format, Guid? threadId = null)
        {
            sysBpmsDynamicForm dynamicForm = new DynamicFormService().GetInfo(formId);
            DataGridHtml control = (DataGridHtml)dynamicForm.FindControl(controlId);

            EngineSharedModel engineSharedModel = dynamicForm.ProcessId.HasValue ?
                new EngineSharedModel(threadId, dynamicForm.ProcessId, base.MyRequest.GetList(base.IsEncrypted, base.ApiSessionId).ToList(), base.ClientUserName, base.ApiSessionId) :
                new EngineSharedModel(dynamicForm.ApplicationPageID.Value, base.MyRequest.GetList(base.IsEncrypted, base.ApiSessionId).ToList(), base.ClientUserName, base.ApiSessionId);

            control.Helper = HtmlElementHelper.MakeModel(engineSharedModel, new UnitOfWork(), HtmlElementHelperModel.e_FormAction.Onload, dynamicForm);
            control.FillDataItem(true);

            using (ReportEngine reportEngine = new ReportEngine(engineSharedModel, new UnitOfWork()))
            {
                reportEngine.PrintRdlcDataGrid(HttpContext.Current.Response, control, format == "pdf" ? DomainUtility.ReportExportType.PDF : DomainUtility.ReportExportType.Excel);
            }
         
        }
    }
}