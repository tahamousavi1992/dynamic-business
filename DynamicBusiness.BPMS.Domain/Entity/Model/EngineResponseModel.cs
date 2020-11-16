using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public class EngineResponseModel
    {
        public EngineResponseModel()
        {
        }
       
        public EngineResponseModel InitPost(ResultOperation resultOperation, List<MessageModel> listMessageModel,
          RedirectUrlModel redirectUrlModel,
          bool isSubmit = true, List<DownloadModel> listDownloadModel = null)
        {
            this.ResultOperation = resultOperation;
            this.ListMessageModel = listMessageModel;
            this.FormModel = null;
            this.RedirectUrlModel = redirectUrlModel;
            this.IsSubmit = isSubmit;
            this.ListDownloadModel = listDownloadModel;
            return this;
        }
        
        public EngineResponseModel InitGet(ResultOperation resultOperation, List<MessageModel> listMessageModel,
           RedirectUrlModel redirectUrlModel, FormModel formModel = null)
        {
            this.ResultOperation = resultOperation;
            this.ListMessageModel = listMessageModel;
            this.FormModel = formModel;
            this.RedirectUrlModel = redirectUrlModel;
            this.IsSubmit = true;
            return this;
        }
       
        public bool IsSubmit { get; set; }

        public ResultOperation ResultOperation { get; set; }
        /// <summary>
        /// dynamic code generate this list
        /// </summary>
        public List<MessageModel> ListMessageModel { get; set; }
        /// <summary>
        /// it is set in get content form action
        /// </summary>
        public FormModel FormModel { get; set; }

        public RedirectUrlModel RedirectUrlModel { get; set; }

        public List<DownloadModel> ListDownloadModel { get; set; }
    }
}
