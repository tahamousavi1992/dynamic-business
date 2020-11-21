using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class MessageCodeHelper : IMessageCodeHelper
    {
        public CodeBaseSharedModel CodeBaseShared { get; set; }
        public MessageCodeHelper(CodeBaseSharedModel codeBaseShared)
        {
            this.CodeBaseShared = codeBaseShared;
        }

        /// <summary>
        /// it is used to show an error message at the end to the client 
        /// </summary>
        public void AddError(string message)
        {
            this.CodeBaseShared.MessageList.Add(new MessageModel(DisplayMessageType.error, message));
        }

        /// <summary>
        /// it is used to show an info message at the end to the client 
        /// </summary>
        public void AddInfo(string message)
        {
            this.CodeBaseShared.MessageList.Add(new MessageModel(DisplayMessageType.info, message));
        }

        /// <summary>
        /// it is used to show a success message at the end to the client 
        /// </summary>
        public void AddSuccess(string message)
        {
            this.CodeBaseShared.MessageList.Add(new MessageModel(DisplayMessageType.success, message));
        }

        /// <summary>
        /// it is used to show a warning message at the end to the client 
        /// </summary>
        public void AddWarning(string message)
        {
            this.CodeBaseShared.MessageList.Add(new MessageModel(DisplayMessageType.warning, message));
        }

    }
}
