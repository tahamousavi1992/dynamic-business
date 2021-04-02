using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public class CodeBaseSharedModel
    {
        public CodeBaseSharedModel(
            List<Domain.MessageModel> messageList = null,
          List<Domain.VariableModel> listSetVariable = null,
          List<Domain.DownloadModel> listDownloadModel = null)
        {
            this.MessageList = messageList ?? new List<Domain.MessageModel>();
            this.ListSetVariable = listSetVariable ?? new List<Domain.VariableModel>();
            this.ListDownloadModel = listDownloadModel ?? new List<Domain.DownloadModel>();
        }
        public List<Domain.MessageModel> MessageList { get; set; }
        public List<Domain.VariableModel> ListSetVariable { get; set; }
        public List<Domain.DownloadModel> ListDownloadModel { get; set; }
        /// <summary>
        /// Is filled by form input element.
        /// </summary>
        public string ThreadTaskDescription { get; set; }
    }
}
