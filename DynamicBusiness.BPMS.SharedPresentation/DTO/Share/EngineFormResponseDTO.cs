using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DynamicBusiness.BPMS.SharedPresentation
{
    /// <summary>
    /// this is used for returning response to post request from forms and UserTasks.
    /// </summary>
    [DataContract]
    public class EngineFormResponseDTO
    {
        public EngineFormResponseDTO()
        {

        }
        public EngineFormResponseDTO(string redirectUrl = null, bool reloadForm = false,
           List<DownloadModel> listDownloadModel = null, bool isSubmit = false, List<MessageModel> messageList = null, bool result = true, string submittedHtmlMessage = "")
        {
            this.RedirectUrl = redirectUrl;
            this.ReloadForm = reloadForm;
            this.IsSubmit = isSubmit;
            this.ListDownloadModel = listDownloadModel;
            this.MessageList = messageList?.Select(c => new PostMethodMessage(c.Message, c.DisplayMessageType)).ToList();
            this.Result = result;
            this.SubmittedHtmlMessage = submittedHtmlMessage;
        }
        /// <summary>
        /// It is used for singleAction to show a predefined message if user submit a form.
        /// </summary>
        [DataMember]
        public string SubmittedHtmlMessage { get; set; }
        [DataMember]
        public string RedirectUrl { get; set; }
        [DataMember]
        public bool IsSubmit { get; set; }
        [DataMember]
        public bool Result { get; set; }
        [DataMember]
        public bool ReloadForm { get; set; }
        [DataMember]
        public Guid? StepID { get; set; }
        [DataMember]
        public Guid? EndAppPageID { get; set; }
        [DataMember]
        public List<DownloadModel> ListDownloadModel { get; set; }
        [DataMember]
        public List<PostMethodMessage> MessageList { get; set; }
    }
}