
namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;
    public partial class sysBpmsVariable
    {

        public sysBpmsVariable()
        {
            this.ThreadVariables = new HashSet<sysBpmsThreadVariable>();
            this.DependentVariableDependencies = new HashSet<sysBpmsVariableDependency>();
            this.ToVariableDependencies = new HashSet<sysBpmsVariableDependency>();
        }
        
        public System.Guid ID { get; set; }
        [ForeignKey(nameof(Process))]
        public Nullable<System.Guid> ProcessID { get; set; }
        [ForeignKey(nameof(ApplicationPage))]
        public Nullable<System.Guid> ApplicationPageID { get; set; }
        [ForeignKey(nameof(EntityDef))]
        public Nullable<System.Guid> EntityDefID { get; set; }
        [ForeignKey(nameof(DBConnection))]
        public Nullable<System.Guid> DBConnectionID { get; set; }
        [Required(AllowEmptyStrings = true)]
        [MaxLength(500)]
        public string Name { get; set; }
        public int VarTypeLU { get; set; }
        [MaxLength(250)]
        public string FieldName { get; set; }
        public string Query { get; set; }
        public Nullable<int> FilterTypeLU { get; set; }
        public string Collection { get; set; }
        public string DefaultValue { get; set; }
        public string WhereClause { get; set; }
        public string OrderByClause { get; set; }

        public virtual sysBpmsApplicationPage ApplicationPage { get; set; }
        public virtual sysBpmsDBConnection DBConnection { get; set; }
        public virtual sysBpmsEntityDef EntityDef { get; set; }
        public virtual sysBpmsProcess Process { get; set; }

        public virtual ICollection<sysBpmsThreadVariable> ThreadVariables { get; set; }

        public virtual ICollection<sysBpmsVariableDependency> DependentVariableDependencies { get; set; }
        public virtual ICollection<sysBpmsVariableDependency> ToVariableDependencies { get; set; }
    }
}
