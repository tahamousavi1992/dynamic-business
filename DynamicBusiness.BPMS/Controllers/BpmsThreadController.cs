using DynamicBusiness.BPMS.BusinessLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using DynamicBusiness.BPMS.Domain;
using DynamicBusiness.BPMS.SharedPresentation;

namespace DynamicBusiness.BPMS.Controllers
{ 
    public class BpmsThreadController : BpmsAdminApiControlBase
    {
        //it retrieves all threads which person has access to.
        public object GetList([System.Web.Http.FromUri] ThreadIndexSearchDTO indexSearchVM)
        {
            using (ThreadService threadService = new ThreadService())
            {
                Guid? ProcessID = indexSearchVM.IsAdvSearch ? indexSearchVM.AdvProcessID : indexSearchVM.ProcessID;

                DateTime? advStartDateFrom = indexSearchVM.IsAdvSearch ? indexSearchVM.AdvStartDateFrom : (DateTime?)null;

                DateTime? advStartDateTo = indexSearchVM.IsAdvSearch ? indexSearchVM.AdvStartDateTo : (DateTime?)null;

                DateTime? advEndDateFrom = indexSearchVM.IsAdvSearch ? indexSearchVM.AdvEndDateFrom : (DateTime?)null;

                DateTime? advEndDateTo = indexSearchVM.IsAdvSearch ? indexSearchVM.AdvEndDateTo : (DateTime?)null;

                List<sysBpmsThread> list = threadService.GetArchiveList(null, ProcessID, null, null, advStartDateFrom, advStartDateTo, advEndDateFrom, advEndDateTo, indexSearchVM.GetPagingProperties, new string[] { nameof(sysBpmsThread.sysBpmsUser), nameof(sysBpmsThread.sysBpmsProcess) });

                using (ProcessService processService = new ProcessService())
                {
                    indexSearchVM.Update(
                        processService.GetList(null, null, null, null).Where(c => c.StatusLU != (int)sysBpmsProcess.Enum_StatusLU.Draft).ToList(),
                        list.Select(c => new ThreadDTO(c)).ToList());
                    return indexSearchVM;
                }
            }
        }

        [HttpGet]
        public object GetThreadDetail(Guid ThreadID)
        {
            using (ThreadService threadService = new ThreadService())
            {
                using (ThreadTaskService threadTaskService = new ThreadTaskService())
                {
                    using (DynamicFormService dynamicFormService = new DynamicFormService())
                    {
                        ThreadDetailDTO threadDetailDTO = new ThreadDetailDTO(
                          threadService.GetInfo(ThreadID,
                          new string[] { nameof(sysBpmsThread.sysBpmsUser), nameof(sysBpmsThread.sysBpmsProcess) }),
                          threadTaskService.GetList(ThreadID, (int)sysBpmsTask.e_TypeLU.UserTask, null, null, new string[] { "sysBpmsTask.sysBpmsElement", nameof(sysBpmsThreadTask.sysBpmsUser) }).Select(c => new ThreadHistoryDTO(c)).ToList());

                        List<sysBpmsDynamicForm> listForms = dynamicFormService.GetList(threadDetailDTO.ProcessID, null, null, "", true, null);
                        using (ProcessEngine processEngine = new ProcessEngine(new EngineSharedModel(ThreadID, threadDetailDTO.ProcessID, this.MyRequest.GetList(false, HttpContext.Current.Session.SessionID).ToList(), base.UserInfo.Username, HttpContext.Current.Session.SessionID)))
                        {
                            foreach (var item in listForms)
                            {
                                var result = processEngine.GetContentHtmlByFormID(item.ID, true);
                                EngineFormModel engineFormModel = new EngineFormModel(result.FormModel, ThreadID, null, threadDetailDTO.ProcessID);
                                engineFormModel.GetPopUpUrl = UrlUtility.GetCartableApiUrl(base.MyRequest, base.PortalSettings.DefaultPortalAlias, "GetPopUp", "CartableThread", "");
                                threadDetailDTO.ListOverviewForms.Add(engineFormModel);
                            }
                        }
                        return threadDetailDTO;
                    }
                }
            }
        }

        [HttpDelete]
        public object Delete(string ID)
        {
            using (ThreadService threadService = new ThreadService())
            {
                ResultOperation resultOperation = new ResultOperation();
                foreach (Guid threadId in ID.ToStringObj().Split(',').Where(c => c.ToGuidObjNull().HasValue).Select(c => c.ToGuidObj()).ToList())
                {
                    resultOperation = threadService.Delete(threadId);
                    if (!resultOperation.IsSuccess)
                        break;
                }
                if (resultOperation.IsSuccess)
                    return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
                else
                    return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
            }
        }


    }
}