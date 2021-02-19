namespace DynamicBusiness.BPMS.BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class version_0 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.sysBpmsAPIAccess",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 500),
                        IPAddress = c.String(nullable: false, maxLength: 50),
                        AccessKey = c.String(nullable: false, maxLength: 500),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.sysBpmsApplicationPageAccess",
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
                .ForeignKey("dbo.sysBpmsApplicationPage", t => t.ApplicationPageID)
                .ForeignKey("dbo.sysBpmsDepartment", t => t.DepartmentID)
                .ForeignKey("dbo.sysBpmsUser", t => t.UserID)
                .Index(t => t.ApplicationPageID)
                .Index(t => t.DepartmentID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.sysBpmsApplicationPage",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        GroupLU = c.Int(nullable: false),
                        Description = c.String(),
                        ShowInMenu = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.sysBpmsDynamicForm",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        ProcessId = c.Guid(),
                        ApplicationPageID = c.Guid(),
                        Name = c.String(nullable: false, maxLength: 500),
                        DesignJson = c.String(),
                        OnExitFormCode = c.String(),
                        OnEntryFormCode = c.String(),
                        Version = c.Int(),
                        ConfigXML = c.String(),
                        ShowInOverview = c.Boolean(),
                        SourceCode = c.String(),
                        CreatedBy = c.String(nullable: false, maxLength: 500),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(nullable: false, maxLength: 500),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.sysBpmsApplicationPage", t => t.ApplicationPageID)
                .ForeignKey("dbo.sysBpmsProcess", t => t.ProcessId)
                .Index(t => t.ProcessId)
                .Index(t => t.ApplicationPageID);
            
            CreateTable(
                "dbo.sysBpmsProcess",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        FormattedNumber = c.String(),
                        Number = c.Int(),
                        Name = c.String(nullable: false, maxLength: 500),
                        Description = c.String(),
                        ProcessVersion = c.Int(),
                        StatusLU = c.Int(),
                        CreatorUsername = c.String(maxLength: 500),
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
                .ForeignKey("dbo.sysBpmsProcess", t => t.ParentProcessID)
                .ForeignKey("dbo.sysBpmsProcessGroup", t => t.ProcessGroupID)
                .Index(t => t.ParentProcessID)
                .Index(t => t.ProcessGroupID);
            
            CreateTable(
                "dbo.sysBpmsElement",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 100),
                        ProcessID = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 500),
                        TypeLU = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ID, t.ProcessID })
                .ForeignKey("dbo.sysBpmsProcess", t => t.ProcessID)
                .Index(t => t.ProcessID);
            
            CreateTable(
                "dbo.sysBpmsEvent",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        ElementID = c.String(nullable: false, maxLength: 100),
                        TypeLU = c.Int(nullable: false),
                        ConfigurationXML = c.String(),
                        SubType = c.Int(),
                        RefElementID = c.String(maxLength: 100),
                        CancelActivity = c.Boolean(),
                        ProcessID = c.Guid(nullable: false),
                        MessageTypeID = c.Guid(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.sysBpmsElement", t => new { t.ElementID, t.ProcessID })
                .ForeignKey("dbo.sysBpmsMessageType", t => t.MessageTypeID)
                .ForeignKey("dbo.sysBpmsProcess", t => t.ProcessID)
                .Index(t => new { t.ElementID, t.ProcessID })
                .Index(t => t.MessageTypeID);
            
            CreateTable(
                "dbo.sysBpmsMessageType",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 500),
                        ParamsXML = c.String(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.sysBpmsThreadEvent",
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
                .ForeignKey("dbo.sysBpmsEvent", t => t.EventID)
                .ForeignKey("dbo.sysBpmsThread", t => t.ThreadID)
                .ForeignKey("dbo.sysBpmsThreadTask", t => t.ThreadTaskID)
                .Index(t => t.ThreadID)
                .Index(t => t.EventID)
                .Index(t => t.ThreadTaskID);
            
            CreateTable(
                "dbo.sysBpmsThread",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(),
                        StatusLU = c.Int(nullable: false),
                        GatewayStatusXml = c.String(),
                        ProcessID = c.Guid(nullable: false),
                        UserID = c.Guid(),
                        FormattedNumber = c.String(maxLength: 50),
                        Number = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.sysBpmsProcess", t => t.ProcessID)
                .ForeignKey("dbo.sysBpmsUser", t => t.UserID)
                .Index(t => t.ProcessID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.sysBpmsDocument",
                c => new
                    {
                        GUID = c.Guid(nullable: false, identity: true),
                        IsDeleted = c.Boolean(nullable: false),
                        DocumentDefID = c.Guid(nullable: false),
                        EntityDefID = c.Guid(),
                        EntityID = c.Guid(),
                        ThreadID = c.Guid(),
                        AtachDateOf = c.DateTime(nullable: false),
                        FileExtention = c.String(nullable: false, maxLength: 10),
                        CaptionOf = c.String(nullable: false, maxLength: 1000),
                    })
                .PrimaryKey(t => t.GUID)
                .ForeignKey("dbo.sysBpmsDocumentDef", t => t.DocumentDefID)
                .ForeignKey("dbo.sysBpmsEntityDef", t => t.EntityDefID)
                .ForeignKey("dbo.sysBpmsThread", t => t.ThreadID)
                .Index(t => t.DocumentDefID)
                .Index(t => t.EntityDefID)
                .Index(t => t.ThreadID);
            
            CreateTable(
                "dbo.sysBpmsDocumentDef",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        DocumentFolderID = c.Guid(nullable: false),
                        NameOf = c.String(nullable: false),
                        DisplayName = c.String(nullable: false),
                        MaxSize = c.Int(),
                        ValidExtentions = c.String(nullable: false),
                        IsMandatory = c.Boolean(nullable: false),
                        Description = c.String(),
                        IsSystemic = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.sysBpmsDocumentFolder", t => t.DocumentFolderID)
                .Index(t => t.DocumentFolderID);
            
            CreateTable(
                "dbo.sysBpmsDocumentFolder",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        NameOf = c.String(nullable: false, maxLength: 500),
                        DisplayName = c.String(nullable: false, maxLength: 500),
                        DocumentFolderID = c.Guid(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.sysBpmsDocumentFolder", t => t.DocumentFolderID)
                .Index(t => t.DocumentFolderID);
            
            CreateTable(
                "dbo.sysBpmsEntityDef",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        DisplayName = c.String(nullable: false, maxLength: 500),
                        Name = c.String(nullable: false, maxLength: 500),
                        DesignXML = c.String(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.sysBpmsVariable",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        ProcessID = c.Guid(),
                        ApplicationPageID = c.Guid(),
                        EntityDefID = c.Guid(),
                        DBConnectionID = c.Guid(),
                        Name = c.String(nullable: false, maxLength: 500),
                        VarTypeLU = c.Int(nullable: false),
                        FieldName = c.String(maxLength: 250),
                        Query = c.String(),
                        FilterTypeLU = c.Int(),
                        Collection = c.String(),
                        DefaultValue = c.String(),
                        WhereClause = c.String(),
                        OrderByClause = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.sysBpmsApplicationPage", t => t.ApplicationPageID)
                .ForeignKey("dbo.sysBpmsDBConnection", t => t.DBConnectionID)
                .ForeignKey("dbo.sysBpmsEntityDef", t => t.EntityDefID)
                .ForeignKey("dbo.sysBpmsProcess", t => t.ProcessID)
                .Index(t => t.ProcessID)
                .Index(t => t.ApplicationPageID)
                .Index(t => t.EntityDefID)
                .Index(t => t.DBConnectionID);
            
            CreateTable(
                "dbo.sysBpmsDBConnection",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 500),
                        DataSource = c.String(nullable: false, maxLength: 500),
                        InitialCatalog = c.String(nullable: false, maxLength: 500),
                        UserID = c.String(nullable: false, maxLength: 500),
                        Password = c.String(nullable: false, maxLength: 500),
                        IntegratedSecurity = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.sysBpmsThreadVariable",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        ThreadID = c.Guid(nullable: false),
                        VariableID = c.Guid(nullable: false),
                        Value = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.sysBpmsThread", t => t.ThreadID)
                .ForeignKey("dbo.sysBpmsVariable", t => t.VariableID)
                .Index(t => t.ThreadID)
                .Index(t => t.VariableID);
            
            CreateTable(
                "dbo.sysBpmsVariableDependency",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        DependentVariableID = c.Guid(nullable: false),
                        DependentPropertyName = c.String(nullable: false, maxLength: 250),
                        ToVariableID = c.Guid(),
                        ToPropertyName = c.String(nullable: false, maxLength: 250),
                        Description = c.String(),
                        sysBpmsVariable_ID = c.Guid(),
                        sysBpmsVariable_ID1 = c.Guid(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.sysBpmsVariable", t => t.DependentVariableID)
                .ForeignKey("dbo.sysBpmsVariable", t => t.ToVariableID)
                .ForeignKey("dbo.sysBpmsVariable", t => t.sysBpmsVariable_ID)
                .ForeignKey("dbo.sysBpmsVariable", t => t.sysBpmsVariable_ID1)
                .Index(t => t.DependentVariableID)
                .Index(t => t.ToVariableID)
                .Index(t => t.sysBpmsVariable_ID)
                .Index(t => t.sysBpmsVariable_ID1);
            
            CreateTable(
                "dbo.sysBpmsThreadTask",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        ThreadID = c.Guid(nullable: false),
                        TaskID = c.Guid(nullable: false),
                        OwnerUserID = c.Guid(),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(),
                        Description = c.String(nullable: false),
                        OwnerRole = c.String(),
                        PriorityLU = c.Int(),
                        StatusLU = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.sysBpmsTask", t => t.TaskID)
                .ForeignKey("dbo.sysBpmsThread", t => t.ThreadID)
                .ForeignKey("dbo.sysBpmsUser", t => t.OwnerUserID)
                .Index(t => t.ThreadID)
                .Index(t => t.TaskID)
                .Index(t => t.OwnerUserID);
            
            CreateTable(
                "dbo.sysBpmsTask",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        ElementID = c.String(nullable: false, maxLength: 100),
                        TypeLU = c.Int(nullable: false),
                        Code = c.String(nullable: false),
                        MarkerTypeLU = c.Int(),
                        OwnerTypeLU = c.Int(),
                        RoleName = c.String(),
                        Rule = c.String(),
                        UserID = c.String(),
                        ProcessID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.sysBpmsElement", t => new { t.ElementID, t.ProcessID })
                .ForeignKey("dbo.sysBpmsProcess", t => t.ProcessID)
                .Index(t => new { t.ElementID, t.ProcessID });
            
            CreateTable(
                "dbo.sysBpmsStep",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        TaskID = c.Guid(nullable: false),
                        Position = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 500),
                        DynamicFormID = c.Guid(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.sysBpmsDynamicForm", t => t.DynamicFormID)
                .ForeignKey("dbo.sysBpmsTask", t => t.TaskID)
                .Index(t => t.TaskID)
                .Index(t => t.DynamicFormID);
            
            CreateTable(
                "dbo.sysBpmsUser",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        Username = c.String(nullable: false, maxLength: 500),
                        FirstName = c.String(maxLength: 500),
                        LastName = c.String(maxLength: 500),
                        Email = c.String(maxLength: 500),
                        Tel = c.String(maxLength: 30),
                        Mobile = c.String(maxLength: 30),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.sysBpmsDepartmentMember",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        DepartmentID = c.Guid(nullable: false),
                        RoleLU = c.Int(nullable: false),
                        UserID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.sysBpmsDepartment", t => t.DepartmentID)
                .ForeignKey("dbo.sysBpmsUser", t => t.UserID)
                .Index(t => t.DepartmentID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.sysBpmsDepartment",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        DepartmentID = c.Guid(),
                        Name = c.String(nullable: false, maxLength: 500),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.sysBpmsDepartment", t => t.DepartmentID)
                .Index(t => t.DepartmentID);
            
            CreateTable(
                "dbo.sysBpmsGateway",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        ElementID = c.String(nullable: false, maxLength: 100),
                        DefaultSequenceFlowID = c.Guid(),
                        TypeLU = c.Int(),
                        TraceToStart = c.String(),
                        ProcessID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.sysBpmsElement", t => new { t.ElementID, t.ProcessID })
                .ForeignKey("dbo.sysBpmsProcess", t => t.ProcessID)
                .ForeignKey("dbo.sysBpmsSequenceFlow", t => t.DefaultSequenceFlowID)
                .Index(t => new { t.ElementID, t.ProcessID })
                .Index(t => t.DefaultSequenceFlowID);
            
            CreateTable(
                "dbo.sysBpmsCondition",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        GatewayID = c.Guid(nullable: false),
                        SequenceFlowID = c.Guid(),
                        Code = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.sysBpmsGateway", t => t.GatewayID)
                .ForeignKey("dbo.sysBpmsSequenceFlow", t => t.SequenceFlowID)
                .Index(t => t.GatewayID)
                .Index(t => t.SequenceFlowID);
            
            CreateTable(
                "dbo.sysBpmsSequenceFlow",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        ElementID = c.String(nullable: false, maxLength: 100),
                        ProcessID = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 500),
                        SourceElementID = c.String(nullable: false, maxLength: 100),
                        TargetElementID = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.sysBpmsElement", t => new { t.ElementID, t.ProcessID })
                .ForeignKey("dbo.sysBpmsProcess", t => t.ProcessID)
                .Index(t => new { t.ElementID, t.ProcessID });
            
            CreateTable(
                "dbo.sysBpmsLane",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        ElementID = c.String(nullable: false, maxLength: 100),
                        ProcessID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.sysBpmsElement", t => new { t.ElementID, t.ProcessID })
                .ForeignKey("dbo.sysBpmsProcess", t => t.ProcessID)
                .Index(t => new { t.ElementID, t.ProcessID });
            
            CreateTable(
                "dbo.sysBpmsProcessGroup",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        ProcessGroupID = c.Guid(),
                        Name = c.String(nullable: false, maxLength: 500),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.sysBpmsProcessGroup", t => t.ProcessGroupID)
                .Index(t => t.ProcessGroupID);
            
            CreateTable(
                "dbo.sysBpmsConfiguration",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 500),
                        Label = c.String(nullable: false, maxLength: 500),
                        DefaultValue = c.String(nullable: false),
                        Value = c.String(nullable: false),
                        LastUpdateOn = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.sysBpmsEmailAccount",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        ObjectTypeLU = c.Int(nullable: false),
                        ObjectID = c.Guid(),
                        SMTP = c.String(nullable: false, maxLength: 500),
                        Port = c.String(nullable: false, maxLength: 10),
                        MailPassword = c.String(nullable: false, maxLength: 500),
                        Email = c.String(nullable: false, maxLength: 500),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.sysBpmsLURow",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        LUTableID = c.Guid(nullable: false),
                        NameOf = c.String(nullable: false, maxLength: 500),
                        CodeOf = c.String(nullable: false, maxLength: 500),
                        DisplayOrder = c.Int(nullable: false),
                        IsSystemic = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.sysBpmsLUTable", t => t.LUTableID)
                .Index(t => t.LUTableID);
            
            CreateTable(
                "dbo.sysBpmsLUTable",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        NameOf = c.String(nullable: false, maxLength: 500),
                        Alias = c.String(nullable: false, maxLength: 500),
                        IsSystemic = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.sysBpmsLURow", "LUTableID", "dbo.sysBpmsLUTable");
            DropForeignKey("dbo.sysBpmsApplicationPageAccess", "UserID", "dbo.sysBpmsUser");
            DropForeignKey("dbo.sysBpmsApplicationPageAccess", "DepartmentID", "dbo.sysBpmsDepartment");
            DropForeignKey("dbo.sysBpmsApplicationPageAccess", "ApplicationPageID", "dbo.sysBpmsApplicationPage");
            DropForeignKey("dbo.sysBpmsDynamicForm", "ProcessId", "dbo.sysBpmsProcess");
            DropForeignKey("dbo.sysBpmsProcess", "ProcessGroupID", "dbo.sysBpmsProcessGroup");
            DropForeignKey("dbo.sysBpmsProcessGroup", "ProcessGroupID", "dbo.sysBpmsProcessGroup");
            DropForeignKey("dbo.sysBpmsProcess", "ParentProcessID", "dbo.sysBpmsProcess");
            DropForeignKey("dbo.sysBpmsElement", "ProcessID", "dbo.sysBpmsProcess");
            DropForeignKey("dbo.sysBpmsLane", "ProcessID", "dbo.sysBpmsProcess");
            DropForeignKey("dbo.sysBpmsLane", new[] { "ElementID", "ProcessID" }, "dbo.sysBpmsElement");
            DropForeignKey("dbo.sysBpmsGateway", "DefaultSequenceFlowID", "dbo.sysBpmsSequenceFlow");
            DropForeignKey("dbo.sysBpmsGateway", "ProcessID", "dbo.sysBpmsProcess");
            DropForeignKey("dbo.sysBpmsGateway", new[] { "ElementID", "ProcessID" }, "dbo.sysBpmsElement");
            DropForeignKey("dbo.sysBpmsCondition", "SequenceFlowID", "dbo.sysBpmsSequenceFlow");
            DropForeignKey("dbo.sysBpmsSequenceFlow", "ProcessID", "dbo.sysBpmsProcess");
            DropForeignKey("dbo.sysBpmsSequenceFlow", new[] { "ElementID", "ProcessID" }, "dbo.sysBpmsElement");
            DropForeignKey("dbo.sysBpmsCondition", "GatewayID", "dbo.sysBpmsGateway");
            DropForeignKey("dbo.sysBpmsThreadEvent", "ThreadTaskID", "dbo.sysBpmsThreadTask");
            DropForeignKey("dbo.sysBpmsThreadEvent", "ThreadID", "dbo.sysBpmsThread");
            DropForeignKey("dbo.sysBpmsThread", "UserID", "dbo.sysBpmsUser");
            DropForeignKey("dbo.sysBpmsThreadTask", "OwnerUserID", "dbo.sysBpmsUser");
            DropForeignKey("dbo.sysBpmsDepartmentMember", "UserID", "dbo.sysBpmsUser");
            DropForeignKey("dbo.sysBpmsDepartmentMember", "DepartmentID", "dbo.sysBpmsDepartment");
            DropForeignKey("dbo.sysBpmsDepartment", "DepartmentID", "dbo.sysBpmsDepartment");
            DropForeignKey("dbo.sysBpmsThreadTask", "ThreadID", "dbo.sysBpmsThread");
            DropForeignKey("dbo.sysBpmsThreadTask", "TaskID", "dbo.sysBpmsTask");
            DropForeignKey("dbo.sysBpmsStep", "TaskID", "dbo.sysBpmsTask");
            DropForeignKey("dbo.sysBpmsStep", "DynamicFormID", "dbo.sysBpmsDynamicForm");
            DropForeignKey("dbo.sysBpmsTask", "ProcessID", "dbo.sysBpmsProcess");
            DropForeignKey("dbo.sysBpmsTask", new[] { "ElementID", "ProcessID" }, "dbo.sysBpmsElement");
            DropForeignKey("dbo.sysBpmsThread", "ProcessID", "dbo.sysBpmsProcess");
            DropForeignKey("dbo.sysBpmsDocument", "ThreadID", "dbo.sysBpmsThread");
            DropForeignKey("dbo.sysBpmsDocument", "EntityDefID", "dbo.sysBpmsEntityDef");
            DropForeignKey("dbo.sysBpmsVariableDependency", "sysBpmsVariable_ID1", "dbo.sysBpmsVariable");
            DropForeignKey("dbo.sysBpmsVariableDependency", "sysBpmsVariable_ID", "dbo.sysBpmsVariable");
            DropForeignKey("dbo.sysBpmsVariableDependency", "ToVariableID", "dbo.sysBpmsVariable");
            DropForeignKey("dbo.sysBpmsVariableDependency", "DependentVariableID", "dbo.sysBpmsVariable");
            DropForeignKey("dbo.sysBpmsThreadVariable", "VariableID", "dbo.sysBpmsVariable");
            DropForeignKey("dbo.sysBpmsThreadVariable", "ThreadID", "dbo.sysBpmsThread");
            DropForeignKey("dbo.sysBpmsVariable", "ProcessID", "dbo.sysBpmsProcess");
            DropForeignKey("dbo.sysBpmsVariable", "EntityDefID", "dbo.sysBpmsEntityDef");
            DropForeignKey("dbo.sysBpmsVariable", "DBConnectionID", "dbo.sysBpmsDBConnection");
            DropForeignKey("dbo.sysBpmsVariable", "ApplicationPageID", "dbo.sysBpmsApplicationPage");
            DropForeignKey("dbo.sysBpmsDocument", "DocumentDefID", "dbo.sysBpmsDocumentDef");
            DropForeignKey("dbo.sysBpmsDocumentDef", "DocumentFolderID", "dbo.sysBpmsDocumentFolder");
            DropForeignKey("dbo.sysBpmsDocumentFolder", "DocumentFolderID", "dbo.sysBpmsDocumentFolder");
            DropForeignKey("dbo.sysBpmsThreadEvent", "EventID", "dbo.sysBpmsEvent");
            DropForeignKey("dbo.sysBpmsEvent", "ProcessID", "dbo.sysBpmsProcess");
            DropForeignKey("dbo.sysBpmsEvent", "MessageTypeID", "dbo.sysBpmsMessageType");
            DropForeignKey("dbo.sysBpmsEvent", new[] { "ElementID", "ProcessID" }, "dbo.sysBpmsElement");
            DropForeignKey("dbo.sysBpmsDynamicForm", "ApplicationPageID", "dbo.sysBpmsApplicationPage");
            DropIndex("dbo.sysBpmsLURow", new[] { "LUTableID" });
            DropIndex("dbo.sysBpmsProcessGroup", new[] { "ProcessGroupID" });
            DropIndex("dbo.sysBpmsLane", new[] { "ElementID", "ProcessID" });
            DropIndex("dbo.sysBpmsSequenceFlow", new[] { "ElementID", "ProcessID" });
            DropIndex("dbo.sysBpmsCondition", new[] { "SequenceFlowID" });
            DropIndex("dbo.sysBpmsCondition", new[] { "GatewayID" });
            DropIndex("dbo.sysBpmsGateway", new[] { "DefaultSequenceFlowID" });
            DropIndex("dbo.sysBpmsGateway", new[] { "ElementID", "ProcessID" });
            DropIndex("dbo.sysBpmsDepartment", new[] { "DepartmentID" });
            DropIndex("dbo.sysBpmsDepartmentMember", new[] { "UserID" });
            DropIndex("dbo.sysBpmsDepartmentMember", new[] { "DepartmentID" });
            DropIndex("dbo.sysBpmsStep", new[] { "DynamicFormID" });
            DropIndex("dbo.sysBpmsStep", new[] { "TaskID" });
            DropIndex("dbo.sysBpmsTask", new[] { "ElementID", "ProcessID" });
            DropIndex("dbo.sysBpmsThreadTask", new[] { "OwnerUserID" });
            DropIndex("dbo.sysBpmsThreadTask", new[] { "TaskID" });
            DropIndex("dbo.sysBpmsThreadTask", new[] { "ThreadID" });
            DropIndex("dbo.sysBpmsVariableDependency", new[] { "sysBpmsVariable_ID1" });
            DropIndex("dbo.sysBpmsVariableDependency", new[] { "sysBpmsVariable_ID" });
            DropIndex("dbo.sysBpmsVariableDependency", new[] { "ToVariableID" });
            DropIndex("dbo.sysBpmsVariableDependency", new[] { "DependentVariableID" });
            DropIndex("dbo.sysBpmsThreadVariable", new[] { "VariableID" });
            DropIndex("dbo.sysBpmsThreadVariable", new[] { "ThreadID" });
            DropIndex("dbo.sysBpmsVariable", new[] { "DBConnectionID" });
            DropIndex("dbo.sysBpmsVariable", new[] { "EntityDefID" });
            DropIndex("dbo.sysBpmsVariable", new[] { "ApplicationPageID" });
            DropIndex("dbo.sysBpmsVariable", new[] { "ProcessID" });
            DropIndex("dbo.sysBpmsDocumentFolder", new[] { "DocumentFolderID" });
            DropIndex("dbo.sysBpmsDocumentDef", new[] { "DocumentFolderID" });
            DropIndex("dbo.sysBpmsDocument", new[] { "ThreadID" });
            DropIndex("dbo.sysBpmsDocument", new[] { "EntityDefID" });
            DropIndex("dbo.sysBpmsDocument", new[] { "DocumentDefID" });
            DropIndex("dbo.sysBpmsThread", new[] { "UserID" });
            DropIndex("dbo.sysBpmsThread", new[] { "ProcessID" });
            DropIndex("dbo.sysBpmsThreadEvent", new[] { "ThreadTaskID" });
            DropIndex("dbo.sysBpmsThreadEvent", new[] { "EventID" });
            DropIndex("dbo.sysBpmsThreadEvent", new[] { "ThreadID" });
            DropIndex("dbo.sysBpmsEvent", new[] { "MessageTypeID" });
            DropIndex("dbo.sysBpmsEvent", new[] { "ElementID", "ProcessID" });
            DropIndex("dbo.sysBpmsElement", new[] { "ProcessID" });
            DropIndex("dbo.sysBpmsProcess", new[] { "ProcessGroupID" });
            DropIndex("dbo.sysBpmsProcess", new[] { "ParentProcessID" });
            DropIndex("dbo.sysBpmsDynamicForm", new[] { "ApplicationPageID" });
            DropIndex("dbo.sysBpmsDynamicForm", new[] { "ProcessId" });
            DropIndex("dbo.sysBpmsApplicationPageAccess", new[] { "UserID" });
            DropIndex("dbo.sysBpmsApplicationPageAccess", new[] { "DepartmentID" });
            DropIndex("dbo.sysBpmsApplicationPageAccess", new[] { "ApplicationPageID" });
            DropTable("dbo.sysBpmsLUTable");
            DropTable("dbo.sysBpmsLURow");
            DropTable("dbo.sysBpmsEmailAccount");
            DropTable("dbo.sysBpmsConfiguration");
            DropTable("dbo.sysBpmsProcessGroup");
            DropTable("dbo.sysBpmsLane");
            DropTable("dbo.sysBpmsSequenceFlow");
            DropTable("dbo.sysBpmsCondition");
            DropTable("dbo.sysBpmsGateway");
            DropTable("dbo.sysBpmsDepartment");
            DropTable("dbo.sysBpmsDepartmentMember");
            DropTable("dbo.sysBpmsUser");
            DropTable("dbo.sysBpmsStep");
            DropTable("dbo.sysBpmsTask");
            DropTable("dbo.sysBpmsThreadTask");
            DropTable("dbo.sysBpmsVariableDependency");
            DropTable("dbo.sysBpmsThreadVariable");
            DropTable("dbo.sysBpmsDBConnection");
            DropTable("dbo.sysBpmsVariable");
            DropTable("dbo.sysBpmsEntityDef");
            DropTable("dbo.sysBpmsDocumentFolder");
            DropTable("dbo.sysBpmsDocumentDef");
            DropTable("dbo.sysBpmsDocument");
            DropTable("dbo.sysBpmsThread");
            DropTable("dbo.sysBpmsThreadEvent");
            DropTable("dbo.sysBpmsMessageType");
            DropTable("dbo.sysBpmsEvent");
            DropTable("dbo.sysBpmsElement");
            DropTable("dbo.sysBpmsProcess");
            DropTable("dbo.sysBpmsDynamicForm");
            DropTable("dbo.sysBpmsApplicationPage");
            DropTable("dbo.sysBpmsApplicationPageAccess");
            DropTable("dbo.sysBpmsAPIAccess");
        }
    }
}
