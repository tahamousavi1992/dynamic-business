namespace DynamicBusiness.BPMS.BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class version_1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.sysBpmsAPIAccesses",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        IPAddress = c.String(),
                        AccessKey = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.sysBpmsApplicationPageAccesses",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        ApplicationPageID = c.Guid(nullable: false),
                        DepartmentID = c.Guid(),
                        RoleLU = c.Int(),
                        UserID = c.Guid(),
                        AllowAdd = c.Boolean(nullable: false),
                        AllowEdit = c.Boolean(nullable: false),
                        AllowDelete = c.Boolean(nullable: false),
                        AllowView = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.sysBpmsApplicationPages", t => t.ApplicationPageID)
                .ForeignKey("dbo.sysBpmsDepartments", t => t.DepartmentID)
                .ForeignKey("dbo.sysBpmsUsers", t => t.UserID)
                .Index(t => t.ApplicationPageID)
                .Index(t => t.DepartmentID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.sysBpmsApplicationPages",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        GroupLU = c.Int(nullable: false),
                        Description = c.String(),
                        ShowInMenu = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.sysBpmsDynamicForms",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        ProcessId = c.Guid(),
                        ApplicationPageID = c.Guid(),
                        Name = c.String(),
                        DesignJson = c.String(),
                        OnExitFormCode = c.String(),
                        OnEntryFormCode = c.String(),
                        Version = c.Int(),
                        ConfigXML = c.String(),
                        ShowInOverview = c.Boolean(),
                        SourceCode = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.sysBpmsApplicationPages", t => t.ApplicationPageID)
                .ForeignKey("dbo.sysBpmsProcesses", t => t.ProcessId)
                .Index(t => t.ProcessId)
                .Index(t => t.ApplicationPageID);
            
            CreateTable(
                "dbo.sysBpmsProcesses",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        FormattedNumber = c.String(),
                        Number = c.Int(),
                        Name = c.String(),
                        Description = c.String(),
                        ProcessVersion = c.Int(),
                        StatusLU = c.Int(),
                        CreatorUsername = c.String(),
                        CreateDate = c.DateTime(),
                        UpdateDate = c.DateTime(),
                        DiagramXML = c.String(),
                        WorkflowXML = c.String(),
                        BeginTasks = c.String(),
                        ParentProcessID = c.Guid(),
                        PublishDate = c.DateTime(),
                        ParallelCountPerUser = c.Int(),
                        SourceCode = c.String(),
                        ProcessGroupID = c.Guid(nullable: false),
                        TypeLU = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.sysBpmsProcesses", t => t.ParentProcessID)
                .ForeignKey("dbo.sysBpmsProcessGroups", t => t.ProcessGroupID)
                .Index(t => t.ParentProcessID)
                .Index(t => t.ProcessGroupID);
            
            CreateTable(
                "dbo.sysBpmsElements",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        TypeLU = c.Int(nullable: false),
                        ProcessID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.sysBpmsProcesses", t => t.ProcessID)
                .Index(t => t.ProcessID);
            
            CreateTable(
                "dbo.sysBpmsEvents",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        ElementID = c.String(maxLength: 128),
                        TypeLU = c.Int(nullable: false),
                        ConfigurationXML = c.String(),
                        SubType = c.Int(),
                        RefElementID = c.String(),
                        CancelActivity = c.Boolean(),
                        ProcessID = c.Guid(nullable: false),
                        MessageTypeID = c.Guid(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.sysBpmsElements", t => t.ElementID)
                .ForeignKey("dbo.sysBpmsMessageTypes", t => t.MessageTypeID)
                .ForeignKey("dbo.sysBpmsProcesses", t => t.ProcessID)
                .Index(t => t.ElementID)
                .Index(t => t.ProcessID)
                .Index(t => t.MessageTypeID);
            
            CreateTable(
                "dbo.sysBpmsMessageTypes",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        ParamsXML = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.sysBpmsThreadEvents",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        ThreadID = c.Guid(nullable: false),
                        EventID = c.Guid(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        ExecuteDate = c.DateTime(nullable: false),
                        StatusLU = c.Int(nullable: false),
                        ThreadTaskID = c.Guid(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.sysBpmsEvents", t => t.EventID)
                .ForeignKey("dbo.sysBpmsThreads", t => t.ThreadID)
                .ForeignKey("dbo.sysBpmsThreadTasks", t => t.ThreadTaskID)
                .Index(t => t.ThreadID)
                .Index(t => t.EventID)
                .Index(t => t.ThreadTaskID);
            
            CreateTable(
                "dbo.sysBpmsThreads",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(),
                        StatusLU = c.Int(nullable: false),
                        GatewayStatusXml = c.String(),
                        ProcessID = c.Guid(nullable: false),
                        UserID = c.Guid(),
                        FormattedNumber = c.String(),
                        Number = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.sysBpmsProcesses", t => t.ProcessID)
                .ForeignKey("dbo.sysBpmsUsers", t => t.UserID)
                .Index(t => t.ProcessID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.sysBpmsDocuments",
                c => new
                    {
                        GUID = c.Guid(nullable: false, identity: true),
                        IsDeleted = c.Boolean(nullable: false),
                        DocumentDefID = c.Guid(nullable: false),
                        EntityDefID = c.Guid(),
                        EntityID = c.Guid(),
                        ThreadID = c.Guid(),
                        AtachDateOf = c.DateTime(nullable: false),
                        FileExtention = c.String(),
                        CaptionOf = c.String(),
                    })
                .PrimaryKey(t => t.GUID)
                .ForeignKey("dbo.sysBpmsDocumentDefs", t => t.DocumentDefID)
                .ForeignKey("dbo.sysBpmsEntityDefs", t => t.EntityDefID)
                .ForeignKey("dbo.sysBpmsThreads", t => t.ThreadID)
                .Index(t => t.DocumentDefID)
                .Index(t => t.EntityDefID)
                .Index(t => t.ThreadID);
            
            CreateTable(
                "dbo.sysBpmsDocumentDefs",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        DocumentFolderID = c.Guid(nullable: false),
                        NameOf = c.String(),
                        DisplayName = c.String(),
                        MaxSize = c.Int(),
                        ValidExtentions = c.String(),
                        IsMandatory = c.Boolean(nullable: false),
                        Description = c.String(),
                        IsSystemic = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.sysBpmsDocumentFolders", t => t.DocumentFolderID)
                .Index(t => t.DocumentFolderID);
            
            CreateTable(
                "dbo.sysBpmsDocumentFolders",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        NameOf = c.String(),
                        DisplayName = c.String(),
                        DocumentFolderID = c.Guid(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.sysBpmsDocumentFolders", t => t.DocumentFolderID)
                .Index(t => t.DocumentFolderID);
            
            CreateTable(
                "dbo.sysBpmsEntityDefs",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        DisplayName = c.String(),
                        Name = c.String(),
                        DesignXML = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.sysBpmsVariables",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        ProcessID = c.Guid(),
                        ApplicationPageID = c.Guid(),
                        EntityDefID = c.Guid(),
                        DBConnectionID = c.Guid(),
                        Name = c.String(),
                        VarTypeLU = c.Int(nullable: false),
                        FieldName = c.String(),
                        Query = c.String(),
                        FilterTypeLU = c.Int(),
                        Collection = c.String(),
                        DefaultValue = c.String(),
                        WhereClause = c.String(),
                        OrderByClause = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.sysBpmsApplicationPages", t => t.ApplicationPageID)
                .ForeignKey("dbo.sysBpmsDBConnections", t => t.DBConnectionID)
                .ForeignKey("dbo.sysBpmsEntityDefs", t => t.EntityDefID)
                .ForeignKey("dbo.sysBpmsProcesses", t => t.ProcessID)
                .Index(t => t.ProcessID)
                .Index(t => t.ApplicationPageID)
                .Index(t => t.EntityDefID)
                .Index(t => t.DBConnectionID);
            
            CreateTable(
                "dbo.sysBpmsDBConnections",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        DataSource = c.String(),
                        InitialCatalog = c.String(),
                        UserID = c.String(),
                        Password = c.String(),
                        IntegratedSecurity = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.sysBpmsThreadVariables",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        ThreadID = c.Guid(nullable: false),
                        VariableID = c.Guid(nullable: false),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.sysBpmsThreads", t => t.ThreadID)
                .ForeignKey("dbo.sysBpmsVariables", t => t.VariableID)
                .Index(t => t.ThreadID)
                .Index(t => t.VariableID);
            
            CreateTable(
                "dbo.sysBpmsVariableDependencies",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        DependentVariableID = c.Guid(nullable: false),
                        DependentPropertyName = c.String(),
                        ToVariableID = c.Guid(),
                        ToPropertyName = c.String(),
                        Description = c.String(),
                        sysBpmsVariable_ID = c.Guid(),
                        sysBpmsVariable_ID1 = c.Guid(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.sysBpmsVariables", t => t.DependentVariableID)
                .ForeignKey("dbo.sysBpmsVariables", t => t.ToVariableID)
                .ForeignKey("dbo.sysBpmsVariables", t => t.sysBpmsVariable_ID)
                .ForeignKey("dbo.sysBpmsVariables", t => t.sysBpmsVariable_ID1)
                .Index(t => t.DependentVariableID)
                .Index(t => t.ToVariableID)
                .Index(t => t.sysBpmsVariable_ID)
                .Index(t => t.sysBpmsVariable_ID1);
            
            CreateTable(
                "dbo.sysBpmsThreadTasks",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        ThreadID = c.Guid(nullable: false),
                        TaskID = c.Guid(nullable: false),
                        OwnerUserID = c.Guid(),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(),
                        Description = c.String(),
                        OwnerRole = c.String(),
                        PriorityLU = c.Int(),
                        StatusLU = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.sysBpmsTasks", t => t.TaskID)
                .ForeignKey("dbo.sysBpmsThreads", t => t.ThreadID)
                .ForeignKey("dbo.sysBpmsUsers", t => t.OwnerUserID)
                .Index(t => t.ThreadID)
                .Index(t => t.TaskID)
                .Index(t => t.OwnerUserID);
            
            CreateTable(
                "dbo.sysBpmsTasks",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        ElementID = c.String(maxLength: 128),
                        TypeLU = c.Int(nullable: false),
                        Code = c.String(),
                        MarkerTypeLU = c.Int(),
                        OwnerTypeLU = c.Int(),
                        RoleName = c.String(),
                        Rule = c.String(),
                        UserID = c.String(),
                        ProcessID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.sysBpmsElements", t => t.ElementID)
                .ForeignKey("dbo.sysBpmsProcesses", t => t.ProcessID)
                .Index(t => t.ElementID)
                .Index(t => t.ProcessID);
            
            CreateTable(
                "dbo.sysBpmsSteps",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        TaskID = c.Guid(nullable: false),
                        Position = c.Int(nullable: false),
                        Name = c.String(),
                        DynamicFormID = c.Guid(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.sysBpmsDynamicForms", t => t.DynamicFormID)
                .ForeignKey("dbo.sysBpmsTasks", t => t.TaskID)
                .Index(t => t.TaskID)
                .Index(t => t.DynamicFormID);
            
            CreateTable(
                "dbo.sysBpmsUsers",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        Username = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Email = c.String(),
                        Tel = c.String(),
                        Mobile = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.sysBpmsDepartmentMembers",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        DepartmentID = c.Guid(nullable: false),
                        RoleLU = c.Int(nullable: false),
                        UserID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.sysBpmsDepartments", t => t.DepartmentID)
                .ForeignKey("dbo.sysBpmsUsers", t => t.UserID)
                .Index(t => t.DepartmentID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.sysBpmsDepartments",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        DepartmentID = c.Guid(),
                        Name = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.sysBpmsDepartments", t => t.DepartmentID)
                .Index(t => t.DepartmentID);
            
            CreateTable(
                "dbo.sysBpmsGateways",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        ElementID = c.String(maxLength: 128),
                        DefaultSequenceFlowID = c.Guid(),
                        TypeLU = c.Int(),
                        TraceToStart = c.String(),
                        ProcessID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.sysBpmsElements", t => t.ElementID)
                .ForeignKey("dbo.sysBpmsProcesses", t => t.ProcessID)
                .ForeignKey("dbo.sysBpmsSequenceFlows", t => t.DefaultSequenceFlowID)
                .Index(t => t.ElementID)
                .Index(t => t.DefaultSequenceFlowID)
                .Index(t => t.ProcessID);
            
            CreateTable(
                "dbo.sysBpmsConditions",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        GatewayID = c.Guid(nullable: false),
                        SequenceFlowID = c.Guid(),
                        Code = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.sysBpmsGateways", t => t.GatewayID)
                .ForeignKey("dbo.sysBpmsSequenceFlows", t => t.SequenceFlowID)
                .Index(t => t.GatewayID)
                .Index(t => t.SequenceFlowID);
            
            CreateTable(
                "dbo.sysBpmsSequenceFlows",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        ElementID = c.String(maxLength: 128),
                        ProcessID = c.Guid(nullable: false),
                        Name = c.String(),
                        SourceElementID = c.String(),
                        TargetElementID = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.sysBpmsElements", t => t.ElementID)
                .ForeignKey("dbo.sysBpmsProcesses", t => t.ProcessID)
                .Index(t => t.ElementID)
                .Index(t => t.ProcessID);
            
            CreateTable(
                "dbo.sysBpmsLanes",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        ElementID = c.String(maxLength: 128),
                        ProcessID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.sysBpmsElements", t => t.ElementID)
                .ForeignKey("dbo.sysBpmsProcesses", t => t.ProcessID)
                .Index(t => t.ElementID)
                .Index(t => t.ProcessID);
            
            CreateTable(
                "dbo.sysBpmsProcessGroups",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        ProcessGroupID = c.Guid(),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.sysBpmsProcessGroups", t => t.ProcessGroupID)
                .Index(t => t.ProcessGroupID);
            
            CreateTable(
                "dbo.sysBpmsConfigurations",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Label = c.String(),
                        DefaultValue = c.String(),
                        Value = c.String(),
                        LastUpdateOn = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.sysBpmsEmailAccounts",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        ObjectTypeLU = c.Int(nullable: false),
                        ObjectID = c.Guid(),
                        SMTP = c.String(),
                        Port = c.String(),
                        MailUserName = c.String(),
                        MailPassword = c.String(),
                        Email = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.sysBpmsLURows",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        LUTableID = c.Guid(nullable: false),
                        NameOf = c.String(),
                        CodeOf = c.String(),
                        DisplayOrder = c.Int(nullable: false),
                        IsSystemic = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.sysBpmsLUTables", t => t.LUTableID)
                .Index(t => t.LUTableID);
            
            CreateTable(
                "dbo.sysBpmsLUTables",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        NameOf = c.String(),
                        Alias = c.String(),
                        IsSystemic = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.sysBpmsLURows", "FK_sysBpmsLURows_sysBpmsLUTables_LUTableID");
            DropForeignKey("dbo.sysBpmsApplicationPageAccesses", "FK_sysBpmsApplicationPageAccesses_sysBpmsUsers_UserID");
            DropForeignKey("dbo.sysBpmsApplicationPageAccesses", "FK_sysBpmsApplicationPageAccesses_sysBpmsDepartments_DepartmentID");
            DropForeignKey("dbo.sysBpmsApplicationPageAccesses", "FK_sysBpmsApplicationPageAccesses_sysBpmsApplicationPages_ApplicationPageID");
            DropForeignKey("dbo.sysBpmsDynamicForms", "FK_sysBpmsDynamicForms_sysBpmsProcesses_ProcessId");
            DropForeignKey("dbo.sysBpmsProcesses", "FK_sysBpmsProcesses_sysBpmsProcessGroups_ProcessGroupID");
            DropForeignKey("dbo.sysBpmsProcessGroups", "FK_sysBpmsProcessGroups_sysBpmsProcessGroups_ProcessGroupID");
            DropForeignKey("dbo.sysBpmsProcesses", "FK_sysBpmsProcesses_sysBpmsProcesses_ParentProcessID");
            DropForeignKey("dbo.sysBpmsElements", "FK_sysBpmsElements_sysBpmsProcesses_ProcessID");
            DropForeignKey("dbo.sysBpmsLanes", "FK_sysBpmsLanes_sysBpmsProcesses_ProcessID");
            DropForeignKey("dbo.sysBpmsLanes", "FK_sysBpmsLanes_sysBpmsElements_ElementID");
            DropForeignKey("dbo.sysBpmsGateways", "FK_sysBpmsGateways_sysBpmsSequenceFlows_DefaultSequenceFlowID");
            DropForeignKey("dbo.sysBpmsGateways", "FK_sysBpmsGateways_sysBpmsProcesses_ProcessID");
            DropForeignKey("dbo.sysBpmsGateways", "FK_sysBpmsGateways_sysBpmsElements_ElementID");
            DropForeignKey("dbo.sysBpmsConditions", "FK_sysBpmsConditions_sysBpmsSequenceFlows_SequenceFlowID");
            DropForeignKey("dbo.sysBpmsSequenceFlows", "FK_sysBpmsSequenceFlows_sysBpmsProcesses_ProcessID");
            DropForeignKey("dbo.sysBpmsSequenceFlows", "FK_sysBpmsSequenceFlows_sysBpmsElements_ElementID");
            DropForeignKey("dbo.sysBpmsConditions", "FK_sysBpmsConditions_sysBpmsGateways_GatewayID");
            DropForeignKey("dbo.sysBpmsThreadEvents", "FK_sysBpmsThreadEvents_sysBpmsThreadTasks_ThreadTaskID");
            DropForeignKey("dbo.sysBpmsThreadEvents", "FK_sysBpmsThreadEvents_sysBpmsThreads_ThreadID");
            DropForeignKey("dbo.sysBpmsThreads", "FK_sysBpmsThreads_sysBpmsUsers_UserID");
            DropForeignKey("dbo.sysBpmsThreadTasks", "FK_sysBpmsThreadTasks_sysBpmsUsers_OwnerUserID");
            DropForeignKey("dbo.sysBpmsDepartmentMembers", "FK_sysBpmsDepartmentMembers_sysBpmsUsers_UserID");
            DropForeignKey("dbo.sysBpmsDepartmentMembers", "FK_sysBpmsDepartmentMembers_sysBpmsDepartments_DepartmentID");
            DropForeignKey("dbo.sysBpmsDepartments", "FK_sysBpmsDepartments_sysBpmsDepartments_DepartmentID");
            DropForeignKey("dbo.sysBpmsThreadTasks", "FK_sysBpmsThreadTasks_sysBpmsThreads_ThreadID");
            DropForeignKey("dbo.sysBpmsThreadTasks", "FK_sysBpmsThreadTasks_sysBpmsTasks_TaskID");
            DropForeignKey("dbo.sysBpmsSteps", "FK_sysBpmsSteps_sysBpmsTasks_TaskID");
            DropForeignKey("dbo.sysBpmsSteps", "FK_sysBpmsSteps_sysBpmsDynamicForms_DynamicFormID");
            DropForeignKey("dbo.sysBpmsTasks", "FK_sysBpmsTasks_sysBpmsProcesses_ProcessID");
            DropForeignKey("dbo.sysBpmsTasks", "FK_sysBpmsTasks_sysBpmsElements_ElementID");
            DropForeignKey("dbo.sysBpmsThreads", "FK_sysBpmsThreads_sysBpmsProcesses_ProcessID");
            DropForeignKey("dbo.sysBpmsDocuments", "FK_sysBpmsDocuments_sysBpmsThreads_ThreadID");
            DropForeignKey("dbo.sysBpmsDocuments", "FK_sysBpmsDocuments_sysBpmsEntityDefs_EntityDefID");
            DropForeignKey("dbo.sysBpmsVariableDependencies", "FK_sysBpmsVariableDependencies_sysBpmsVariables_sysBpmsVariable_ID1");
            DropForeignKey("dbo.sysBpmsVariableDependencies", "FK_sysBpmsVariableDependencies_sysBpmsVariables_sysBpmsVariable_ID");
            DropForeignKey("dbo.sysBpmsVariableDependencies", "FK_sysBpmsVariableDependencies_sysBpmsVariables_ToVariableID");
            DropForeignKey("dbo.sysBpmsVariableDependencies", "FK_sysBpmsVariableDependencies_sysBpmsVariables_DependentVariableID");
            DropForeignKey("dbo.sysBpmsThreadVariables", "FK_sysBpmsThreadVariables_sysBpmsVariables_VariableID");
            DropForeignKey("dbo.sysBpmsThreadVariables", "FK_sysBpmsThreadVariables_sysBpmsThreads_ThreadID");
            DropForeignKey("dbo.sysBpmsVariables", "FK_sysBpmsVariables_sysBpmsProcesses_ProcessID");
            DropForeignKey("dbo.sysBpmsVariables", "FK_sysBpmsVariables_sysBpmsEntityDefs_EntityDefID");
            DropForeignKey("dbo.sysBpmsVariables", "FK_sysBpmsVariables_sysBpmsDBConnections_DBConnectionID");
            DropForeignKey("dbo.sysBpmsVariables", "FK_sysBpmsVariables_sysBpmsApplicationPages_ApplicationPageID");
            DropForeignKey("dbo.sysBpmsDocuments", "FK_sysBpmsDocuments_sysBpmsDocumentDefs_DocumentDefID");
            DropForeignKey("dbo.sysBpmsDocumentDefs", "FK_sysBpmsDocumentDefs_sysBpmsDocumentFolders_DocumentFolderID");
            DropForeignKey("dbo.sysBpmsDocumentFolders", "FK_sysBpmsDocumentFolders_sysBpmsDocumentFolders_DocumentFolderID");
            DropForeignKey("dbo.sysBpmsThreadEvents", "FK_sysBpmsThreadEvents_sysBpmsEvents_EventID");
            DropForeignKey("dbo.sysBpmsEvents", "FK_sysBpmsEvents_sysBpmsProcesses_ProcessID");
            DropForeignKey("dbo.sysBpmsEvents", "FK_sysBpmsEvents_sysBpmsMessageTypes_MessageTypeID");
            DropForeignKey("dbo.sysBpmsEvents", "FK_sysBpmsEvents_sysBpmsElements_ElementID");
            DropForeignKey("dbo.sysBpmsDynamicForms", "FK_sysBpmsDynamicForms_sysBpmsApplicationPages_ApplicationPageID");
            DropIndex("dbo.sysBpmsLURows", new[] { "LUTableID" });
            DropIndex("dbo.sysBpmsProcessGroups", new[] { "ProcessGroupID" });
            DropIndex("dbo.sysBpmsLanes", new[] { "ProcessID" });
            DropIndex("dbo.sysBpmsLanes", new[] { "ElementID" });
            DropIndex("dbo.sysBpmsSequenceFlows", new[] { "ProcessID" });
            DropIndex("dbo.sysBpmsSequenceFlows", new[] { "ElementID" });
            DropIndex("dbo.sysBpmsConditions", new[] { "SequenceFlowID" });
            DropIndex("dbo.sysBpmsConditions", new[] { "GatewayID" });
            DropIndex("dbo.sysBpmsGateways", new[] { "ProcessID" });
            DropIndex("dbo.sysBpmsGateways", new[] { "DefaultSequenceFlowID" });
            DropIndex("dbo.sysBpmsGateways", new[] { "ElementID" });
            DropIndex("dbo.sysBpmsDepartments", new[] { "DepartmentID" });
            DropIndex("dbo.sysBpmsDepartmentMembers", new[] { "UserID" });
            DropIndex("dbo.sysBpmsDepartmentMembers", new[] { "DepartmentID" });
            DropIndex("dbo.sysBpmsSteps", new[] { "DynamicFormID" });
            DropIndex("dbo.sysBpmsSteps", new[] { "TaskID" });
            DropIndex("dbo.sysBpmsTasks", new[] { "ProcessID" });
            DropIndex("dbo.sysBpmsTasks", new[] { "ElementID" });
            DropIndex("dbo.sysBpmsThreadTasks", new[] { "OwnerUserID" });
            DropIndex("dbo.sysBpmsThreadTasks", new[] { "TaskID" });
            DropIndex("dbo.sysBpmsThreadTasks", new[] { "ThreadID" });
            DropIndex("dbo.sysBpmsVariableDependencies", new[] { "sysBpmsVariable_ID1" });
            DropIndex("dbo.sysBpmsVariableDependencies", new[] { "sysBpmsVariable_ID" });
            DropIndex("dbo.sysBpmsVariableDependencies", new[] { "ToVariableID" });
            DropIndex("dbo.sysBpmsVariableDependencies", new[] { "DependentVariableID" });
            DropIndex("dbo.sysBpmsThreadVariables", new[] { "VariableID" });
            DropIndex("dbo.sysBpmsThreadVariables", new[] { "ThreadID" });
            DropIndex("dbo.sysBpmsVariables", new[] { "DBConnectionID" });
            DropIndex("dbo.sysBpmsVariables", new[] { "EntityDefID" });
            DropIndex("dbo.sysBpmsVariables", new[] { "ApplicationPageID" });
            DropIndex("dbo.sysBpmsVariables", new[] { "ProcessID" });
            DropIndex("dbo.sysBpmsDocumentFolders", new[] { "DocumentFolderID" });
            DropIndex("dbo.sysBpmsDocumentDefs", new[] { "DocumentFolderID" });
            DropIndex("dbo.sysBpmsDocuments", new[] { "ThreadID" });
            DropIndex("dbo.sysBpmsDocuments", new[] { "EntityDefID" });
            DropIndex("dbo.sysBpmsDocuments", new[] { "DocumentDefID" });
            DropIndex("dbo.sysBpmsThreads", new[] { "UserID" });
            DropIndex("dbo.sysBpmsThreads", new[] { "ProcessID" });
            DropIndex("dbo.sysBpmsThreadEvents", new[] { "ThreadTaskID" });
            DropIndex("dbo.sysBpmsThreadEvents", new[] { "EventID" });
            DropIndex("dbo.sysBpmsThreadEvents", new[] { "ThreadID" });
            DropIndex("dbo.sysBpmsEvents", new[] { "MessageTypeID" });
            DropIndex("dbo.sysBpmsEvents", new[] { "ProcessID" });
            DropIndex("dbo.sysBpmsEvents", new[] { "ElementID" });
            DropIndex("dbo.sysBpmsElements", new[] { "ProcessID" });
            DropIndex("dbo.sysBpmsProcesses", new[] { "ProcessGroupID" });
            DropIndex("dbo.sysBpmsProcesses", new[] { "ParentProcessID" });
            DropIndex("dbo.sysBpmsDynamicForms", new[] { "ApplicationPageID" });
            DropIndex("dbo.sysBpmsDynamicForms", new[] { "ProcessId" });
            DropIndex("dbo.sysBpmsApplicationPageAccesses", new[] { "UserID" });
            DropIndex("dbo.sysBpmsApplicationPageAccesses", new[] { "DepartmentID" });
            DropIndex("dbo.sysBpmsApplicationPageAccesses", new[] { "ApplicationPageID" });
            DropTable("dbo.sysBpmsLUTables");
            DropTable("dbo.sysBpmsLURows");
            DropTable("dbo.sysBpmsEmailAccounts");
            DropTable("dbo.sysBpmsConfigurations");
            DropTable("dbo.sysBpmsProcessGroups");
            DropTable("dbo.sysBpmsLanes");
            DropTable("dbo.sysBpmsSequenceFlows");
            DropTable("dbo.sysBpmsConditions");
            DropTable("dbo.sysBpmsGateways");
            DropTable("dbo.sysBpmsDepartments");
            DropTable("dbo.sysBpmsDepartmentMembers");
            DropTable("dbo.sysBpmsUsers");
            DropTable("dbo.sysBpmsSteps");
            DropTable("dbo.sysBpmsTasks");
            DropTable("dbo.sysBpmsThreadTasks");
            DropTable("dbo.sysBpmsVariableDependencies");
            DropTable("dbo.sysBpmsThreadVariables");
            DropTable("dbo.sysBpmsDBConnections");
            DropTable("dbo.sysBpmsVariables");
            DropTable("dbo.sysBpmsEntityDefs");
            DropTable("dbo.sysBpmsDocumentFolders");
            DropTable("dbo.sysBpmsDocumentDefs");
            DropTable("dbo.sysBpmsDocuments");
            DropTable("dbo.sysBpmsThreads");
            DropTable("dbo.sysBpmsThreadEvents");
            DropTable("dbo.sysBpmsMessageTypes");
            DropTable("dbo.sysBpmsEvents");
            DropTable("dbo.sysBpmsElements");
            DropTable("dbo.sysBpmsProcesses");
            DropTable("dbo.sysBpmsDynamicForms");
            DropTable("dbo.sysBpmsApplicationPages");
            DropTable("dbo.sysBpmsApplicationPageAccesses");
            DropTable("dbo.sysBpmsAPIAccesses");
        }
    }
}
