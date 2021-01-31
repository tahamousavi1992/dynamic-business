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
    public class SelectVariableDTO
    {
        public SelectVariableDTO()
        {
            this.listVariableDependencyDTO = new List<VariableDependencyDTO>();
        }
        public SelectVariableDTO(sysBpmsVariable variable)
        {
            this.ID = variable.ID;
            this.ProcessID = variable.ProcessID;
            this.ApplicationPageID = variable.ApplicationPageID;
            this.Name = variable.Name;
            this.VarTypeLU = variable.VarTypeLU;
            this.EntityDefID = variable.EntityDefID;
            this.FieldName = variable.FieldName;
            this.Query = variable.Query;
            this.FilterTypeLU = variable.FilterTypeLU;
            this.Collection = variable.Collection;
            this.DBConnectionID = variable.DBConnectionID;
            this.DefaultValue = variable.DefaultValue;
            this.WhereClause = variable.WhereClause;
            this.OrderByClause = variable.OrderByClause;
            this.listVariableDependencyDTO = variable.VariableDependencies?.Select(c => new VariableDependencyDTO(c)).ToList();

            if (this.EntityDefID.HasValue && this.VarTypeLU == (int)sysBpmsVariable.e_VarTypeLU.Object)
                using (EntityDefService entityDefService = new EntityDefService())
                    this.ListEntityPropertyModel = entityDefService.GetInfo(this.EntityDefID.Value).AllProperties.Where(c => c.IsActive).ToList();
        }
        [DataMember]
        public Guid ID { get; set; }
        [DataMember]
        public Guid? ProcessID { get; set; }
        [DataMember]
        public Nullable<System.Guid> ApplicationPageID { get; set; }
        [DataMember]
        public string Collection { get; set; }
        [Required] 
        [DataMember]
        public string Name { get; set; }

        [Required] 
        [DataMember]
        public int VarTypeLU { get; set; }

        [DataMember]
        public string VarTypeName { get { return ((sysBpmsVariable.e_VarTypeLU)this.VarTypeLU).GetDescription(); } set { } }

        [Required] 
        [DataMember]
        public int? FilterTypeLU { get; set; }

        [DataMember]
        public string FilterTypeName { get { return ((sysBpmsVariable.e_FilterTypeLU?)this.FilterTypeLU).GetDescription(); } set { } }
         
        [DataMember]
        public Guid? EntityDefID { get; set; }
         
        [DataMember]
        public string FieldName { get; set; }
         
        [DataMember]
        public string Query { get; set; }
         
        [DataMember]
        public Guid? DBConnectionID { get; set; }
         
        [DataMember]
        public string DefaultValue { get; set; } 
        [DataMember]
        public string WhereClause { get; set; } 
        [DataMember]
        public string OrderByClause { get; set; }
        [DataMember]
        public IEnumerable<VariableItemModel> Items
        {
            get
            {
                return this.Collection.ParseXML<List<VariableItemModel>>();
            }
            private set { }
        }
        [DataMember]
        public int RelationTypeLU
        {
            get
            {
                if (this.EntityDefID.HasValue)
                    return (int)sysBpmsVariable.e_RelationTypeLU.Entity;
                if (!string.IsNullOrWhiteSpace(this.Query))
                    return (int)sysBpmsVariable.e_RelationTypeLU.SqlQuery;
                return (int)sysBpmsVariable.e_RelationTypeLU.Local;
            }
            private set { }
        }
        [DataMember]
        public List<VariableDependencyDTO> listVariableDependencyDTO { get; set; }
        [DataMember]
        public List<EntityPropertyModel> ListEntityPropertyModel { get; set; }
    }
}