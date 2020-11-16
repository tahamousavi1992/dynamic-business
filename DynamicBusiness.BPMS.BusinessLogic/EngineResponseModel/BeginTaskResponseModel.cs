using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    [DataContract]
    public class BeginTaskResponseModel
    {
        public BeginTaskResponseModel() { }
        public BeginTaskResponseModel(string message, bool result, Guid? threadID, Guid? threadTaskID)
        {
            this.Message = message;
            this.Result = result;
            this.ThreadID = threadID;
            this.ThreadTaskID = threadTaskID;
        }

        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public bool Result { get; set; }
        [DataMember]
        public Guid? ThreadID { get; set; }
        [DataMember]
        public Guid? ThreadTaskID { get; set; }
    }
}