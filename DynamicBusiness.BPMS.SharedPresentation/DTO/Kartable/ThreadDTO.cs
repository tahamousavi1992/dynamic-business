using DynamicBusiness.BPMS.BusinessLogic;
using DynamicBusiness.BPMS.Domain;
using DynamicBusiness.BPMS.SharedPresentation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DynamicBusiness.BPMS.SharedPresentation
{
    [DataContract]
    public class ThreadDTO
    {
        public ThreadDTO()
        { }
        public ThreadDTO(sysBpmsThread thread)
        {
            if (thread != null)
            {
                this.ID = thread.ID;
                this.ProcessID = thread.ProcessID;
                this.UserID = thread.UserID;
                this.StartDate = thread.StartDate;
                this.EndDate = thread.EndDate;
                this.Number = thread.Number;
                this.FormattedNumber = thread.FormattedNumber;
                this.StatusLU = thread.StatusLU;
                this.User = thread.UserID.HasValue ? (thread.sysBpmsUser != null ? new UserDTO(thread.sysBpmsUser) : new UserDTO(new UserService().GetInfo(this.UserID.Value))) : new UserDTO();
                this.Process = new ProcessDTO(thread.sysBpmsProcess);
            }
        }
        [DataMember]
        public Guid ID { get; set; }
        [DataMember]
        public Guid ProcessID { get; set; }
        [DataMember]
        public Guid? UserID { get; set; }
        [DataMember]
        public System.DateTime StartDate { get; set; }
        [DataMember]
        public Nullable<System.DateTime> EndDate { get; set; }
        [DataMember]
        public int? Number { get; set; }
        [DataMember]
        public string FormattedNumber { get; set; }
        [DataMember]
        public int StatusLU { get; set; }
        [DataMember]
        public string StatusName { get { return ((sysBpmsThread.Enum_StatusLU)this.StatusLU).GetDescription(); } set { } }
        [DataMember]
        public UserDTO User { get; set; }
        [DataMember]
        public ProcessDTO Process { get; set; }
    }
}