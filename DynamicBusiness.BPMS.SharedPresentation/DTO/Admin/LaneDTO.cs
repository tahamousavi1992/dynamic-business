using DynamicBusiness.BPMS.BusinessLogic;
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
    public class LaneDTO
    {
        public LaneDTO() { }
        public LaneDTO(sysBpmsLane lane)
        {
            if (lane != null)
            {
                this.ID = lane.ID;
                this.ProcessID = lane.ProcessID;
                this.ElementID = lane.ElementID;
            }
        }
        [DataMember]
        public Guid ID { get; set; }
        [DataMember]
        public Guid ProcessID { get; set; }
        [DataMember]
        public string ElementID { get; set; }
    }
}