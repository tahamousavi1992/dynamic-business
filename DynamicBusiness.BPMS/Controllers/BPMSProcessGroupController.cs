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
using System.Web.Script.Serialization;
using DynamicBusiness.BPMS.SharedPresentation;

namespace DynamicBusiness.BPMS.Controllers
{
    public class BpmsProcessGroupController : BpmsAdminApiControlBase
    {
        public BpmsProcessGroupController()
        {

        }
        [HttpGet]
        public object GetList(Guid? SelectedID = null)
        {
            using (ProcessGroupService processGroupService = new ProcessGroupService())
                return Json(new
                {
                    SelectedID,
                    GroupList = processGroupService.HelperGetTreeList(SelectedID)
                });
        }

        [HttpGet]
        public object GetAddEdit(Guid? ID = null, Guid? ParentID = null)
        {
            if (ParentID.HasValue)
                return new ProcessGroupDTO() { ProcessGroupID = ParentID };
            else
            {
                using (ProcessGroupService ProcessGroupService = new ProcessGroupService())
                    return new ProcessGroupDTO(ID.ToGuidObj() != Guid.Empty ? ProcessGroupService.GetInfo(ID.Value) : null);
            }
        }

        [HttpPost]
        public object PostAddEdit(ProcessGroupDTO processGroupDTO)
        {
            using (ProcessGroupService ProcessGroupService = new ProcessGroupService())
            {
                sysBpmsProcessGroup ProcessGroup = processGroupDTO.ID != Guid.Empty ? ProcessGroupService.GetInfo(processGroupDTO.ID) : new sysBpmsProcessGroup();
                ResultOperation resultOperation = ProcessGroup.Update(processGroupDTO.ProcessGroupID, processGroupDTO.Name, processGroupDTO.Description);

                if (!resultOperation.IsSuccess)
                    return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
                if (processGroupDTO.ID != Guid.Empty)
                    resultOperation = ProcessGroupService.Update(ProcessGroup);
                else
                    resultOperation = ProcessGroupService.Add(ProcessGroup);

                if (resultOperation.IsSuccess)
                    return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
                else
                    return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
            }
        }

        [HttpPost]
        public object Delete(Guid ID)
        {
            using (ProcessGroupService ProcessGroupService = new ProcessGroupService())
            {
                ResultOperation resultOperation = ProcessGroupService.Delete(ID);
                if (resultOperation.IsSuccess)
                    return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
                else
                    return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
            }
        }
    }
}