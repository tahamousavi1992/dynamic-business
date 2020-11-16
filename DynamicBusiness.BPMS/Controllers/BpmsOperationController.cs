using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DynamicBusiness.BPMS.BusinessLogic;
using DynamicBusiness.BPMS.SharedPresentation;
using DynamicBusiness.BPMS.Domain;

namespace DynamicBusiness.BPMS.Controllers
{ 
    public class BpmsOperationController : BpmsAdminApiControlBase
    {
        // GET: BpmsOperation
        [HttpGet]
        public object GetList([System.Web.Http.FromUri] OperationIndexSearchDTO indexSearchVM)
        { 
            using (OperationService operationService = new OperationService())
            {
                indexSearchVM.Update(operationService.GetList(null, indexSearchVM.GetPagingProperties).Select(c => new OperationDTO(c)).ToList());
            }
            return indexSearchVM;
        }

        [HttpGet]
        public object GetAddEdit(Guid? ID = null)
        {
            using (OperationService operationService = new OperationService())
            {
                using (LURowService lURowService = new LURowService())
                {
                    OperationDTO operation = new OperationDTO(ID.HasValue ? operationService.GetInfo(ID.Value) : new sysBpmsOperation());
                    operation.Groups = lURowService.GetList(sysBpmsLUTable.e_LUTable.OperationGroupLU.ToString()).Select(c => new LURowDTO(c)).ToList();
                    return operation;
                }
            }
        }

        [HttpPost]
        public object PostAddEdit(OperationDTO OperationDTO)
        {
            //base.SetMenuIndex(5);
            using (OperationService operationService = new OperationService())
            {
                sysBpmsOperation operation = OperationDTO.ID != Guid.Empty ? operationService.GetInfo(OperationDTO.ID) : new sysBpmsOperation();
                ResultOperation resultOperation = operation.Update(OperationDTO.GroupLU, OperationDTO.TypeLU, OperationDTO.SqlCommand, OperationDTO.Name);
                if (resultOperation.IsSuccess)
                {
                    if (operation.ID != Guid.Empty)
                        resultOperation = operationService.Update(operation);
                    else
                        resultOperation = operationService.Add(operation);

                    if (resultOperation.IsSuccess)
                    {
                        return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
                    }
                    else
                        return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
                }
                else
                    return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);

            }
        }

        [HttpDelete]
        public object Delete(Guid ID)
        {
            using (OperationService operationService = new OperationService())
            {
                operationService.Delete(ID);
                return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
            }
        }
    }
}