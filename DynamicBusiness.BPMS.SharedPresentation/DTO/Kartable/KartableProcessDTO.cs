using DynamicBusiness.BPMS.Domain;
using DynamicBusiness.BPMS.BusinessLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace DynamicBusiness.BPMS.SharedPresentation
{
    [DataContract]
    [KnownType(typeof(KartableProcessDTO))]
    public class KartableProcessDTO
    {
        public KartableProcessDTO(sysBpmsProcess process)
        {
            this.ID = process.ID; 
            this.FormattedNumber = process.FormattedNumber;
            this.Name = process.Name;
            this.Description = process.Description;
            this.ProcessVersion = process.ProcessVersion;
            this.StatusLUName = ((sysBpmsProcess.Enum_StatusLU)process.StatusLU).GetDescription();
            this.CreatorUsername = process.CreatorUsername;
            this.ParentProcessID = process.ParentProcessID;
            this.CreateDate = process.CreateDate;
            this.UpdateDate = process.UpdateDate;
        }
        [DataMember]
        public Guid ID { get; set; }
        [DataMember]
        public string FormattedNumber { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public Nullable<int> ProcessVersion { get; set; }
        [DataMember]
        public string StatusLUName { get; set; }
        [DataMember]
        public string CreatorUsername { get; set; }
        [DataMember]
        public Nullable<Guid> ParentProcessID { get; set; }
        [DataMember]
        public Nullable<System.DateTime> CreateDate { get; set; }
        [DataMember]
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }

}