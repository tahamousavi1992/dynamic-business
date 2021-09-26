namespace DynamicBusiness.BPMS.BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class version_0 : DbMigration
    {
        public override void Up()
        {
            Sql(@"CREATE FUNCTION [sysBpmsSplit]
(    
    @RowData NVARCHAR(MAX),
    @Delimeter NVARCHAR(MAX)
)
RETURNS @RtnValue TABLE 
(
    ID INT IDENTITY(1,1),
    Data NVARCHAR(MAX)
) 
AS
BEGIN 
    DECLARE @Iterator INT
    SET @Iterator = 1

    DECLARE @FoundIndex INT
    SET @FoundIndex = CHARINDEX(@Delimeter,@RowData)

    WHILE (@FoundIndex>0)
    BEGIN
        INSERT INTO @RtnValue (data)
        SELECT 
            Data = LTRIM(RTRIM(SUBSTRING(@RowData, 1, @FoundIndex - 1)))

        SET @RowData = SUBSTRING(@RowData,
                @FoundIndex + DATALENGTH(@Delimeter) / 2,
                LEN(@RowData))

        SET @Iterator = @Iterator + 1
        SET @FoundIndex = CHARINDEX(@Delimeter, @RowData)
    END
    
    INSERT INTO @RtnValue (Data)
    SELECT Data = LTRIM(RTRIM(@RowData))

    RETURN
END

GO
");
            CreateTable(
                "dbo.sysBpmsAPIAccess",
                c => new
                {
                    ID = c.Guid(nullable: false, defaultValueSql: "NewId()"),
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
                    ID = c.Guid(nullable: false, defaultValueSql: "NewId()"),
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
                    ID = c.Guid(nullable: false, defaultValueSql: "NewId()"),
                    GroupLU = c.Int(nullable: false),
                    Description = c.String(),
                    ShowInMenu = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.sysBpmsDynamicForm",
                c => new
                {
                    ID = c.Guid(nullable: false, defaultValueSql: "NewId()"),
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
                    ID = c.Guid(nullable: false, defaultValueSql: "NewId()"),
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
                    ID = c.Guid(nullable: false, defaultValueSql: "NewId()"),
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
                    ID = c.Guid(nullable: false, defaultValueSql: "NewId()"),
                    Name = c.String(nullable: false, maxLength: 500),
                    ParamsXML = c.String(nullable: false),
                    IsActive = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.sysBpmsThreadEvent",
                c => new
                {
                    ID = c.Guid(nullable: false, defaultValueSql: "NewId()"),
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
                    ID = c.Guid(nullable: false, defaultValueSql: "NewId()"),
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
                    GUID = c.Guid(nullable: false, defaultValueSql: "NewId()"),
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
                    ID = c.Guid(nullable: false, defaultValueSql: "NewId()"),
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
                    ID = c.Guid(nullable: false, defaultValueSql: "NewId()"),
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
                    ID = c.Guid(nullable: false, defaultValueSql: "NewId()"),
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
                    ID = c.Guid(nullable: false, defaultValueSql: "NewId()"),
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
                    ID = c.Guid(nullable: false, defaultValueSql: "NewId()"),
                    Name = c.String(nullable: false, maxLength: 500),
                    DataSource = c.String(nullable: false, maxLength: 500),
                    InitialCatalog = c.String(nullable: false, maxLength: 500),
                    UserID = c.String(nullable: false, maxLength: 500),
                    Password = c.String(nullable: false, maxLength: 500),
                    IntegratedSecurity = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.sysBpmsVariableDependency",
                c => new
                {
                    ID = c.Guid(nullable: false, defaultValueSql: "NewId()"),
                    DependentVariableID = c.Guid(nullable: false),
                    DependentPropertyName = c.String(nullable: false, maxLength: 250),
                    ToVariableID = c.Guid(),
                    ToPropertyName = c.String(nullable: false, maxLength: 250),
                    Description = c.String(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.sysBpmsVariable", t => t.DependentVariableID)
                .ForeignKey("dbo.sysBpmsVariable", t => t.ToVariableID)
                .Index(t => t.DependentVariableID)
                .Index(t => t.ToVariableID);

            CreateTable(
                "dbo.sysBpmsThreadVariable",
                c => new
                {
                    ID = c.Guid(nullable: false, defaultValueSql: "NewId()"),
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
                "dbo.sysBpmsThreadTask",
                c => new
                {
                    ID = c.Guid(nullable: false, defaultValueSql: "NewId()"),
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
                    ID = c.Guid(nullable: false, defaultValueSql: "NewId()"),
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
                    ID = c.Guid(nullable: false, defaultValueSql: "NewId()"),
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
                    ID = c.Guid(nullable: false, defaultValueSql: "NewId()"),
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
                    ID = c.Guid(nullable: false, defaultValueSql: "NewId()"),
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
                    ID = c.Guid(nullable: false, defaultValueSql: "NewId()"),
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
                    ID = c.Guid(nullable: false, defaultValueSql: "NewId()"),
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
                    ID = c.Guid(nullable: false, defaultValueSql: "NewId()"),
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
                    ID = c.Guid(nullable: false, defaultValueSql: "NewId()"),
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
                    ID = c.Guid(nullable: false, defaultValueSql: "NewId()"),
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
                    ID = c.Guid(nullable: false, defaultValueSql: "NewId()"),
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
                    ID = c.Guid(nullable: false, defaultValueSql: "NewId()"),
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
                    ID = c.Guid(nullable: false, defaultValueSql: "NewId()"),
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
                    ID = c.Guid(nullable: false, defaultValueSql: "NewId()"),
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
                    ID = c.Guid(nullable: false, defaultValueSql: "NewId()"),
                    NameOf = c.String(nullable: false, maxLength: 500),
                    Alias = c.String(nullable: false, maxLength: 500),
                    IsSystemic = c.Boolean(nullable: false),
                    IsActive = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.ID);

            //Seed
            Sql(@"INSERT [sysBpmsConfiguration] ([ID], [Name], [Label], [DefaultValue], [Value], [LastUpdateOn]) VALUES (N'bad77f76-c7b3-4fb1-b668-05cc1e27a5ef', N'ProcessFormatSerlialNumber', N'Workflow Serlial number format', N'yyyy/mm/dd_#####', N'', NULL)
GO
INSERT [sysBpmsConfiguration] ([ID], [Name], [Label], [DefaultValue], [Value], [LastUpdateOn]) VALUES (N'eb1fc836-518a-41d4-a6ea-0a6141b9da81', N'LastSqlUpdatedVersion', N'Last sql version executed ', N'00.00.00', N' ', NULL)
GO
INSERT [sysBpmsConfiguration] ([ID], [Name], [Label], [DefaultValue], [Value], [LastUpdateOn]) VALUES (N'e473816a-de0c-4128-951c-32e0cd2e850b', N'NoSkinPath', N'No Skin Path', N'SkinSrc=desktopmodules/MVC/DynamicBusiness.Bpms/noskin', N'SkinSrc=desktopmodules/MVC/DynamicBusiness.Bpms/noskin', NULL)
GO
INSERT [sysBpmsConfiguration] ([ID], [Name], [Label], [DefaultValue], [Value], [LastUpdateOn]) VALUES (N'1e3dca37-e40d-41cb-9ff6-66941aa5309b', N'LoadUserPanelJquery', N'Load User Panel Jquery', N'true', N'true', NULL)
GO
INSERT [sysBpmsConfiguration] ([ID], [Name], [Label], [DefaultValue], [Value], [LastUpdateOn]) VALUES (N'c9bb66bb-7eb6-43a8-8eb7-6ca983de4221', N'ThreadFormatSerlialNumber', N'Request Serlial number format', N'yyyy/mm/dd_#####', N'', NULL)
GO
INSERT [sysBpmsConfiguration] ([ID], [Name], [Label], [DefaultValue], [Value], [LastUpdateOn]) VALUES (N'43e3c96c-a1c6-4c0a-9a4f-70d6b2f7d047', N'DefaultReportFontFamily', N'Report Default Font', N'Times New Roman', N'Times New Roman', NULL)
GO
INSERT [sysBpmsConfiguration] ([ID], [Name], [Label], [DefaultValue], [Value], [LastUpdateOn]) VALUES (N'cebe0666-76f4-4a7e-97ec-80634fa6628b', N'ShowUserPanelWithNoSkin', N'Show User Panel With NoSkin', N'true', N'true', NULL)
GO
INSERT [sysBpmsConfiguration] ([ID], [Name], [Label], [DefaultValue], [Value], [LastUpdateOn]) VALUES (N'df7f94b6-1a66-472e-b20f-9e5870a8e74e', N'ThreadStartPointSerlialNumber', N'Thread Start Point Serlial Number', N'1', N'1', NULL)
GO
INSERT [sysBpmsConfiguration] ([ID], [Name], [Label], [DefaultValue], [Value], [LastUpdateOn]) VALUES (N'65b7a7de-0ba8-4dfe-abb0-b936b20d2d2c', N'WebServicePass', N'WebService Password', N' ', N' ', NULL)
GO
INSERT [sysBpmsConfiguration] ([ID], [Name], [Label], [DefaultValue], [Value], [LastUpdateOn]) VALUES (N'fae48efb-b810-4db9-bad0-dd74e4ef69e4', N'AddUserAutomatically', N'Add User Automatically', N'true', N'true', NULL)
GO
INSERT [sysBpmsConfiguration] ([ID], [Name], [Label], [DefaultValue], [Value], [LastUpdateOn]) VALUES (N'fae48efb-b810-4db9-bad0-dd78e4ee69e4', N'ProcessStartPointSerlialNumber', N'Workflow StartPoint Serlial Number', N'1', N'1', NULL)
GO
INSERT [sysBpmsConfiguration] ([ID], [Name], [Label], [DefaultValue], [Value], [LastUpdateOn]) VALUES (N'a0fc45ad-4ed4-468b-8cbe-eccc2f6643b1', N'LoadUserPanelBootstrap', N'Load User Panel Bootstrap', N'true', N'true', NULL)
GO
INSERT [sysBpmsLURow] ([ID], [LUTableID], [NameOf], [CodeOf], [DisplayOrder], [IsSystemic], [IsActive]) VALUES (N'b975200a-2bbb-4aa3-9153-221ed49b9af0', N'4ba48a03-422c-43cb-a5e0-8df77f869edf', N'Draft', N'1', 1, 1, 1)
GO
INSERT [sysBpmsLURow] ([ID], [LUTableID], [NameOf], [CodeOf], [DisplayOrder], [IsSystemic], [IsActive]) VALUES (N'64f971f8-0796-4a2c-b362-22e136b49907', N'd26ef8e8-5fe3-41fc-9f77-2126fb38b3f9', N'Requester', N'1', 1, 1, 1)
GO
INSERT [sysBpmsLURow] ([ID], [LUTableID], [NameOf], [CodeOf], [DisplayOrder], [IsSystemic], [IsActive]) VALUES (N'b0a928c2-9592-4487-b89a-44e157cf590f', N'f62945c4-ffcc-47f3-a295-77fc8b7b7cb8', N'User', N'1', 1, 1, 1)
GO
INSERT [sysBpmsLURow] ([ID], [LUTableID], [NameOf], [CodeOf], [DisplayOrder], [IsSystemic], [IsActive]) VALUES (N'579b616b-056b-4898-a21e-54396b05b171', N'4ba48a03-422c-43cb-a5e0-8df77f869edf', N'Published', N'2', 2, 1, 1)
GO
INSERT [sysBpmsLURow] ([ID], [LUTableID], [NameOf], [CodeOf], [DisplayOrder], [IsSystemic], [IsActive]) VALUES (N'aa704084-a7d4-40f1-b1c2-708f7da9e53b', N'4ba48a03-422c-43cb-a5e0-8df77f869edf', N'InActive', N'3', 3, 1, 1)
GO
INSERT [sysBpmsLURow] ([ID], [LUTableID], [NameOf], [CodeOf], [DisplayOrder], [IsSystemic], [IsActive]) VALUES (N'3015e3ec-1c39-4092-9f80-86c1d40cbc40', N'f62945c4-ffcc-47f3-a295-77fc8b7b7cb8', N'Role', N'2', 2, 1, 1)
GO
INSERT [sysBpmsLURow] ([ID], [LUTableID], [NameOf], [CodeOf], [DisplayOrder], [IsSystemic], [IsActive]) VALUES (N'b71f886e-95f7-427f-9728-ad73907af2b8', N'4ba48a03-422c-43cb-a5e0-8df77f869edf', N'Finished', N'4', 4, 1, 1)
GO
INSERT [sysBpmsLURow] ([ID], [LUTableID], [NameOf], [CodeOf], [DisplayOrder], [IsSystemic], [IsActive]) VALUES (N'0336f54c-afc9-44d0-b12e-f62f6581f8ef', N'393529c0-e96c-4cf4-92fd-c83efe7ce0b9', N'General', N'1', 1, 1, 1)
GO
INSERT [sysBpmsLUTable] ([ID], [NameOf], [Alias], [IsSystemic], [IsActive]) VALUES (N'd26ef8e8-5fe3-41fc-9f77-2126fb38b3f9', N'Organization Role', N'DepartmentRoleLU', 0, 1)
GO
INSERT [sysBpmsLUTable] ([ID], [NameOf], [Alias], [IsSystemic], [IsActive]) VALUES (N'f62945c4-ffcc-47f3-a295-77fc8b7b7cb8', N'Owner Access Type', N'LaneOwnerTypeLU', 1, 1)
GO
INSERT [sysBpmsLUTable] ([ID], [NameOf], [Alias], [IsSystemic], [IsActive]) VALUES (N'4ba48a03-422c-43cb-a5e0-8df77f869edf', N'Workflow Status', N'ProcessStatusLU', 1, 1)
GO
INSERT [sysBpmsLUTable] ([ID], [NameOf], [Alias], [IsSystemic], [IsActive]) VALUES (N'393529c0-e96c-4cf4-92fd-c83efe7ce0b9', N'Application Page Group', N'ApplicationPageGroupLU', 0, 1)
GO");
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
            DropForeignKey("dbo.sysBpmsThreadVariable", "VariableID", "dbo.sysBpmsVariable");
            DropForeignKey("dbo.sysBpmsThreadVariable", "ThreadID", "dbo.sysBpmsThread");
            DropForeignKey("dbo.sysBpmsVariable", "ProcessID", "dbo.sysBpmsProcess");
            DropForeignKey("dbo.sysBpmsVariable", "EntityDefID", "dbo.sysBpmsEntityDef");
            DropForeignKey("dbo.sysBpmsVariableDependency", "ToVariableID", "dbo.sysBpmsVariable");
            DropForeignKey("dbo.sysBpmsVariableDependency", "DependentVariableID", "dbo.sysBpmsVariable");
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
            DropIndex("dbo.sysBpmsThreadVariable", new[] { "VariableID" });
            DropIndex("dbo.sysBpmsThreadVariable", new[] { "ThreadID" });
            DropIndex("dbo.sysBpmsVariableDependency", new[] { "ToVariableID" });
            DropIndex("dbo.sysBpmsVariableDependency", new[] { "DependentVariableID" });
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
            DropTable("dbo.sysBpmsThreadVariable");
            DropTable("dbo.sysBpmsVariableDependency");
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
