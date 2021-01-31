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
    public class VariableDTO
    {
        public VariableDTO()
        {
            this.ListVariableDependencyDTO = new List<VariableDependencyDTO>();
        }
        public VariableDTO(sysBpmsVariable variable)
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
            this.ListVariableDependencyDTO = variable.VariableDependencies?.Select(c => new VariableDependencyDTO(c)).ToList();
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
        public List<VariableDependencyDTO> ListVariableDependencyDTO { get; set; }

        #region ..:: for PostAddEdit Variable Manager Controller ::..
        public List<VariableDependencyDTO> ListItems { get; set; }
        public List<VariableDependencyDTO> ListDependencies { get; set; }
        #endregion
 
    }
}