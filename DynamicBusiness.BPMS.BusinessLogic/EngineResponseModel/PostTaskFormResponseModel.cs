using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    [DataContract]
    public class PostTaskFormResponseModel
    {
        public PostTaskFormResponseModel() { }
        public PostTaskFormResponseModel(RedirectUrlModel redirectUrlModel, string message, bool isSuccess,
            bool isSubmit, Guid? stepID = null, bool? isNextPrevious = null,
            List<MessageModel> listMessageModel = null, List<DownloadModel> listDownloadModel = null)
        {
            this.ListMessageModel = listMessageModel;
            this.IsSuccess = isSuccess;
            this.IsSubmit = isSubmit;
            this.StepID = stepID;
            this.IsNextPrevious = isNextPrevious;
            this.RedirectUrlModel = redirectUrlModel;
            this.ListDownloadModel = listDownloadModel;
            if (!string.IsNullOrWhiteSpace(message))
            {
                this.ListMessageModel = (this.ListMessageModel ?? new List<MessageModel>()).Union(new List<MessageModel>() {
                      new MessageModel(this.IsSuccess ? DisplayMessageType.success : DisplayMessageType.error, message)
                  }).ToList();
            }
        }
        [DataMember]
        public List<MessageModel> ListMessageModel { get; set; }
        [DataMember]
        public bool IsSuccess { get; set; }
        [DataMember]
        public bool IsSubmit { get; set; }
        [DataMember]
        public Guid? StepID { get; set; }
        [DataMember]
        public bool? IsNextPrevious { get; set; }
        [DataMember]
        public RedirectUrlModel RedirectUrlModel { get; set; }
        [DataMember]
        public List<DownloadModel> ListDownloadModel { get; set; }
    }
}