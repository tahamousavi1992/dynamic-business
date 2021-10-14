using DynamicBusiness.BPMS.BusinessLogic;
using DynamicBusiness.BPMS.Domain;
using DynamicBusiness.BPMS.SharedPresentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DynamicBusiness.BPMS.Cartable.Controllers
{
    public class CartableManageController : BpmsCartableApiControlBase
    {
        //It retrieves open threads which are in person kartable or step.
        public object GetIndex([System.Web.Http.FromUri] KartableIndexSearchDTO indexSearchVM)
        {
            //base.SetMenuIndex(1);
            using (ThreadTaskService threadTaskService = new ThreadTaskService())
            {
                List<sysBpmsThreadTask> list = threadTaskService.GetListKartable(MyUser.ID, new int[] {
            (int)sysBpmsThreadTask.e_StatusLU.New,
            (int)sysBpmsThreadTask.e_StatusLU.Ongoing }, indexSearchVM.GetPagingProperties);
                indexSearchVM.Update(list.Select(c => new KartableDTO(c)).ToList());
                return indexSearchVM;
            }
        }
        [HttpGet]
        //it retrieves all threads which person has access to.
        public object GetThreadIndex([System.Web.Http.FromUri] ThreadIndexSearchDTO indexSearchVM)
        {
            //base.SetMenuIndex(3);
            using (ThreadService threadService = new ThreadService())
            {

                Guid? ProcessID = indexSearchVM.IsAdvSearch ? indexSearchVM.AdvProcessID : indexSearchVM.ProcessID;

                DateTime? advStartDateFrom = indexSearchVM.IsAdvSearch ? indexSearchVM.AdvStartDateFrom : (DateTime?)null;

                DateTime? advStartDateTo = indexSearchVM.IsAdvSearch ? indexSearchVM.AdvStartDateTo : (DateTime?)null;

                DateTime? advEndDateFrom = indexSearchVM.IsAdvSearch ? indexSearchVM.AdvEndDateFrom : (DateTime?)null;

                DateTime? advEndDateTo = indexSearchVM.IsAdvSearch ? indexSearchVM.AdvEndDateTo : (DateTime?)null;

                List<sysBpmsThread> list = threadService.GetArchiveList(MyUser.ID, ProcessID, new int[] {
            (int)sysBpmsThread.Enum_StatusLU.InProgress,
            (int)sysBpmsThread.Enum_StatusLU.Done,
            (int)sysBpmsThread.Enum_StatusLU.Draft,
            (int)sysBpmsThread.Enum_StatusLU.InActive}, null, advStartDateFrom, advStartDateTo, advEndDateFrom, advEndDateTo, indexSearchVM.GetPagingProperties, new string[] { nameof(sysBpmsThread.User), nameof(sysBpmsThread.Process) });

                using (ProcessService processService = new ProcessService())
                {
                    indexSearchVM.Update(
                        processService.GetList(null, null, null, null).Where(c => c.StatusLU != (int)sysBpmsProcess.Enum_StatusLU.Draft),
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
                          new string[] { nameof(sysBpmsThread.User), nameof(sysBpmsThread.Process) }),
                          threadTaskService.GetList(ThreadID, (int)sysBpmsTask.e_TypeLU.UserTask, null, null, new string[] { $"{nameof(sysBpmsThreadTask.Task)}.{nameof(sysBpmsThreadTask.Task.Element)}", nameof(sysBpmsThreadTask.User) }).Select(c => new ThreadHistoryDTO(c)).ToList());

                        List<sysBpmsDynamicForm> listForms = dynamicFormService.GetList(threadDetailDTO.ProcessID, null, null, "", true, null);
                        using (ProcessEngine processEngine = new ProcessEngine(new EngineSharedModel(ThreadID, threadDetailDTO.ProcessID, this.MyRequest.GetList(false, base.ApiSessionId).ToList(), base.ClientUserName, base.ApiSessionId)))
                        {
                            foreach (var item in listForms)
                            {
                                var result = processEngine.GetContentHtmlByFormID(item.ID, true);
                                EngineFormModel engineFormModel = new EngineFormModel(result.FormModel, ThreadID, null, threadDetailDTO.ProcessID);
                                string popUpUrl = UrlUtility.GetCartableApiUrl(base.MyRequest, base.PortalSettings.DefaultPortalAlias, nameof(CartableThreadController.GetPopUp), nameof(CartableThreadController), "");
                                engineFormModel.SetReadOnlyUrls(popUpUrl, new HttpRequestWrapper(base.MyRequest), base.PortalSettings.DefaultPortalAlias, FormTokenUtility.GetFormToken(base.ApiSessionId, engineFormModel?.FormModel?.ContentHtml?.DynamicFormID ?? Guid.Empty, engineFormModel?.FormModel?.IsEncrypted ?? false));
                                threadDetailDTO.ListOverviewForms.Add(engineFormModel);
                            }
                        }
                        return threadDetailDTO;
                    }
                }
            }
        }

        [HttpGet]
        public object GetListProcess([System.Web.Http.FromUri] GetListProcessSearchDTO indexSearchVM)
        {

            using (ProcessEngine processEngine = new ProcessEngine(new EngineSharedModel(currentThread: null, currentProcessID: Guid.Empty, baseQueryModel: base.MyRequest.GetList(false, base.ApiSessionId).ToList(), currentUserName: base.ClientUserName, apiSessionId: base.ApiSessionId)))
            {
                List<KartableProcessDTO> list = processEngine.GetAvailableProccess(this.MyUser.ID).Select(c => new KartableProcessDTO(c)).ToList();
                indexSearchVM.Update(list);
                return indexSearchVM;
            }
        }

        [HttpGet]
        public object GetBeginTask(Guid ProcessID)
        {
            using (ThreadTaskService threadTaskService = new ThreadTaskService())
            {
                (ResultOperation result, List<MessageModel> msgModel) = new ProcessEngine(new EngineSharedModel(currentThread: null, currentProcessID: ProcessID, baseQueryModel: base.MyRequest.GetList(false, base.ApiSessionId).ToList(), currentUserName: base.ClientUserName, apiSessionId: base.ApiSessionId)).StartProcess(base.MyUser.ID);
                if (result.IsSuccess)
                {
                    sysBpmsThreadTask threadTask = threadTaskService.GetList(((sysBpmsThread)result.CurrentObject).ID, (int)sysBpmsTask.e_TypeLU.UserTask, null, (int)sysBpmsThreadTask.e_StatusLU.New).LastOrDefault();
                    return new
                    {
                        MessageList = msgModel,
                        ThreadTaskID = threadTask.ID,
                        Result = true,
                    };
                }
                else
                {
                    return new
                    {
                        MessageList = new List<MessageModel>() { new MessageModel(DisplayMessageType.error, result.GetErrors()) },
                        Result = false,
                    };
                }
            }
        }

        [HttpGet]
        public object GetPagesForMenu()
        {

            List<sysBpmsApplicationPage> list = new ApplicationPageEngine(new EngineSharedModel(Guid.Empty, null, this.ClientUserName, "")).GetAvailable(null, this.ClientUserName, true);
            using (LURowService luRowService = new LURowService())
                return list.GroupBy(c => c.GroupLU).Select(c => new
                {
                    Group = c.Key,
                    GroupName = luRowService.GetNameOfByAlias(sysBpmsLUTable.e_LUTable.ApplicationPageGroupLU.ToString(), c.Key.ToString()),
                    List = c.ToList().Select(d => new
                    {
                        d.ID,
                        Name = d.DynamicForms.FirstOrDefault()?.Name ?? "no page"
                    }),
                }).ToList();
        }

    }
}