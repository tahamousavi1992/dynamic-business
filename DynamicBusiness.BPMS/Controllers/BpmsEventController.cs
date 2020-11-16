using DynamicBusiness.BPMS.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using DynamicBusiness.BPMS.Domain;
using DynamicBusiness.BPMS.SharedPresentation;
using System.Web.UI.WebControls;

namespace DynamicBusiness.BPMS.Controllers
{
    public class BpmsEventController : BpmsAdminApiControlBase
    {
        // GET: BpmsEvent
        public object GetIndex(Guid ProcessId, string ElementId)
        {
            EventDTO eventDTO = null;
            using (EventService eventService = new EventService())
            {
                eventDTO = new EventDTO(eventService.GetInfo(ElementId, ProcessId));
            }
            //Filling Default Message ViewBag 
            using (MessageTypeService messageTypeService = new MessageTypeService())
            {

                if (eventDTO.SubTypeMessageEventModel?.Type == (int)SubTypeMessageEventModel.e_Type.Message &&
                    eventDTO.MessageTypeID.HasValue)
                {
                    //If messageType was changed, it adds or removes extra params. 
                    List<MessageTypeParamsModel> listMTParams = messageTypeService.GetInfo(eventDTO.MessageTypeID.Value).ParamsXmlModel;

                    //update list of parameter
                    eventDTO.SubTypeMessageEventModel.MessageParams = listMTParams.Select(c =>
                    new SubTypeMessageParamEventModel(eventDTO.SubTypeMessageEventModel.MessageParams.FirstOrDefault(d => d.Name == c.Name)?.Variable, c.Name, c.IsRequired)).ToList();
                }

                using (EmailAccountService emailAccountService = new EmailAccountService())
                {
                    List<QueryModel> list = new List<QueryModel>() {
                          new QueryModel(((int)SubTypeEmailEventModel.e_FromType.CurrentUser).ToString(),"Current User"),
                          new QueryModel(((int)SubTypeEmailEventModel.e_FromType.CurrentThreadUser).ToString(),"Requested User")
                         };
                    return new
                    {
                        SubTypeTimerEventModelIntervalTypes = EnumObjHelper.GetEnumList<SubTypeTimerEventModel.e_IntervalType>().Select(c => new QueryModel(c.Key.ToString(), c.Value)).ToList(),
                        SubTypeTimerEventModelSetTypes = EnumObjHelper.GetEnumList<SubTypeTimerEventModel.e_SetType>().Select(c => new QueryModel(c.Key.ToString(), c.Value)).ToList(),
                        SubTypeTimerEventModelTypes = EnumObjHelper.GetEnumList<SubTypeTimerEventModel.e_Type>().Where(c => eventDTO.TypeLU == (int)sysBpmsEvent.e_TypeLU.StartEvent || eventDTO.TypeLU == (int)sysBpmsEvent.e_TypeLU.boundary || c.Key != (int)SubTypeTimerEventModel.e_Type.Interval).Select(c => new QueryModel(c.Key.ToString(), c.Value)).ToList(),
                        SubTypeEmailEventModelToSystemicTypes = EnumObjHelper.GetEnumList<SubTypeEmailEventModel.e_ToSystemicType>().Select(c => new QueryModel(c.Key.ToString(), c.Value)).ToList(),
                        SubTypeEmailEventModelToTypes = EnumObjHelper.GetEnumList<SubTypeEmailEventModel.e_ToType>().Select(c => new QueryModel(c.Key.ToString(), c.Value)).ToList(),
                        SubTypeMessageEventModelKeyTypes = EnumObjHelper.GetEnumList<SubTypeMessageEventModel.e_KeyType>().Select(c => new QueryModel(c.Key.ToString(), c.Value)).ToList(),
                        SubTypeMessageEventModelTypes = EnumObjHelper.GetEnumList<SubTypeMessageEventModel.e_Type>().Select(c => new QueryModel(c.Key.ToString(), c.Value)).ToList(),
                        //eventDTO.MessageTypeID
                        MessageTypes = messageTypeService.GetList("", true).Select(c => new QueryModel(c.ID.ToString(), c.Name)).ToList(),
                        //eventDTO.SubTypeMessageEventModel?.Email?.From
                        EmailAccounts = emailAccountService.GetList((int)sysBpmsEmailAccount.e_ObjectTypeLU.Systemic, null, null).
                        Select(c => new QueryModel(c.ID.ToString(), c.Email)).Union(list).ToList(),
                        Model = eventDTO,
                    };
                }
            }
        }

        [HttpPost]
        public object PostSubTypeEmail(PostSubTypeEmailDTO model)
        {

            model.SubTypeMessageEventModel.Type = (int)SubTypeMessageEventModel.e_Type.Email;
            model.SubTypeMessageEventModel.MessageParams = null;
            using (EventService eventService = new EventService())
            {
                switch ((SubTypeEmailEventModel.e_ToType)model.SubTypeMessageEventModel.Email.ToType)
                {
                    case SubTypeEmailEventModel.e_ToType.Static:
                        model.SubTypeMessageEventModel.Email.To = model.ToStatic;
                        break;
                    case SubTypeEmailEventModel.e_ToType.Systemic:
                        model.SubTypeMessageEventModel.Email.To = model.ToSystemic;
                        break;
                    case SubTypeEmailEventModel.e_ToType.Variable:
                        model.SubTypeMessageEventModel.Email.To = model.ToVariable;
                        break;
                }

                sysBpmsEvent _event = eventService.GetInfo(model.ID);
                ResultOperation resultOperation = _event.Update(model.TypeLU, _event.RefElementID, model.SubTypeMessageEventModel.BuildXml(), model.SubType, _event.CancelActivity, null);
                if (!resultOperation.IsSuccess)
                    return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
                resultOperation = eventService.Update(_event);
                if (resultOperation.IsSuccess)
                    return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
                else
                    return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
            }

        }

        [HttpPost]
        public object PostSaveSubTypeMessage(PostSaveSubTypeMessageDTO model)
        {

            model.SubTypeMessageEventModel.Type = (int)SubTypeMessageEventModel.e_Type.Message;
            model.SubTypeMessageEventModel.Email = null;

            using (EventService eventService = new EventService())
            {
                switch ((SubTypeMessageEventModel.e_KeyType)model.SubTypeMessageEventModel.KeyType)
                {
                    case SubTypeMessageEventModel.e_KeyType.Static:
                        model.SubTypeMessageEventModel.Key = model.KeyStatic;
                        break;
                    case SubTypeMessageEventModel.e_KeyType.Variable:
                        model.SubTypeMessageEventModel.Key = model.KeyVariable;
                        break;
                }
                model.SubTypeMessageEventModel.MessageParams = model.ListParameter;
                sysBpmsEvent _event = eventService.GetInfo(model.ID);
                ResultOperation resultOperation = _event.Update(model.TypeLU, _event.RefElementID, model.SubTypeMessageEventModel.BuildXml(), model.SubType, _event.CancelActivity, model.MessageTypeID);
                if (!resultOperation.IsSuccess)
                    return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
                resultOperation = eventService.Update(_event);
                if (resultOperation.IsSuccess)
                    return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
                else
                    return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
            }

        }

        [HttpPost]
        public object PostSubTypeTimer(EventDTO EventDTO)
        {

            using (EventService eventService = new EventService())
            {
                EventDTO.SubTypeTimerEventModel.SetProperties();
                sysBpmsEvent _event = eventService.GetInfo(EventDTO.ID);
                ResultOperation resultOperation = _event.Update(EventDTO.TypeLU, _event.RefElementID, EventDTO.SubTypeTimerEventModel.BuildXml(), EventDTO.SubType, _event.CancelActivity, null);
                if (!resultOperation.IsSuccess)
                    return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
                resultOperation = eventService.Update(_event);
                if (resultOperation.IsSuccess)
                    return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
                else
                    return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
            }

        }

        [HttpGet]
        public object GetThrowMessageParams(Guid MessageTypeID)
        {
            using (MessageTypeService messageTypeService = new MessageTypeService())
                return new MessageTypeDTO(messageTypeService.GetInfo(MessageTypeID)).ListParameter;
        }

    }
}