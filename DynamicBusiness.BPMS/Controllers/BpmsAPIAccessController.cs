﻿using DynamicBusiness.BPMS.BusinessLogic;
using DynamicBusiness.BPMS.Domain;
using DynamicBusiness.BPMS.SharedPresentation;
using System;
using System.Linq;
using System.Web.Mvc;

namespace DynamicBusiness.BPMS.Controllers
{
    public class BpmsAPIAccessController : BpmsAdminApiControlBase
    {
        [HttpGet]
        public object GetList([System.Web.Http.FromUri] APIAccessIndexSearchDTO indexSearchVM)
        {
            using (APIAccessService apiAccessService = new APIAccessService())
            {
                indexSearchVM.Update(apiAccessService.GetList("", "", "", null, indexSearchVM.GetPagingProperties).Select(c => new APIAccessDTO(c)).ToList());
                return indexSearchVM;
            }
        }

        [HttpGet]
        public object GetAddEdit(Guid? ID = null)
        {
            using (APIAccessService apiAccessService = new APIAccessService())
            {
                APIAccessDTO APIAccessDTO = ID.HasValue ? new APIAccessDTO(apiAccessService.GetInfo(ID.Value)) : new APIAccessDTO() { IsActive = true };
                return APIAccessDTO;
            }
        }

        [HttpPost]
        public object PostAddEdit(APIAccessDTO APIAccessDTO)
        {
            using (APIAccessService apiAccessService = new APIAccessService())
            {
                ResultOperation resultOperation = null;
                sysBpmsAPIAccess apiAccess = new sysBpmsAPIAccess().Update(APIAccessDTO.Name, APIAccessDTO.IPAddress, APIAccessDTO.AccessKey, APIAccessDTO.IsActive);
                apiAccess.ID = APIAccessDTO.ID;
                if (apiAccess.ID != Guid.Empty)
                    resultOperation = apiAccessService.Update(apiAccess);
                else
                    resultOperation = apiAccessService.Add(apiAccess);

                if (resultOperation.IsSuccess)
                    return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
                else
                    return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
            }
        }

        [HttpGet]
        public object GetInActive(Guid ID)
        {
            using (APIAccessService apiAccessService = new APIAccessService())
            {
                sysBpmsAPIAccess apiAccess = apiAccessService.GetInfo(ID);
                apiAccess.Update(false);
                ResultOperation resultOperation = apiAccessService.Update(apiAccess);
            }
            return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
        }

        [HttpDelete]
        public object Delete(Guid ID)
        {
            using (APIAccessService apiAccessService = new APIAccessService())
            {
                ResultOperation resultOperation = apiAccessService.Delete(ID);
                if (resultOperation.IsSuccess)
                    return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
                else
                    return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
            }

        }


        [HttpGet]
        public object GetActive(Guid ID)
        {
            using (APIAccessService apiAccessService = new APIAccessService())
            {
                sysBpmsAPIAccess apiAccess = apiAccessService.GetInfo(ID);
                apiAccess.Update(true);
                ResultOperation resultOperation = apiAccessService.Update(apiAccess);
            }
            return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
        }
    }
}