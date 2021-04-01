using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    [DataContract]
    public class PostFormResponseModel
    {
        public PostFormResponseModel() { }
        public PostFormResponseModel(List<MessageModel> listMessageModel, string message, bool isSuccess,
            bool isSubmit, RedirectUrlModel redirectUrlModel, List<DownloadModel> listDownloadModel = null)
        {
            this.ListMessageModel = listMessageModel;
            this.IsSuccess = isSuccess;
            this.IsSubmit = isSubmit;
            this.RedirectUrlModel = redirectUrlModel;
            this.ListDownloadModel = listDownloadModel;
            if (!string.IsNullOrWhiteSpace(message) &&
                //If it is a success message and user added at least one success message,this skips default success message.
                (!isSuccess || !this.ListMessageModel.Any(c => c.DisplayMessageType == DisplayMessageType.success)))
            {
                this.ListMessageModel = (this.ListMessageModel ?? new List<MessageModel>()).Union(new List<MessageModel>() {
                      new MessageModel(this.IsSuccess ? DisplayMessageType.success : DisplayMessageType.error, message)
                  }).ToList();
            }
        }
        [DataMember]
        public bool IsSubmit { get; set; }
        [DataMember]
        public bool IsSuccess { get; set; }
        [DataMember]
        public List<MessageModel> ListMessageModel { get; set; }
        public void AddMessage(DisplayMessageType displayMessageType, string message)
        {
            this.ListMessageModel = this.ListMessageModel ?? new List<MessageModel>();
            this.ListMessageModel.Add(new MessageModel(displayMessageType, message));
        }

        [DataMember]
        public RedirectUrlModel RedirectUrlModel { get; set; }
        [DataMember]
        public List<DownloadModel> ListDownloadModel { get; set; }
    }
}