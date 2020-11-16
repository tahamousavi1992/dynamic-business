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
    public class ServiceTaskDTO
    {
        public ServiceTaskDTO() { }
        public ServiceTaskDTO(sysBpmsTask task)
        {
            this.ID = task.ID;
            this.ProcessID = task.ProcessID;
            this.ElementID = task.ElementID;
            this.TypeLU = task.TypeLU;
            this.Code = task.Code;
        }
        [DataMember]
        public Guid ID { get; set; }
        [DataMember]
        public Guid ProcessID { get; set; }
        [DataMember]
        public string ElementID { get; set; }
        [DataMember]
        public int TypeLU { get; set; }
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public DesignCodeModel DesignCodeModel
        {
            get
            {
                return DesignCodeUtility.GetDesignCodeFromXml(this.Code);
            }
            private set { }
        }
    }
}