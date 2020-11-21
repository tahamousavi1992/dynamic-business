using DotNetNuke.Web.Api;
using DynamicBusiness.BPMS.BusinessLogic;
using DynamicBusiness.BPMS.Domain;
using DynamicBusiness.BPMS.SharedPresentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace DynamicBusiness.BPMS.EngineApi.Controllers
{
    public class EngineFormController : BpmsEngineApiControlBase
    {
        [BpmsAuth]
        [HttpGet]
        public List<ComboSearchItem> GetList(Guid? processId = null, string query = "")
        {
            using (DynamicFormService dynamicFormService = new DynamicFormService())
            {
                return dynamicFormService.GetList(processId, null, null, query, null, null).Select(c => new ComboSearchItem(c.ID.ToString(), c.Name)).ToList();
            }
        }
         
        [BpmsAuth]
        [HttpGet]
        public DynamicFormDTO GetInfo(Guid Id)
        {
            using (DynamicFormService dynamicFormService = new DynamicFormService())
            {
                return new DynamicFormDTO(dynamicFormService.GetInfo(Id));
            }
        }

    }
}