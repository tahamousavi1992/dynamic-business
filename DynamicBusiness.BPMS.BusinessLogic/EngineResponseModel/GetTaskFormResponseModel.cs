using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    [DataContract]
    public class GetTaskFormResponseModel
    {
        public GetTaskFormResponseModel() { }
        public GetTaskFormResponseModel(EngineFormModel engineFormModel, List<MessageModel> listMessageModel, RedirectUrlModel redirectUrlModel)
        {
            this.EngineFormModel = engineFormModel;
            this.ListMessageModel = listMessageModel;
            this.RedirectUrlModel = redirectUrlModel;
        }
        [DataMember]
        public EngineFormModel EngineFormModel { get; set; }
        [DataMember]
        public List<MessageModel> ListMessageModel { get; set; }
        [DataMember]
        public RedirectUrlModel RedirectUrlModel { get; set; }
    }
}