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
    public class BpmsDbManagerController : BpmsAdminApiControlBase
    {
        [HttpGet]
        public object GetList([System.Web.Http.FromUri] DbManagerIndexSearchDTO indexSearchVM)
        {
            using (DBConnectionService dBConnectionService = new DBConnectionService())
            {
                indexSearchVM.Update(dBConnectionService.GetList(indexSearchVM.IsAdvSearch ? indexSearchVM.AdvName : indexSearchVM.Name,
                    indexSearchVM.IsAdvSearch ? indexSearchVM.AdvDataSource : string.Empty, string.Empty, indexSearchVM.GetPagingProperties).Select(c => new DBConnectionDTO(c)).ToList());
                return indexSearchVM;
            }
        }

        [HttpGet]
        public object GetAddEdit(Guid ID)
        {
            using (DBConnectionService dBConnectionService = new DBConnectionService())
            {
                return ID != Guid.Empty ? new DBConnectionDTO(dBConnectionService.GetInfo(ID)) : new DBConnectionDTO();
            }
        }

        [HttpPost]
        public object PostAddEdit(DBConnectionDTO DBConnectionDTO)
        {
            sysBpmsDBConnection dBConnection = new sysBpmsDBConnection();
            ResultOperation resultOperation = dBConnection.Update(DBConnectionDTO.ID, DBConnectionDTO.Name, DBConnectionDTO.DataSource, DBConnectionDTO.InitialCatalog, DBConnectionDTO.UserID, DBConnectionDTO.Password, DBConnectionDTO.IntegratedSecurity);
            if (!resultOperation.IsSuccess)
                return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);

            using (DBConnectionService dBConnectionService = new DBConnectionService())
            {
                if (dBConnection.ID != Guid.Empty)
                    resultOperation = dBConnectionService.Update(dBConnection);
                else
                    resultOperation = dBConnectionService.Add(dBConnection);

                if (resultOperation.IsSuccess)
                    return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
                else
                    return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
            }
        }

        [HttpDelete]
        public object Delete(Guid ID)
        {
            using (DBConnectionService dBConnectionService = new DBConnectionService())
            {

                ResultOperation resultOperation = dBConnectionService.Delete(ID);
                if (resultOperation.IsSuccess)
                    return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
                return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
            }
        }

        [HttpPost]
        public object TestConnectionString(DBConnectionDTO DBConnectionDTO)
        {
            using (DBConnectionService dBConnectionService = new DBConnectionService())
            {
                sysBpmsDBConnection dBConnection = new sysBpmsDBConnection();
                ResultOperation resultOperation = dBConnection.Update(DBConnectionDTO.ID, DBConnectionDTO.Name, DBConnectionDTO.DataSource, DBConnectionDTO.InitialCatalog, DBConnectionDTO.UserID, DBConnectionDTO.Password, DBConnectionDTO.IntegratedSecurity);
                if (resultOperation.IsSuccess)
                {
                    if (dBConnectionService.TestConnection(dBConnection))
                        return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
                    else
                        return new PostMethodMessage("Error while testing", DisplayMessageType.error);
                }
                else
                    return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
            }
        }

    }
}