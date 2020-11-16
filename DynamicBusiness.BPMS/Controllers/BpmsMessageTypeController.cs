using DynamicBusiness.BPMS.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DynamicBusiness.BPMS.Domain;
using DynamicBusiness.BPMS.SharedPresentation;
using System.Data;
namespace DynamicBusiness.BPMS.Controllers
{
    public class BpmsMessageTypeController : BpmsAdminApiControlBase
    {
        // GET: MessageType
        [HttpGet]
        public object GetList([System.Web.Http.FromUri]MessageTypeIndexSearchDTO indexSearchVM)
        {
            //base.SetMenuIndex(AdminMenuIndex.EntityManagerIndex);
            using (MessageTypeService messageTypeService = new MessageTypeService())
            {
                indexSearchVM.Update(messageTypeService.GetList(indexSearchVM.Name, true, indexSearchVM.GetPagingProperties).Select(c => new MessageTypeDTO(c)).ToList());
                return indexSearchVM;
            }
        }

        [HttpGet]
        public object GetAddEdit(Guid? ID = null)
        {
            using (MessageTypeService messageTypeService = new MessageTypeService())
            {
                return (ID.ToGuidObj() != Guid.Empty ? new MessageTypeDTO(messageTypeService.GetInfo(ID.Value)) : new MessageTypeDTO());
            }
        }

        [HttpPost]
        public object PostAddEdit(MessageTypeDTO MessageTypeDTO)
        {
            if (ModelState.IsValid)
            {
                using (MessageTypeService messageTypeService = new MessageTypeService())
                {
                    sysBpmsMessageType messageType = MessageTypeDTO.ID != Guid.Empty ? messageTypeService.GetInfo(MessageTypeDTO.ID) : new sysBpmsMessageType();
                    ResultOperation resultOperation = messageType.Update(MessageTypeDTO.Name, true, MessageTypeDTO.ListParameter);
                    if (!resultOperation.IsSuccess)
                        return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);

                    if (messageType.ID != Guid.Empty)
                        resultOperation = messageTypeService.Update(messageType);
                    else
                        resultOperation = messageTypeService.Add(messageType);

                    if (resultOperation.IsSuccess)
                        return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
                    else
                        return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
                }
            }
            else
                return new PostMethodMessage(SharedLang.Get("NotFound.Text"), DisplayMessageType.error);
        }

        [HttpGet]
        public object GetInActive(Guid ID)
        {
            using (MessageTypeService messageTypeService = new MessageTypeService())
            {
                ResultOperation resultOperation = messageTypeService.InActive(ID);
                if (resultOperation.IsSuccess)
                    return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
                else
                    return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
            }
        }

        [HttpGet]
        public object GetActive(Guid ID)
        {
            using (MessageTypeService messageTypeService = new MessageTypeService())
            {
                ResultOperation resultOperation = messageTypeService.Active(ID);
                if (resultOperation.IsSuccess)
                    return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
                else
                    return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
            }
        }
    }
}