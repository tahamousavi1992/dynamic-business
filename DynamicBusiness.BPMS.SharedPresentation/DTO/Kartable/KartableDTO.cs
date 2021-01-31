using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DynamicBusiness.BPMS.SharedPresentation
{
    [DataContract]
    public class KartableDTO
    {
        public KartableDTO(sysBpmsThreadTask threadTask)
        {
            this.ThreadTaskID = threadTask.ID;
            this.ThreadTaskStartDate = threadTask.StartDate;
            this.ThreadTaskEndDate = threadTask.EndDate;
            this.ThreadTaskDescription = threadTask.Description;
            this.OwnerUserID = threadTask.OwnerUserID;
            this.ThreadTaskStatusLU = threadTask.StatusLU;
            this.TaskName = threadTask.Task?.Element?.Name ?? "";
            this.Thread = new ThreadDTO(threadTask.Thread);
        }
        [DataMember]
        public Guid ThreadTaskID { get; set; }

        [DataMember]
        public System.DateTime ThreadTaskStartDate { get; set; }
        [DataMember]
        public System.DateTime? ThreadTaskEndDate { get; set; }
        [DataMember]
        public string ThreadTaskDescription { get; set; }
        [DataMember]
        public Guid? OwnerUserID { get; set; }
        [DataMember]
        public int ThreadTaskStatusLU { get; set; }
        [DataMember]
        public ThreadDTO Thread { get; set; }
        [DataMember]
        public string TaskName { get; set; }
    }
}