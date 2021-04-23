using DotNetNuke.Web.Mvc.Framework.Controllers;
using DotNetNuke.Web.Mvc.Routing;
using DynamicBusiness.BPMS.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using DynamicBusiness.BPMS.Domain;
using DynamicBusiness.BPMS.SharedPresentation;
using System.Web.Script.Serialization;

namespace DynamicBusiness.BPMS.Controllers
{
    public class BpmsProcessController : BpmsAdminApiControlBase
    {
        // GET: /BpmsProcess/
        public object GetList([System.Web.Http.FromUri] ProcessIndexSearchDTO indexSearchVM)
        {

            using (ProcessService processService = new ProcessService())
                indexSearchVM.Update(processService.GetList(null, null, indexSearchVM.SelectedID, indexSearchVM.GetPagingProperties).Select(c => new ProcessDTO(c)).ToList());
            return indexSearchVM;

        }

        [HttpGet]
        public object GetAdd(Guid ProcessGroupID)
        {
            return new ProcessDTO()
            {
                TypeLU = (int)sysBpmsProcess.e_TypeLU.General,
                StatusLU = (int)sysBpmsProcess.Enum_StatusLU.Draft,
                ProcessGroupID = ProcessGroupID,
                ListTypes = EnumObjHelper.GetEnumList<sysBpmsProcess.e_TypeLU>().Select(c => new QueryModel(c.Key.ToString(), c.Value)).ToList()
            };
        }

        [HttpPost]
        public object PostAdd(ProcessDTO processDTO)
        {
            using (ProcessService processService = new ProcessService())
            {
                ResultOperation resultOperation = processService.Add(processDTO.Name, processDTO.Description, base.userName, processDTO.ParallelCountPerUser, processDTO.ProcessGroupID, processDTO.TypeLU);
                return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success, ((sysBpmsProcess)resultOperation.CurrentObject).ID.ToString());
            }
        }

        [HttpGet]
        public object GetPublish(Guid ID)
        {
            using (ProcessService processService = new ProcessService())
            {
                ResultOperation resultOperation = processService.Publish(ID);
                if (resultOperation.IsSuccess)
                    return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
                else
                    return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
            }
        }

        [HttpGet]
        public object GetInActive(Guid ID)
        {
            using (ProcessService processService = new ProcessService())
            {
                sysBpmsProcess process = processService.GetInfo(ID);
                ResultOperation resultOperation = processService.InActive(process);
                if (resultOperation.IsSuccess)
                    return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
                else
                    return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
            }
        }

        [HttpGet]
        public object GetActive(Guid ID)
        {
            using (ProcessService processService = new ProcessService())
            {
                sysBpmsProcess process = processService.GetInfo(ID);
                ResultOperation resultOperation = processService.Publish(ID);
                if (resultOperation.IsSuccess)
                    return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
                else
                    return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
            }
        }

        [HttpDelete]
        public object Delete(Guid ID)
        {
            using (ProcessService processService = new ProcessService())
            {
                ResultOperation resultOperation = processService.Delete(ID);
                if (resultOperation.IsSuccess)
                    return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
                else
                    return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
            }
        }

        [HttpGet]
        public object GetNewVersion(Guid ID)
        {
            using (ProcessService processService = new ProcessService())
            {
                ResultOperation resultOperation = processService.NewVersion(processService.GetInfo(ID), base.userName);
                if (resultOperation.IsSuccess)
                    return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
                else
                    return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
            }
        }
    }
}