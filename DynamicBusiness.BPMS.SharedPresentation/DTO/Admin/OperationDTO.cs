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
    public class OperationDTO
    {
        public OperationDTO() { }

        public OperationDTO(sysBpmsOperation operation)
        {
            this.ID = operation.ID;
            this.GroupLU = operation.GroupLU;
            this.TypeLU = operation.TypeLU;
            this.Name = operation.Name;
            this.SqlCommand = operation.SqlCommand;
        }
        [DataMember]
        public Guid ID { get; set; }
         
        [DataMember]
        public int GroupLU { get; set; }
         
        [DataMember]
        public string Name { get; set; }
         
        [DataMember]
        public string GroupName { get { return new LURowService().GetNameOfByAlias(sysBpmsLUTable.e_LUTable.OperationGroupLU.ToString(), this.GroupLU.ToStringObj()); } private set { } }
         
        [DataMember]
        public string SqlCommand { get; set; }
         
        [DataMember]
        public int TypeLU { get; set; }
         
        [DataMember]
        public string TypeLUName { get { return ((sysBpmsOperation.e_TypeLU)this.TypeLU).GetDescription(); } private set { } }
        [DataMember]
        public List<LURowDTO> Groups { get; set; }
        [DataMember]
        public List<QueryModel> Types { get { return EnumObjHelper.GetEnumList<sysBpmsOperation.e_TypeLU>().Select(c => new QueryModel(c.Key.ToString(), c.Value)).ToList(); } private set { } }
    }
}