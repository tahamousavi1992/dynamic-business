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
    public class BpmsLURowController : BpmsAdminApiControlBase
    {
        [HttpGet]
        public object GetList([System.Web.Http.FromUri] LURowIndexSearchDTO indexSearchVM)
        {
            using (LURowService luRowService = new LURowService())
            {
                indexSearchVM.Update(indexSearchVM.LUTableID.HasValue ? luRowService.GetList(indexSearchVM.LUTableID.Value, "", true, indexSearchVM.GetPagingProperties).Select(c => new LURowDTO(c)).ToList() : new List<LURowDTO>());
                using (LUTableService luTableService = new LUTableService())
                {
                    indexSearchVM.Update(luTableService.GetList());
                }
                return indexSearchVM;
            }
        }

        [HttpGet]
        public object GetAddEdit(Guid? ID = null, Guid? LUTableID = null)
        {
            using (LURowService luRowService = new LURowService())
            {
                using (LUTableService luTableService = new LUTableService())
                {
                    sysBpmsLURow lURow = !ID.HasValue ?
                        new sysBpmsLURow().Update(LUTableID.Value, "", (luRowService.MaxCodeOfByLUTableID(LUTableID.Value) + 1).ToStringObj(), luRowService.MaxOrderByLUTableID(LUTableID.Value) + 1, false, true) :
                        luRowService.GetInfo(ID.Value);
                    return new LURowDTO(lURow);
                }
            }
        }

        [HttpPost]
        public object PostAddEdit(LURowDTO lURowDTO)
        {
            using (LURowService luRowService = new LURowService())
            {
                ResultOperation resultOperation = null;
                sysBpmsLURow lURow = new sysBpmsLURow().Update(lURowDTO.LUTableID, lURowDTO.NameOf, lURowDTO.CodeOf, lURowDTO.DisplayOrder, lURowDTO.IsSystemic, lURowDTO.IsActive);
                lURow.ID = lURowDTO.ID;
                if (lURow.ID != Guid.Empty)
                    resultOperation = luRowService.Update(lURow);
                else
                    resultOperation = luRowService.Add(lURow);

                if (resultOperation.IsSuccess)
                    return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
                else
                    return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
            }
        }

        [HttpGet]
        public object GetInActive(Guid ID)
        {
            using (LURowService luRowService = new LURowService())
            {
                sysBpmsLURow lURow = luRowService.GetInfo(ID);
                lURow.Update(false);
                ResultOperation resultOperation = luRowService.Update(lURow);
                if (resultOperation.IsSuccess)
                    return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
                return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
            }
        }
    }
}