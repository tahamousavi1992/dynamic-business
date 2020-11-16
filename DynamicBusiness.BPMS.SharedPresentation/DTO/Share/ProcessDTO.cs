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
    public class ProcessDTO
    {
        public ProcessDTO() { }

        public ProcessDTO(sysBpmsProcess process)
        {
            if (process != null)
            {
                this.ID = process.ID;
                this.Name = process.Name;
                this.Number = process.Number;
                this.FormattedNumber = process.FormattedNumber;
                this.Description = process.Description;
                this.StatusLU = process.StatusLU;
                this.CreatorUsername = process.CreatorUsername;
                this.CreateDate = process.CreateDate;
                this.UpdateDate = process.UpdateDate;
                this.ParallelCountPerUser = process.ParallelCountPerUser;
                this.ProcessGroupID = process.ProcessGroupID;
                this.TypeLU = process.TypeLU;
                this.ProcessVersion = process.ProcessVersion;
                this.DiagramXML = process.DiagramXML;
            }
        }
        [DataMember]
        public Guid ID { get; set; } 
        [Required]
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string DiagramXML { get; set; } 
        [DataMember]
        public Nullable<int> ProcessVersion { get; set; } 
        [DataMember]
        public string Description { get; set; } 
        [DataMember]
        public int? Number { get; set; } 
        [DataMember]
        public string FormattedNumber { get; set; } 
        [DataMember]
        public int? StatusLU { get; set; }
        [DataMember]
        public string StatusName { get { return this.StatusLU.HasValue ? ((sysBpmsProcess.Enum_StatusLU)this.StatusLU).GetDescription() : ""; } set { } }
        [DataMember]
        public string CreatorUsername { get; set; } 
        [DataMember]
        public Nullable<System.DateTime> CreateDate { get; set; } 
        [DataMember]
        public Nullable<System.DateTime> UpdateDate { get; set; } 
        [DataMember]
        public Nullable<int> ParallelCountPerUser { get; set; }
        [DataMember]
        public System.Guid ProcessGroupID { get; set; } 
        [DataMember]
        public int TypeLU { get; set; }
        [DataMember]
        public string TypeName { get { return ((sysBpmsProcess.e_TypeLU)this.TypeLU).GetDescription(); } set { } }
        [DataMember]
        public bool AllowEdit
        {
            get { return this.StatusLU == (int)sysBpmsProcess.Enum_StatusLU.Draft || this.StatusLU == (int)sysBpmsProcess.Enum_StatusLU.Inactive; }
            set { }
        }
        [DataMember]
        public List<QueryModel> ListTypes { get; set; }
    }
}