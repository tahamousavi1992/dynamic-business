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
    public class ThreadHistoryDTO
    {
        public ThreadHistoryDTO()
        { }
        public ThreadHistoryDTO(sysBpmsThreadTask threadTask)
        {
            this.ThreadTaskID = threadTask.ID;
            this.ThreadTaskStartDate = threadTask.StartDate;
            this.ThreadTaskEndDate = threadTask.EndDate;
            this.Description = threadTask.Description;
            this.OwnerUserID = threadTask.OwnerUserID;
            this.ThreadTaskStatusLU = threadTask.StatusLU;
            this.TaskName = threadTask.sysBpmsTask?.sysBpmsElement?.Name ?? "";
            this.OwnerUser = new UserDTO(threadTask.sysBpmsUser);
        }
        [DataMember]
        public Guid ThreadTaskID { get; set; }
        [DataMember]
        public System.DateTime ThreadTaskStartDate { get; set; }
        [DataMember]
        public System.DateTime? ThreadTaskEndDate { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public Guid? OwnerUserID { get; set; }
        [DataMember]
        public int ThreadTaskStatusLU { get; set; }
        [DataMember]
        public string TaskName { get; set; }
        [DataMember]
        public UserDTO OwnerUser { get; set; }
    }
}