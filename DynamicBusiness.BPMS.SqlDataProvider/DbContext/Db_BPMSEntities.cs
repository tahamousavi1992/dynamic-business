
namespace DynamicBusiness.BPMS.BusinessLogic
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    using DynamicBusiness.BPMS.Domain;
    using System.Data.Entity.ModelConfiguration.Conventions;
    using CodeFirstStoreFunctions;

    public partial class Db_BPMSEntities : DbContext
    {
        public Db_BPMSEntities()
          : base(DomainUtility.GetConnectionName())
        {
            Database.SetInitializer(new NullDatabaseInitializer<Db_BPMSEntities>());
        }

        //public Db_BPMSEntities()
        //    : base("Db_BPMSEntities")
        //{
        //    Database.SetInitializer(new NullDatabaseInitializer<Db_BPMSEntities>());
        //}

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // It must be commented when running add-migration command.
            modelBuilder.Conventions.Add(new FunctionsConvention<Db_BPMSEntities>("dbo"));

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            //for one to one: HasRequired -> WithOptional
            //Configure primary key

            //Process Models
            modelBuilder.Entity<sysBpmsGateway>().HasRequired(c => c.Element).WithMany(c => c.Gateways).
                HasForeignKey(c => new { c.ElementID, c.ProcessID });

            modelBuilder.Entity<sysBpmsEvent>().HasRequired(c => c.Element).WithMany(c => c.Events).
                HasForeignKey(c => new { c.ElementID, c.ProcessID });

            modelBuilder.Entity<sysBpmsTask>().HasRequired(c => c.Element).WithMany(c => c.Tasks).
                HasForeignKey(c => new { c.ElementID, c.ProcessID });

            modelBuilder.Entity<sysBpmsSequenceFlow>().HasRequired(c => c.Element).WithMany(c => c.SequenceFlows).
                HasForeignKey(c => new { c.ElementID, c.ProcessID });

            modelBuilder.Entity<sysBpmsLane>().HasRequired(c => c.Element).WithMany(c => c.Lanes).
                HasForeignKey(c => new { c.ElementID, c.ProcessID });

            //sysBpmsVariableDependency
            modelBuilder.Entity<sysBpmsVariableDependency>().HasRequired(c => c.DependentVariable).WithMany(c => c.DependentVariableDependencies).
                HasForeignKey(c => c.DependentVariableID);

            modelBuilder.Entity<sysBpmsVariableDependency>().HasOptional(c => c.ToVariable).WithMany(c => c.ToVariableDependencies).
                HasForeignKey(c => c.ToVariableID);
        }

        public virtual DbSet<sysBpmsAPIAccess> sysBpmsAPIAccesses { get; set; }
        public virtual DbSet<sysBpmsApplicationPage> sysBpmsApplicationPages { get; set; }
        public virtual DbSet<sysBpmsApplicationPageAccess> sysBpmsApplicationPageAccesses { get; set; }
        public virtual DbSet<sysBpmsCondition> sysBpmsConditions { get; set; }
        public virtual DbSet<sysBpmsConfiguration> sysBpmsConfigurations { get; set; }
        public virtual DbSet<sysBpmsDBConnection> sysBpmsDBConnections { get; set; }
        public virtual DbSet<sysBpmsDepartment> sysBpmsDepartments { get; set; }
        public virtual DbSet<sysBpmsDepartmentMember> sysBpmsDepartmentMembers { get; set; }
        public virtual DbSet<sysBpmsDocument> sysBpmsDocuments { get; set; }
        public virtual DbSet<sysBpmsDocumentDef> sysBpmsDocumentDefs { get; set; }
        public virtual DbSet<sysBpmsDocumentFolder> sysBpmsDocumentFolders { get; set; }
        public virtual DbSet<sysBpmsDynamicForm> sysBpmsDynamicForms { get; set; }
        public virtual DbSet<sysBpmsElement> sysBpmsElements { get; set; }
        public virtual DbSet<sysBpmsEmailAccount> sysBpmsEmailAccounts { get; set; }
        public virtual DbSet<sysBpmsEntityDef> sysBpmsEntityDefs { get; set; }
        public virtual DbSet<sysBpmsEvent> sysBpmsEvents { get; set; }
        public virtual DbSet<sysBpmsGateway> sysBpmsGateways { get; set; }
        public virtual DbSet<sysBpmsLane> sysBpmsLanes { get; set; }
        public virtual DbSet<sysBpmsLURow> sysBpmsLURows { get; set; }
        public virtual DbSet<sysBpmsLUTable> sysBpmsLUTables { get; set; }
        public virtual DbSet<sysBpmsMessageType> sysBpmsMessageTypes { get; set; }
        public virtual DbSet<sysBpmsProcess> sysBpmsProcesses { get; set; }
        public virtual DbSet<sysBpmsProcessGroup> sysBpmsProcessGroups { get; set; }
        public virtual DbSet<sysBpmsSequenceFlow> sysBpmsSequenceFlows { get; set; }
        public virtual DbSet<sysBpmsStep> sysBpmsSteps { get; set; }
        public virtual DbSet<sysBpmsTask> sysBpmsTasks { get; set; }
        public virtual DbSet<sysBpmsThread> sysBpmsThreads { get; set; }
        public virtual DbSet<sysBpmsThreadEvent> sysBpmsThreadEvents { get; set; }
        public virtual DbSet<sysBpmsThreadTask> sysBpmsThreadTasks { get; set; }
        public virtual DbSet<sysBpmsThreadVariable> sysBpmsThreadVariables { get; set; }
        public virtual DbSet<sysBpmsUser> sysBpmsUsers { get; set; }
        public virtual DbSet<sysBpmsVariable> sysBpmsVariables { get; set; }
        public virtual DbSet<sysBpmsVariableDependency> sysBpmsVariableDependencies { get; set; }
        /// <summary>
        /// It must be removed or commented when running add-migration command
        /// </summary>
        public virtual DbSet<sysBpmsSplit_Result> sysBpmsSplit_Results { get; set; }

        /// <summary>
        /// DbFunction attribute must be removed or commented when running add-migration command
        /// </summary>
        [DbFunction(nameof(Db_BPMSEntities), "sysBpmsSplit")]
        public virtual IQueryable<sysBpmsSplit_Result> sysBpmsSplit(string rowData, string delimeter)
        {
            var rowDataParameter = rowData != null ?
                new ObjectParameter("RowData", rowData) :
                new ObjectParameter("RowData", typeof(string));

            var delimeterParameter = delimeter != null ?
                new ObjectParameter("Delimeter", delimeter) :
                new ObjectParameter("Delimeter", typeof(string));

            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<sysBpmsSplit_Result>("[Db_BPMSEntities].[sysBpmsSplit](@RowData, @Delimeter)", rowDataParameter, delimeterParameter);
        }
    }
}
