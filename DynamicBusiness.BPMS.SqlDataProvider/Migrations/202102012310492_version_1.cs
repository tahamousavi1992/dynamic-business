namespace DynamicBusiness.BPMS.BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class version_1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.sysBpmsAPIAccess",
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
                .ForeignKey("dbo.sysBpmsProcess", t => t.ParentProcessID)
                .ForeignKey("dbo.sysBpmsProcessGroup", t => t.ProcessGroupID)
                .Index(t => t.ParentProcessID)
                .Index(t => t.ProcessGroupID);
            
            CreateTable(
                "dbo.sysBpmsElement",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        TypeLU = c.Int(nullable: false),
                        ProcessID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.sysBpmsProcess", t => t.ProcessID)
                .Index(t => t.ProcessID);
            
            CreateTable(
                "dbo.sysBpmsEvent",
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
                .ForeignKey("dbo.sysBpmsElement", t => t.ElementID)
                .ForeignKey("dbo.sysBpmsMessageType", t => t.MessageTypeID)
                .ForeignKey("dbo.sysBpmsProcess", t => t.ProcessID)
                .Index(t => t.ElementID)
                .Index(t => t.ProcessID)
                .Index(t => t.MessageTypeID);
            
            CreateTable(
                "dbo.sysBpmsMessageType",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        ParamsXML = c.String(),
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
                        FormattedNumber = c.String(),
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
                        FileExtention = c.String(),
                        CaptionOf = c.String(),
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
                .ForeignKey("dbo.sysBpmsDocumentFolder", t => t.DocumentFolderID)
                .Index(t => t.DocumentFolderID);
            
            CreateTable(
                "dbo.sysBpmsDocumentFolder",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        NameOf = c.String(),
                        DisplayName = c.String(),
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
                        DisplayName = c.String(),
                        Name = c.String(),
                        DesignXML = c.String(),
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
                        Name = c.String(),
                        DataSource = c.String(),
                        InitialCatalog = c.String(),
                        UserID = c.String(),
                        Password = c.String(),
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
                        Value = c.String(),
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
                        DependentPropertyName = c.String(),
                        ToVariableID = c.Guid(),
                        ToPropertyName = c.String(),
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
                        Description = c.String(),
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
                .ForeignKey("dbo.sysBpmsElement", t => t.ElementID)
                .ForeignKey("dbo.sysBpmsProcess", t => t.ProcessID)
                .Index(t => t.ElementID)
                .Index(t => t.ProcessID);
            
            CreateTable(
                "dbo.sysBpmsStep",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        TaskID = c.Guid(nullable: false),
                        Position = c.Int(nullable: false),
                        Name = c.String(),
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
                        Username = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Email = c.String(),
                        Tel = c.String(),
                        Mobile = c.String(),
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
                        Name = c.String(),
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
                        ElementID = c.String(maxLength: 128),
                        DefaultSequenceFlowID = c.Guid(),
                        TypeLU = c.Int(),
                        TraceToStart = c.String(),
                        ProcessID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.sysBpmsElement", t => t.ElementID)
                .ForeignKey("dbo.sysBpmsProcess", t => t.ProcessID)
                .ForeignKey("dbo.sysBpmsSequenceFlow", t => t.DefaultSequenceFlowID)
                .Index(t => t.ElementID)
                .Index(t => t.DefaultSequenceFlowID)
                .Index(t => t.ProcessID);
            
            CreateTable(
                "dbo.sysBpmsCondition",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        GatewayID = c.Guid(nullable: false),
                        SequenceFlowID = c.Guid(),
                        Code = c.String(),
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
                        ElementID = c.String(maxLength: 128),
                        ProcessID = c.Guid(nullable: false),
                        Name = c.String(),
                        SourceElementID = c.String(),
                        TargetElementID = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.sysBpmsElement", t => t.ElementID)
                .ForeignKey("dbo.sysBpmsProcess", t => t.ProcessID)
                .Index(t => t.ElementID)
                .Index(t => t.ProcessID);
            
            CreateTable(
                "dbo.sysBpmsLane",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        ElementID = c.String(maxLength: 128),
                        ProcessID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.sysBpmsElement", t => t.ElementID)
                .ForeignKey("dbo.sysBpmsProcess", t => t.ProcessID)
                .Index(t => t.ElementID)
                .Index(t => t.ProcessID);
            
            CreateTable(
                "dbo.sysBpmsProcessGroup",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        ProcessGroupID = c.Guid(),
                        Name = c.String(),
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
                        Name = c.String(),
                        Label = c.String(),
                        DefaultValue = c.String(),
                        Value = c.String(),
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
                        SMTP = c.String(),
                        Port = c.String(),
                        MailUserName = c.String(),
                        MailPassword = c.String(),
                        Email = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.sysBpmsLURow",
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
                .ForeignKey("dbo.sysBpmsLUTable", t => t.LUTableID)
                .Index(t => t.LUTableID);
            
            CreateTable(
                "dbo.sysBpmsLUTable",
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
            DropForeignKey("dbo.sysBpmsLane", "ElementID", "dbo.sysBpmsElement");
            DropForeignKey("dbo.sysBpmsGateway", "DefaultSequenceFlowID", "dbo.sysBpmsSequenceFlow");
            DropForeignKey("dbo.sysBpmsGateway", "ProcessID", "dbo.sysBpmsProcess");
            DropForeignKey("dbo.sysBpmsGateway", "ElementID", "dbo.sysBpmsElement");
            DropForeignKey("dbo.sysBpmsCondition", "SequenceFlowID", "dbo.sysBpmsSequenceFlow");
            DropForeignKey("dbo.sysBpmsSequenceFlow", "ProcessID", "dbo.sysBpmsProcess");
            DropForeignKey("dbo.sysBpmsSequenceFlow", "ElementID", "dbo.sysBpmsElement");
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
            DropForeignKey("dbo.sysBpmsTask", "ElementID", "dbo.sysBpmsElement");
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
            DropForeignKey("dbo.sysBpmsEvent", "ElementID", "dbo.sysBpmsElement");
            DropForeignKey("dbo.sysBpmsDynamicForm", "ApplicationPageID", "dbo.sysBpmsApplicationPage");
            DropIndex("dbo.sysBpmsLURow", new[] { "LUTableID" });
            DropIndex("dbo.sysBpmsProcessGroup", new[] { "ProcessGroupID" });
            DropIndex("dbo.sysBpmsLane", new[] { "ProcessID" });
            DropIndex("dbo.sysBpmsLane", new[] { "ElementID" });
            DropIndex("dbo.sysBpmsSequenceFlow", new[] { "ProcessID" });
            DropIndex("dbo.sysBpmsSequenceFlow", new[] { "ElementID" });
            DropIndex("dbo.sysBpmsCondition", new[] { "SequenceFlowID" });
            DropIndex("dbo.sysBpmsCondition", new[] { "GatewayID" });
            DropIndex("dbo.sysBpmsGateway", new[] { "ProcessID" });
            DropIndex("dbo.sysBpmsGateway", new[] { "DefaultSequenceFlowID" });
            DropIndex("dbo.sysBpmsGateway", new[] { "ElementID" });
            DropIndex("dbo.sysBpmsDepartment", new[] { "DepartmentID" });
            DropIndex("dbo.sysBpmsDepartmentMember", new[] { "UserID" });
            DropIndex("dbo.sysBpmsDepartmentMember", new[] { "DepartmentID" });
            DropIndex("dbo.sysBpmsStep", new[] { "DynamicFormID" });
            DropIndex("dbo.sysBpmsStep", new[] { "TaskID" });
            DropIndex("dbo.sysBpmsTask", new[] { "ProcessID" });
            DropIndex("dbo.sysBpmsTask", new[] { "ElementID" });
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
            DropIndex("dbo.sysBpmsEvent", new[] { "ProcessID" });
            DropIndex("dbo.sysBpmsEvent", new[] { "ElementID" });
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
