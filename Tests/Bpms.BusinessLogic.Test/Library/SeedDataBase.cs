using DynamicBusiness.BPMS.BusinessLogic;
using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bpms.BusinessLogic.Test
{
    public class SeedDataBase
    {
        private readonly Db_BPMSEntities db = null;
        public SeedDataBase()
        {
            this.db = new Db_BPMSEntities();
        }

        public void SeedAll()
        {
            this.SeedProcesses();
            this.SeedUsersDepartments();
            this.SeedDocuments();
        }

        public void SeedProcesses()
        {
            //set Process Group
            var pGroup = new sysBpmsProcessGroup() { Description = "", Name = "Test", ID = Guid.NewGuid() };
            this.db.sysBpmsProcessGroups.Add(pGroup);
            //Set processes
            this.db.sysBpmsProcesses.AddRange(new List<sysBpmsProcess>()
            {
               new sysBpmsProcess(){
                   Description = "",
                   Name = " Recruitment Process With Local Variable",
                   ID = new Guid("3E9E69D2-9232-4F07-8B63-08FBDD778F45"),
                   ProcessGroupID = pGroup.ID,
                   CreateDate = DateTime.Now,
                   Number = 1 ,
                   UpdateDate =DateTime.Now,
                   ParentProcessID =null,
                   StatusLU = (int)sysBpmsProcess.Enum_StatusLU.Published,
                   TypeLU =  (int)sysBpmsProcess.e_TypeLU.General,
                   BeginTasks="Activity_1k5enxp",
                   CreatorUsername = "host",
                   WorkflowXML = @"<?xml version=""1.0"" encoding=""utf-16""?>
<process id=""Process_0avyb7c"">
  <startEvent id=""Event_1rghv3g"" name=""start"">
    <outgoing>Flow_0va6l5w</outgoing>
  </startEvent>
  <sequenceFlow id=""Flow_0va6l5w"" sourceRef=""Event_1rghv3g"" targetRef=""Activity_1k5enxp"" />
  <sequenceFlow id=""Flow_0oo1rbi"" sourceRef=""Activity_1k5enxp"" targetRef=""Activity_15hd835"" />
  <userTask id=""Activity_1k5enxp"" name=""Add Request"">
    <incoming>Flow_0va6l5w</incoming>
    <outgoing>Flow_0oo1rbi</outgoing>
  </userTask>
  <userTask id=""Activity_15hd835"" name=""Chech Request &amp; Set Appointment"">
    <incoming>Flow_0oo1rbi</incoming>
    <incoming>Flow_177jung</incoming>
    <outgoing>Flow_1nhucgt</outgoing>
  </userTask>
  <exclusiveGateway id=""Gateway_1szzta7"">
    <incoming>Flow_1nhucgt</incoming>
    <outgoing>Flow_1k2y640</outgoing>
    <outgoing>Flow_1enbk0l</outgoing>
  </exclusiveGateway>
  <sequenceFlow id=""Flow_1nhucgt"" sourceRef=""Activity_15hd835"" targetRef=""Gateway_1szzta7"" />
  <sequenceFlow id=""Flow_1k2y640"" name=""Yes"" sourceRef=""Gateway_1szzta7"" targetRef=""Event_176jcd8"" />
  <endEvent id=""Event_1p1p5sg"" name=""finished"">
    <incoming>Flow_1enbk0l</incoming>
    <incoming>Flow_0mixqiq</incoming>
  </endEvent>
  <sequenceFlow id=""Flow_1enbk0l"" name=""No"" sourceRef=""Gateway_1szzta7"" targetRef=""Event_1p1p5sg"" />
  <intermediateThrowEvent id=""Event_176jcd8"" name=""Email"">
    <incoming>Flow_1k2y640</incoming>
    <outgoing>Flow_0wblxyd</outgoing>
    <messageEventDefinition id=""MessageEventDefinition_1c01dze"" />
  </intermediateThrowEvent>
  <sequenceFlow id=""Flow_0wblxyd"" sourceRef=""Event_176jcd8"" targetRef=""Activity_1maluwj"" />
  <exclusiveGateway id=""Gateway_1gt00dk"">
    <incoming>Flow_1g1gxzb</incoming>
    <outgoing>Flow_0mixqiq</outgoing>
    <outgoing>Flow_177jung</outgoing>
  </exclusiveGateway>
  <sequenceFlow id=""Flow_1g1gxzb"" sourceRef=""Activity_1maluwj"" targetRef=""Gateway_1gt00dk"" />
  <sequenceFlow id=""Flow_0mixqiq"" name=""Yes"" sourceRef=""Gateway_1gt00dk"" targetRef=""Event_1p1p5sg"" />
  <userTask id=""Activity_1maluwj"" name=""Aware Requester"">
    <incoming>Flow_0wblxyd</incoming>
    <outgoing>Flow_1g1gxzb</outgoing>
  </userTask>
  <sequenceFlow id=""Flow_177jung"" name=""No"" sourceRef=""Gateway_1gt00dk"" targetRef=""Activity_15hd835"" />
</process>",
                   DiagramXML ="",
                   SourceCode ="",
                   ParallelCountPerUser = null,
                   FormattedNumber ="",
                   PublishDate =DateTime.Now,
               },
            });

            //Set Forms
            this.db.sysBpmsDynamicForms.AddRange(new List<sysBpmsDynamicForm>()
            {
                new sysBpmsDynamicForm()
                {
                    ID = new Guid("4670331D-D887-486C-935C-5B294CB6238C"),
                    ProcessId = new Guid("3E9E69D2-9232-4F07-8B63-08FBDD778F45"),
                    ApplicationPageID = null,
                    Name = "Aware Requester",
                    Version = 0,
                    ShowInOverview = true,
                    CreatedBy = "bpms_manager",
                    CreatedDate = DateTime.Now,
                    UpdatedBy = "bpms_manager",
                    UpdatedDate = DateTime.Now,
                    ConfigXML = @"﻿<?xml version=""1.0"" encoding=""utf-16""?>
<DynamicFormConfigXmlModel xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
  <IsEncrypted>false</IsEncrypted>
</DynamicFormConfigXmlModel>",
                    DesignJson = @"{""id"":""content"",""type"":""CONTENT"",""cssClass"":"""",""label"":null,""rows"":[{""id"":""ROW118462"",""type"":""ROW"",""cssClass"":""  row  "",""label"":null,""expressionVisibilityCode"":"""",""columns"":[{""id"":""COLUMN418724"",""type"":""COLUMN"",""cssClass"":""col-sm-12  "",""label"":null,""expressionVisibilityCode"":"""",""children"":[{""id"":""FORM356066"",""type"":""FORM"",""cssClass"":"""",""label"":""New Form"",""expressionVisibilityCode"":"""",""readOnly"":true,""formId"":""760063c7-ec5d-4994-ab71-61c271b86dd7""}]}],""isFooter"":false},{""id"":""ROW432600"",""type"":""ROW"",""cssClass"":""  row  "",""label"":null,""expressionVisibilityCode"":"""",""columns"":[{""id"":""COLUMN656457"",""type"":""COLUMN"",""cssClass"":"" col-sm-4 "",""label"":null,""expressionVisibilityCode"":"""",""children"":[{""id"":""DATEPICKER778135"",""type"":""DATEPICKER"",""cssClass"":""form-control"",""label"":""Appointment Date"",""expressionVisibilityCode"":"""",""helpMessageText"":null,""isRequired"":false,""fillBinding"":"" varAppointmentDate"",""mapBinding"":"" varAppointmentDate"",""events"":[],""readOnly"":""true"",""validationGroup"":"""",""parameter"":"""",""showtype"":""datetime"",""dateformat"":""yyyy/mm/dd""}]},{""id"":""COLUMN857461"",""type"":""COLUMN"",""cssClass"":"" col-sm-4 "",""label"":null,""expressionVisibilityCode"":"""",""children"":[{""id"":""DROPDOWNLIST535805"",""type"":""DROPDOWNLIST"",""cssClass"":""form-control required-field "",""label"":""Status"",""expressionVisibilityCode"":"""",""helpMessageText"":null,""isRequired"":true,""fontIconCssClass"":""fa fa-check"",""options"":[{""label"":""Approve"",""value"":""1"",""selected"":true},{""label"":""Reject"",""value"":""2"",""selected"":false}],""mapBinding"":""varUserStatus"",""fillBinding"":""varUserStatus"",""fillListBinding"":"""",""fillKey"":"""",""fillText"":"""",""events"":[],""parameter"":"""",""optionalCaption"":"""",""readOnly"":false,""validationGroup"":""""}]},{""id"":""COLUMN309135"",""type"":""COLUMN"",""cssClass"":"" col-sm-4 "",""label"":null,""expressionVisibilityCode"":"""",""children"":[]}],""isFooter"":false}]}",
                },
                new sysBpmsDynamicForm()
                {
                    ID = new Guid("760063C7-EC5D-4994-AB71-61C271B86DD7"),
                    ProcessId = new Guid("3E9E69D2-9232-4F07-8B63-08FBDD778F45"),
                    ApplicationPageID = null,
                    Name = "Add Request",
                    Version = 0,
                    ShowInOverview = true,
                    CreatedBy = "bpms_manager",
                    CreatedDate = DateTime.Now,
                    UpdatedBy = "bpms_manager",
                    UpdatedDate = DateTime.Now,
                    ConfigXML = @"﻿<?xml version=""1.0"" encoding=""utf-16""?>
<DynamicFormConfigXmlModel xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
  <IsEncrypted>false</IsEncrypted>
</DynamicFormConfigXmlModel>",
                    DesignJson = @"{""id"":""content"",""type"":""CONTENT"",""cssClass"":"""",""label"":null,""rows"":[{""id"":""ROW304543"",""type"":""ROW"",""cssClass"":""  row  "",""label"":null,""expressionVisibilityCode"":"""",""columns"":[{""id"":""COLUMN948739"",""type"":""COLUMN"",""cssClass"":"" col-sm-4 "",""label"":null,""expressionVisibilityCode"":"""",""children"":[{""id"":""TEXTBOX634031"",""type"":""TEXTBOX"",""cssClass"":""form-control required-field "",""label"":""First Name"",""expressionVisibilityCode"":"""",""subtype"":""text"",""helpMessageText"":null,""isRequired"":true,""fontIconCssClass"":""fa fa-hashtag"",""fillBinding"":""varFirstName"",""mapBinding"":""varFirstName"",""readOnly"":false,""pattern"":"""",""events"":[],""isMultiline"":""false"",""validationGroup"":"""",""parameter"":""""}]},{""id"":""COLUMN326575"",""type"":""COLUMN"",""cssClass"":"" col-sm-4 "",""label"":null,""expressionVisibilityCode"":"""",""children"":[{""id"":""TEXTBOX595851"",""type"":""TEXTBOX"",""cssClass"":""form-control required-field "",""label"":""Last Name"",""expressionVisibilityCode"":"""",""subtype"":""text"",""helpMessageText"":null,""isRequired"":true,""fontIconCssClass"":""fa fa-hashtag"",""fillBinding"":""varLastName"",""mapBinding"":""varLastName"",""readOnly"":false,""pattern"":"""",""events"":[],""isMultiline"":""false"",""validationGroup"":"""",""parameter"":""""}]},{""id"":""COLUMN214721"",""type"":""COLUMN"",""cssClass"":"" col-sm-4 "",""label"":null,""expressionVisibilityCode"":"""",""children"":[{""id"":""DATEPICKER674934"",""type"":""DATEPICKER"",""cssClass"":""form-control required-field "",""label"":""Birth Date"",""expressionVisibilityCode"":"""",""helpMessageText"":null,""isRequired"":true,""fillBinding"":""varBirthDate"",""mapBinding"":""varBirthDate"",""events"":[],""readOnly"":""false"",""validationGroup"":"""",""parameter"":"""",""showtype"":""date"",""dateformat"":""yyyy/mm/dd""}]}],""isFooter"":false},{""id"":""ROW852986"",""type"":""ROW"",""cssClass"":""  row  "",""label"":null,""expressionVisibilityCode"":"""",""columns"":[{""id"":""COLUMN790052"",""type"":""COLUMN"",""cssClass"":"" col-sm-4 "",""label"":null,""expressionVisibilityCode"":"""",""children"":[{""id"":""TEXTBOX796879"",""type"":""TEXTBOX"",""cssClass"":""form-control required-field "",""label"":""Expected Salary"",""expressionVisibilityCode"":"""",""subtype"":""text"",""helpMessageText"":null,""isRequired"":true,""fontIconCssClass"":""fa fa-hashtag"",""fillBinding"":""varExpectedSalary"",""mapBinding"":""varExpectedSalary"",""readOnly"":false,""pattern"":"""",""events"":[],""isMultiline"":""false"",""validationGroup"":"""",""parameter"":""""}]},{""id"":""COLUMN658050"",""type"":""COLUMN"",""cssClass"":"" col-sm-4 "",""label"":null,""expressionVisibilityCode"":"""",""children"":[{""id"":""TEXTBOX103744"",""type"":""TEXTBOX"",""cssClass"":""form-control required-field "",""label"":""Email"",""expressionVisibilityCode"":"""",""subtype"":""email"",""helpMessageText"":null,""isRequired"":true,""fontIconCssClass"":""fa fa-hashtag"",""fillBinding"":""varEmail"",""mapBinding"":""varEmail"",""readOnly"":false,""pattern"":"""",""events"":[],""isMultiline"":""false"",""validationGroup"":"""",""parameter"":""""}]},{""id"":""COLUMN756124"",""type"":""COLUMN"",""cssClass"":"" col-sm-4 "",""label"":null,""expressionVisibilityCode"":"""",""children"":[]}],""isFooter"":false},{""id"":""ROW791324"",""type"":""ROW"",""cssClass"":""  row  "",""label"":null,""expressionVisibilityCode"":"""",""columns"":[{""id"":""COLUMN175540"",""type"":""COLUMN"",""cssClass"":""col-sm-12  "",""label"":null,""expressionVisibilityCode"":"""",""children"":[{""id"":""FILEUPLOAD78162"",""type"":""FILEUPLOAD"",""cssClass"":""form-control"",""label"":""new File Uploader"",""expressionVisibilityCode"":"""",""helpMessageText"":null,""placeholderText"":""Select File"",""isRequired"":false,""fontIconCssClass"":""fa fa-upload"",""documentDefId"":""c9ed3c81-116f-eb11-924f-54e1ade02b17"",""entityVariableId"":"""",""multiple"":false,""events"":[],""deleteClass"":""btn btn-sm btn-clean btn-icon btn-icon-md"",""downloadClass"":""btn btn-sm btn-clean btn-icon btn-icon-md"",""deleteCaption"":""<span class=\""svg-icon svg-icon-md\""><i class=\""fad fa-times\""></i></span>"",""downloadCaption"":""<span class=\""svg-icon svg-icon-md\""><i class=\""fad fa-download\""></i></span>"",""validationGroup"":"""",""documentFolderId"":""c9ed3c81-116f-eb11-924f-54e1ade02b17""}]}],""isFooter"":false}]}",
                },
                new sysBpmsDynamicForm()
                {
                    ID = new Guid("181D14E6-F44F-4892-9277-E76D196EDA6B"),
                    ProcessId = new Guid("3E9E69D2-9232-4F07-8B63-08FBDD778F45"),
                    ApplicationPageID = null,
                    Name = "Chech Request & Set Appointment",
                    Version = 0,
                    ShowInOverview = true,
                    CreatedBy = "bpms_manager",
                    CreatedDate = DateTime.Now,
                    UpdatedBy = "bpms_manager",
                    UpdatedDate = DateTime.Now,
                    ConfigXML = @"﻿﻿<?xml version=""1.0"" encoding=""utf-16""?>
<DynamicFormConfigXmlModel xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
  <IsEncrypted>false</IsEncrypted>
</DynamicFormConfigXmlModel>",
                    DesignJson = @"{""id"":""content"",""type"":""CONTENT"",""cssClass"":"""",""label"":null,""rows"":[{""id"":""ROW736868"",""type"":""ROW"",""cssClass"":""  row  "",""label"":null,""expressionVisibilityCode"":"""",""columns"":[{""id"":""COLUMN336405"",""type"":""COLUMN"",""cssClass"":""col-sm-12  "",""label"":null,""expressionVisibilityCode"":"""",""children"":[{""id"":""FORM260960"",""type"":""FORM"",""cssClass"":"""",""label"":""New Form"",""expressionVisibilityCode"":"""",""readOnly"":true,""formId"":""760063c7-ec5d-4994-ab71-61c271b86dd7""}]}],""isFooter"":false},{""id"":""ROW718089"",""type"":""ROW"",""cssClass"":""  row  "",""label"":null,""expressionVisibilityCode"":"""",""columns"":[{""id"":""COLUMN79173"",""type"":""COLUMN"",""cssClass"":"" col-sm-4 "",""label"":null,""expressionVisibilityCode"":"""",""children"":[{""id"":""DATEPICKER608684"",""type"":""DATEPICKER"",""cssClass"":""form-control required-field "",""label"":"" Appointment Date"",""expressionVisibilityCode"":"""",""helpMessageText"":null,""isRequired"":true,""fillBinding"":"" varAppointmentDate"",""mapBinding"":"" varAppointmentDate"",""events"":[],""readOnly"":""false"",""validationGroup"":"""",""parameter"":"""",""showtype"":""datetime"",""dateformat"":""yyyy/mm/dd""}]},{""id"":""COLUMN188266"",""type"":""COLUMN"",""cssClass"":"" col-sm-4 "",""label"":null,""expressionVisibilityCode"":"""",""children"":[{""id"":""DROPDOWNLIST908269"",""type"":""DROPDOWNLIST"",""cssClass"":""form-control required-field "",""label"":""Status"",""expressionVisibilityCode"":"""",""helpMessageText"":null,""isRequired"":true,""fontIconCssClass"":""fa fa-check"",""options"":[{""label"":""Approve"",""value"":""1"",""selected"":false},{""label"":""Reject"",""value"":""2"",""selected"":false}],""mapBinding"":""varStatus"",""fillBinding"":""varStatus"",""fillListBinding"":"""",""fillKey"":"""",""fillText"":"""",""events"":[],""parameter"":"""",""optionalCaption"":"""",""readOnly"":false,""validationGroup"":""""}]},{""id"":""COLUMN682808"",""type"":""COLUMN"",""cssClass"":"" col-sm-4 "",""label"":null,""expressionVisibilityCode"":"""",""children"":[]}],""isFooter"":false}]}",
                }
            });

            //Set Variables
            this.db.sysBpmsVariables.AddRange(new List<sysBpmsVariable>()
            {
                new sysBpmsVariable(){
                    ID = new Guid("39FAB6B5-9B9D-4DBC-A985-25D5A7F4624E"),
                    ProcessID = new Guid("3E9E69D2-9232-4F07-8B63-08FBDD778F45"),
                    Name = "varUserStatus",  VarTypeLU = 2
                },
                 new sysBpmsVariable(){
                    ID = new Guid("9F1EE658-560A-45C9-88D0-472B270F5CE8"),
                    ProcessID = new Guid("3E9E69D2-9232-4F07-8B63-08FBDD778F45"),
                    Name = "varFirstName",  VarTypeLU = 1
                },
                 new sysBpmsVariable(){
                    ID = new Guid("0E3DB221-764A-4A6E-8149-5ED6C8F729A7"),
                    ProcessID = new Guid("3E9E69D2-9232-4F07-8B63-08FBDD778F45"),
                    Name = "varStatus",  VarTypeLU = 2
                },
                 new sysBpmsVariable(){
                    ID = new Guid("4E9E2DF5-A1F2-4A67-9DF1-612C59626EEF"),
                    ProcessID = new Guid("3E9E69D2-9232-4F07-8B63-08FBDD778F45"),
                    Name = "varEmail",  VarTypeLU = 1
                },
                 new sysBpmsVariable(){
                    ID = new Guid("1228A052-2B96-4C38-849F-94722185215A"),
                    ProcessID = new Guid("3E9E69D2-9232-4F07-8B63-08FBDD778F45"),
                    Name = "varExpectedSalary",  VarTypeLU = 3
                },
                 new sysBpmsVariable(){
                    ID = new Guid("3489B803-A6ED-4795-8588-C553C6F6F382"),
                    ProcessID = new Guid("3E9E69D2-9232-4F07-8B63-08FBDD778F45"),
                    Name = "varBirthDate",  VarTypeLU = 6
                },
                 new sysBpmsVariable(){
                    ID = new Guid("D2D2B10E-B714-4DBA-B768-EAEC3FF259E0"),
                    ProcessID = new Guid("3E9E69D2-9232-4F07-8B63-08FBDD778F45"),
                    Name = "varLastName",  VarTypeLU = 1
                },
                 new sysBpmsVariable(){
                    ID = new Guid("65319B3E-2C5A-4FA8-B357-EDD6CF03F474"),
                    ProcessID = new Guid("3E9E69D2-9232-4F07-8B63-08FBDD778F45"),
                    Name = " varAppointmentDate",  VarTypeLU = 6
                },
            });

            //Set Elements
            this.db.sysBpmsElements.AddRange(new List<sysBpmsElement>()
            {
                new sysBpmsElement(){
                    ID = "Activity_15hd835",
                    ProcessID = new Guid("3E9E69D2-9232-4F07-8B63-08FBDD778F45"),
                    Name = "Chech Request & Set Appointment",  TypeLU = 1
                },
                new sysBpmsElement(){
                    ID = "Activity_1k5enxp",
                    ProcessID = new Guid("3E9E69D2-9232-4F07-8B63-08FBDD778F45"),
                    Name = "Add Request",  TypeLU = 1
                },
                new sysBpmsElement(){
                    ID = "Activity_1maluwj",
                    ProcessID = new Guid("3E9E69D2-9232-4F07-8B63-08FBDD778F45"),
                    Name = "Aware Requester",  TypeLU = 1
                },
                new sysBpmsElement(){
                    ID = "Event_176jcd8",
                    ProcessID = new Guid("3E9E69D2-9232-4F07-8B63-08FBDD778F45"),
                    Name = "Email",  TypeLU = 3
                },
                new sysBpmsElement(){
                    ID = "Event_1p1p5sg",
                    ProcessID = new Guid("3E9E69D2-9232-4F07-8B63-08FBDD778F45"),
                    Name = "finished",  TypeLU = 3
                },
                new sysBpmsElement(){
                    ID = "Event_1rghv3g",
                    ProcessID = new Guid("3E9E69D2-9232-4F07-8B63-08FBDD778F45"),
                    Name = "start",  TypeLU = 3
                },
                new sysBpmsElement(){
                    ID = "Flow_0mixqiq",
                    ProcessID = new Guid("3E9E69D2-9232-4F07-8B63-08FBDD778F45"),
                    Name = "Yes",  TypeLU = 5
                },
                new sysBpmsElement(){
                    ID = "Flow_0oo1rbi",
                    ProcessID = new Guid("3E9E69D2-9232-4F07-8B63-08FBDD778F45"),
                    Name = "",  TypeLU = 5
                },
                new sysBpmsElement(){
                    ID = "Flow_0va6l5w",
                    ProcessID = new Guid("3E9E69D2-9232-4F07-8B63-08FBDD778F45"),
                    Name = "",  TypeLU = 5
                },
                new sysBpmsElement(){
                    ID = "Flow_0wblxyd",
                    ProcessID = new Guid("3E9E69D2-9232-4F07-8B63-08FBDD778F45"),
                    Name = "",  TypeLU = 5
                },
                new sysBpmsElement(){
                    ID = "Flow_177jung",
                    ProcessID = new Guid("3E9E69D2-9232-4F07-8B63-08FBDD778F45"),
                    Name = "No",  TypeLU = 5
                },
                new sysBpmsElement(){
                    ID = "Flow_1enbk0l",
                    ProcessID = new Guid("3E9E69D2-9232-4F07-8B63-08FBDD778F45"),
                    Name = "No",  TypeLU = 5
                },
                new sysBpmsElement(){
                    ID = "Flow_1g1gxzb",
                    ProcessID = new Guid("3E9E69D2-9232-4F07-8B63-08FBDD778F45"),
                    Name = "",  TypeLU = 5
                },
                new sysBpmsElement(){
                    ID = "Flow_1k2y640",
                    ProcessID = new Guid("3E9E69D2-9232-4F07-8B63-08FBDD778F45"),
                    Name = "Yes",  TypeLU = 5
                },
                new sysBpmsElement(){
                    ID = "Flow_1nhucgt",
                    ProcessID = new Guid("3E9E69D2-9232-4F07-8B63-08FBDD778F45"),
                    Name = "",  TypeLU = 5
                },
                new sysBpmsElement(){
                    ID = "Gateway_1gt00dk",
                    ProcessID = new Guid("3E9E69D2-9232-4F07-8B63-08FBDD778F45"),
                    Name = "",  TypeLU = 2
                },
                new sysBpmsElement(){
                    ID = "Gateway_1szzta7",
                    ProcessID = new Guid("3E9E69D2-9232-4F07-8B63-08FBDD778F45"),
                    Name = "",  TypeLU = 2
                },
            });

            //Set Tasks
            this.db.sysBpmsTasks.AddRange(new List<sysBpmsTask>()
            {
                new sysBpmsTask()
                {
                    ID = new Guid("1A7BDA0B-1931-455D-A964-03A76708DB0F"),
                    ProcessID = new Guid("3E9E69D2-9232-4F07-8B63-08FBDD778F45"),
                    ElementID = "Activity_15hd835",
                    TypeLU = 1,
                    Code = "",
                    MarkerTypeLU = null,
                    OwnerTypeLU = 2,
                    RoleName = ",0:2,",
                    Rule = @"﻿<?xml version=""﻿1.0""﻿ encoding=""utf-16""﻿?>
<UserTaskRuleModel xmlns:xsd=""http://www.w3.org/2001/XMLSchema﻿"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" >
  <AccessType>2</AccessType>
  <GoUpDepartment>false</GoUpDepartment>
  <UserType xsi:nil=""true"" />
  <SpecificDepartmentId xsi:nil=""﻿true""﻿ />
</UserTaskRuleModel>",
                    UserID = null,
                },
                new sysBpmsTask()
                {
                    ID = new Guid("D48A408A-156F-4893-9E8E-144B4C698F87"),
                    ProcessID = new Guid("3E9E69D2-9232-4F07-8B63-08FBDD778F45"),
                    ElementID = "Activity_1maluwj",
                    TypeLU = 1,
                    Code = "",
                    MarkerTypeLU = null,
                    OwnerTypeLU = 2,
                    RoleName = ",0:1,",
                    Rule = @"﻿﻿<?xml version=""1.0"" encoding=""utf-16""?>
<UserTaskRuleModel xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
  <AccessType>2</AccessType>
  <GoUpDepartment>false</GoUpDepartment>
  <UserType xsi:nil=""true"" />
  <SpecificDepartmentId xsi:nil=""true"" />
</UserTaskRuleModel>",
                    UserID = null,
                },
                new sysBpmsTask()
                {
                    ID = new Guid("7A5B0B25-9DA5-4E4E-B743-4F51055B95E0"),
                    ProcessID = new Guid("3E9E69D2-9232-4F07-8B63-08FBDD778F45"),
                    ElementID = "Activity_1k5enxp",
                    TypeLU = 1,
                    Code = "",
                    MarkerTypeLU = null,
                    OwnerTypeLU = 2,
                    RoleName = ",0:1,",
                    Rule = @"﻿<?xml version=""1.0"" encoding=""utf-16""?>
<UserTaskRuleModel xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
  <AccessType>2</AccessType>
  <GoUpDepartment>false</GoUpDepartment>
  <UserType xsi:nil=""true"" />
  <SpecificDepartmentId xsi:nil=""true"" />
</UserTaskRuleModel>",
                    UserID = null,
                },
            });

            //Set Events
            this.db.sysBpmsEvents.AddRange(new List<sysBpmsEvent>()
            {
                new sysBpmsEvent()
                {
                    ID = new Guid("EE6FA936-CEF0-4D5E-9389-6728FC8F1D89"),
                    ProcessID = new Guid("3E9E69D2-9232-4F07-8B63-08FBDD778F45"),
                    ElementID = "Event_1rghv3g",
                    TypeLU = 1,
                    SubType = 0,
                },
                new sysBpmsEvent()
                {
                    ID = new Guid("F0B9137D-FC18-47A6-9A35-7455FCAD7BAA"),
                    ProcessID = new Guid("3E9E69D2-9232-4F07-8B63-08FBDD778F45"),
                    ElementID = "Event_176jcd8",
                    ConfigurationXML = @"﻿<?xml version=""1.0"" encoding=""utf-16""?>
<SubTypeMessageEventModel xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
  <Type>2</Type>
  <KeyType xsi:nil=""true"" />
  <MessageParams />
  <Email>
    <Subject>Appointment</Subject>
    <Content>I am writing to request for an appointment with you on[varAppointmentDate] in company's address.</Content>
    <From> 1 </From>
    <ToType> 2 </ToType>
    <To> varEmail </To>
  </Email>
</SubTypeMessageEventModel>",
                    TypeLU = 3,
                    SubType = 1,
                },
                 new sysBpmsEvent()
                {
                    ID = new Guid("1ED2A8BD-1B61-49FA-BAE4-D51AC257A854"),
                    ProcessID = new Guid("3E9E69D2-9232-4F07-8B63-08FBDD778F45"),
                    ElementID = "Event_1p1p5sg",
                    TypeLU = 2,
                    SubType = 0,
                },
            });

            //Set Steps
            this.db.sysBpmsSteps.AddRange(new List<sysBpmsStep>()
            {
                new sysBpmsStep()
                {
                    ID = new Guid("EE6FA936-CEF0-4D5E-9389-6728FC8F1D89"),
                    TaskID = new Guid("D48A408A-156F-4893-9E8E-144B4C698F87"),
                    DynamicFormID = new Guid("4670331D-D887-486C-935C-5B294CB6238C"),
                    Name = "Aware",
                    Position = 1,
                },
                new sysBpmsStep()
                {
                    ID = new Guid("3DBEA555-B289-4392-8917-EC7D55FD0929"),
                    TaskID = new Guid("1A7BDA0B-1931-455D-A964-03A76708DB0F"),
                    DynamicFormID = new Guid("181D14E6-F44F-4892-9277-E76D196EDA6B"),
                    Name = "Check Request",
                    Position = 1,
                },
               new sysBpmsStep()
                {
                    ID = new Guid("F82C4171-26F9-4D26-9F4B-FBB18CC5F397"),
                    TaskID = new Guid("7A5B0B25-9DA5-4E4E-B743-4F51055B95E0"),
                    DynamicFormID = new Guid("760063C7-EC5D-4994-AB71-61C271B86DD7"),
                    Name = "Add Request",
                    Position = 1,
                },
            });


            //Set Gateways
            this.db.sysBpmsGateways.AddRange(new List<sysBpmsGateway>()
            {
                new sysBpmsGateway()
                {
                    ID = new Guid("357A8441-9F73-4E8D-9DFE-0E6313F3F5C7"),
                    ProcessID = new Guid("3E9E69D2-9232-4F07-8B63-08FBDD778F45"),
                    ElementID = "Gateway_1gt00dk",
                    TypeLU = 1,
                    TraceToStart = "Activity_1maluwj",
                },
                new sysBpmsGateway()
                {
                    ID = new Guid("791E9DCC-3B7E-4D27-8E20-EE3563DCF6D4"),
                    ProcessID = new Guid("3E9E69D2-9232-4F07-8B63-08FBDD778F45"),
                    ElementID = "Gateway_1szzta7",
                    TypeLU = 1,
                    TraceToStart = "Activity_15hd835,Activity_1maluwj,Activity_1k5enxp",
                },
            });

            //Set SequenceFlow
            this.db.sysBpmsSequenceFlows.AddRange(new List<sysBpmsSequenceFlow>()
            {
                new sysBpmsSequenceFlow()
                {
                    ID = new Guid("DDA57963-DF1E-4ED6-A3F9-054D0679F2C7"),
                    ProcessID = new Guid("3E9E69D2-9232-4F07-8B63-08FBDD778F45"),
                    ElementID = "Flow_1enbk0l",
                    Name = "No",
                    SourceElementID = "Gateway_1szzta7",
                    TargetElementID = "Event_1p1p5sg",
                },
                new sysBpmsSequenceFlow()
                {
                    ID = new Guid("5E282A56-F176-4A91-B4F4-0D0B7EF9723F"),
                    ProcessID = new Guid("3E9E69D2-9232-4F07-8B63-08FBDD778F45"),
                    ElementID = "Flow_177jung",
                    Name = "No",
                    SourceElementID = "Gateway_1gt00dk",
                    TargetElementID = "Activity_15hd835",
                },
                new sysBpmsSequenceFlow()
                {
                    ID = new Guid("774696B2-E9A5-48EC-9CCA-47FE33C136DE"),
                    ProcessID = new Guid("3E9E69D2-9232-4F07-8B63-08FBDD778F45"),
                    ElementID = "Flow_0oo1rbi",
                    Name = "",
                    SourceElementID = "Activity_1k5enxp",
                    TargetElementID = "Activity_15hd835",
                },
                new sysBpmsSequenceFlow()
                {
                    ID = new Guid("BE7AA4A2-A7A9-48D5-B4DF-95BB008E9A10"),
                    ProcessID = new Guid("3E9E69D2-9232-4F07-8B63-08FBDD778F45"),
                    ElementID = "Flow_1nhucgt",
                    Name = "",
                    SourceElementID = "Activity_15hd835",
                    TargetElementID = "Gateway_1szzta7",
                },
                new sysBpmsSequenceFlow()
                {
                    ID = new Guid("D84838B1-75C7-48D4-86CA-A127274C147C"),
                    ProcessID = new Guid("3E9E69D2-9232-4F07-8B63-08FBDD778F45"),
                    ElementID = "Flow_0mixqiq",
                    Name = "Yes",
                    SourceElementID = "Gateway_1gt00dk",
                    TargetElementID = "Event_1p1p5sg",
                },
                new sysBpmsSequenceFlow()
                {
                    ID = new Guid("E494A424-E165-4292-B2D7-A8FE13BCC130"),
                    ProcessID = new Guid("3E9E69D2-9232-4F07-8B63-08FBDD778F45"),
                    ElementID = "Flow_0va6l5w",
                    Name = "",
                    SourceElementID = "Event_1rghv3g",
                    TargetElementID = "Activity_1k5enxp",
                },
                new sysBpmsSequenceFlow()
                {
                    ID = new Guid("C6CCD555-6AEB-4446-8456-B44BC8C19F0D"),
                    ProcessID = new Guid("3E9E69D2-9232-4F07-8B63-08FBDD778F45"),
                    ElementID = "Flow_1k2y640",
                    Name = "Yes",
                    SourceElementID = "Gateway_1szzta7",
                    TargetElementID = "Event_176jcd8",
                },
                new sysBpmsSequenceFlow()
                {
                    ID = new Guid("41DB5E6C-8338-406F-9F41-F5918507A1AD"),
                    ProcessID = new Guid("3E9E69D2-9232-4F07-8B63-08FBDD778F45"),
                    ElementID = "Flow_1g1gxzb",
                    Name = "",
                    SourceElementID = "Activity_1maluwj",
                    TargetElementID = "Gateway_1gt00dk",
                },
                new sysBpmsSequenceFlow()
                {
                    ID = new Guid("9BD33B8C-3E12-4039-9BEE-F9349EC73684"),
                    ProcessID = new Guid("3E9E69D2-9232-4F07-8B63-08FBDD778F45"),
                    ElementID = "Flow_0wblxyd",
                    Name = "",
                    SourceElementID = "Event_176jcd8",
                    TargetElementID = "Activity_1maluwj",
                },
            });

            //Set Conditions
            this.db.sysBpmsConditions.AddRange(new List<sysBpmsCondition>()
            {
                new sysBpmsCondition()
                {
                    ID = new Guid("742B0D7E-C9A6-450C-BB0A-25CA16A35B59"),
                    GatewayID = new Guid("791E9DCC-3B7E-4D27-8E20-EE3563DCF6D4"),
                    SequenceFlowID = new Guid("DDA57963-DF1E-4ED6-A3F9-054D0679F2C7"),
                    Code = @"<?xml version=""1.0"" encoding=""utf-16""?>
             <DesignCodeModel xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
               <Code><![CDATA[]]></Code>
               <DesignCode><![CDATA[<ArrayOfObjects><DCConditionModel>
<ID>7c4dce71-360a-4b05-8eaf-f2b3d95b35d6</ID>
<ParentShapeID></ParentShapeID>
<IsFirst>true</IsFirst>
<ShapeID></ShapeID>
<Name>Approved</Name>
<FuncName>func100</FuncName>
<IsOutputYes>false</IsOutputYes>
<ActionType>2</ActionType>
<EvaluateType>1</EvaluateType>
<Rows><DCRowConditionModel>
<FirstConditionType>1</FirstConditionType>
<FirstConditionValue>varStatus</FirstConditionValue>
<SecondConditionType>2</SecondConditionType>
<SecondConditionValue>2</SecondConditionValue>
<OperationType>1</OperationType>
</DCRowConditionModel></Rows>
</DCConditionModel></ArrayOfObjects>]]></DesignCode>
               <ID>143af94f-5a94-4043-b89b-f75fa4216105</ID>
               <TimeStamp>2021031400380593</TimeStamp>
               <Assemblies>undefined</Assemblies>
               <Diagram><![CDATA[]]></Diagram>
             </DesignCodeModel>",
                },
                new sysBpmsCondition()
                {
                    ID = new Guid("DE45EBE3-50A1-4CC8-94B8-5134EAE9A1C3"),
                    GatewayID = new Guid("357A8441-9F73-4E8D-9DFE-0E6313F3F5C7"),
                    SequenceFlowID = new Guid("5E282A56-F176-4A91-B4F4-0D0B7EF9723F"),
                    Code = @"<?xml version=""1.0"" encoding=""utf-16""?>
             <DesignCodeModel xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
               <Code><![CDATA[]]></Code>
               <DesignCode><![CDATA[<ArrayOfObjects><DCConditionModel>
<ID>f62a6eb9-b8f9-45f5-98ca-3805d807fc69</ID>
<ParentShapeID></ParentShapeID>
<IsFirst>true</IsFirst>
<ShapeID></ShapeID>
<Name>Reject</Name>
<FuncName>func100</FuncName>
<IsOutputYes>null</IsOutputYes>
<ActionType>2</ActionType>
<EvaluateType>1</EvaluateType>
<Rows><DCRowConditionModel>
<FirstConditionType>1</FirstConditionType>
<FirstConditionValue>varUserStatus</FirstConditionValue>
<SecondConditionType>2</SecondConditionType>
<SecondConditionValue>2</SecondConditionValue>
<OperationType>1</OperationType>
</DCRowConditionModel></Rows>
</DCConditionModel></ArrayOfObjects>]]></DesignCode>
               <ID>070fe696-2d3f-4d9c-ac61-539827529f26</ID>
               <TimeStamp>2021031400380995</TimeStamp>
               <Assemblies>undefined</Assemblies>
               <Diagram><![CDATA[]]></Diagram>
             </DesignCodeModel>",
                },
                new sysBpmsCondition()
                {
                    ID = new Guid("2949A861-C549-43C6-AF58-5E60E0DAD934"),
                    GatewayID = new Guid("791E9DCC-3B7E-4D27-8E20-EE3563DCF6D4"),
                    SequenceFlowID = new Guid("C6CCD555-6AEB-4446-8456-B44BC8C19F0D"),
                    Code = @"<?xml version=""1.0"" encoding=""utf-16""?>
             <DesignCodeModel xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
               <Code><![CDATA[]]></Code>
               <DesignCode><![CDATA[<ArrayOfObjects><DCConditionModel>
<ID>a3e4fadb-783a-4c2d-acdc-bf7bc5009a30</ID>
<ParentShapeID></ParentShapeID>
<IsFirst>true</IsFirst>
<ShapeID></ShapeID>
<Name>Reject</Name>
<FuncName>func100</FuncName>
<IsOutputYes>false</IsOutputYes>
<ActionType>2</ActionType>
<EvaluateType>1</EvaluateType>
<Rows><DCRowConditionModel>
<FirstConditionType>1</FirstConditionType>
<FirstConditionValue>varStatus</FirstConditionValue>
<SecondConditionType>2</SecondConditionType>
<SecondConditionValue>1</SecondConditionValue>
<OperationType>1</OperationType>
</DCRowConditionModel></Rows>
</DCConditionModel></ArrayOfObjects>]]></DesignCode>
               <ID>00580d60-3403-4a54-bef5-b69cd4286eba</ID>
               <TimeStamp>2021031400380687</TimeStamp>
               <Assemblies>undefined</Assemblies>
               <Diagram><![CDATA[]]></Diagram>
             </DesignCodeModel>",
                },
                new sysBpmsCondition()
                {
                    ID = new Guid("73C2B0D9-F97A-46BF-9406-C58B0049B42C"),
                    GatewayID = new Guid("357A8441-9F73-4E8D-9DFE-0E6313F3F5C7"),
                    SequenceFlowID = new Guid("D84838B1-75C7-48D4-86CA-A127274C147C"),
                    Code = @"<?xml version=""1.0"" encoding=""utf-16""?>
             <DesignCodeModel xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
               <Code><![CDATA[]]></Code>
               <DesignCode><![CDATA[<ArrayOfObjects><DCConditionModel>
<ID>be360459-dab9-4653-bcb9-7aea6011826e</ID>
<ParentShapeID></ParentShapeID>
<IsFirst>true</IsFirst>
<ShapeID></ShapeID>
<Name>Approved</Name>
<FuncName>func100</FuncName>
<IsOutputYes>false</IsOutputYes>
<ActionType>2</ActionType>
<EvaluateType>1</EvaluateType>
<Rows><DCRowConditionModel>
<FirstConditionType>1</FirstConditionType>
<FirstConditionValue>varUserStatus</FirstConditionValue>
<SecondConditionType>2</SecondConditionType>
<SecondConditionValue>1</SecondConditionValue>
<OperationType>1</OperationType>
</DCRowConditionModel></Rows>
</DCConditionModel></ArrayOfObjects>]]></DesignCode>
               <ID>56b12a5d-ea86-44b9-8357-8c4d67f65600</ID>
               <TimeStamp>2021031400380311</TimeStamp>
               <Assemblies>undefined</Assemblies>
               <Diagram><![CDATA[]]></Diagram>
             </DesignCodeModel>",
                },
            });

            //Set EmailAccounts
            this.db.sysBpmsEmailAccounts.AddRange(new List<sysBpmsEmailAccount>()
            {
              new sysBpmsEmailAccount()
                {
                    ID = new Guid("D9B6F95E-CDB3-4DBE-B0D2-1E044F459D8A"),
                    ObjectTypeLU = 3,
                    ObjectID = new Guid("7AE9DA09-8369-EB11-924E-54E1ADE02B17"),
                    SMTP = "smtp.gmail.com",
                    Port = "587",
                    MailPassword = "13701370",
                    Email = "taha.cod@gmail.com",
                },
              new sysBpmsEmailAccount()
                {
                    ID = new Guid("EE0B70FA-E45D-4AE0-97B5-291FDE0B1690"),
                    ObjectTypeLU = 1,
                    ObjectID = null,
                    SMTP = "smtp.office365.com",
                    Port = "587",
                    MailPassword = "890065842+",
                    Email = "easy-bpms@outlook.com",
                },
              new sysBpmsEmailAccount()
                {
                    ID = new Guid("7E71C7BB-8269-EB11-924E-54E1ADE02B17"),
                    ObjectTypeLU = 3,
                    ObjectID = new Guid("EFF13FCD-7D69-EB11-924E-54E1ADE02B17"),
                    SMTP = "taga",
                    Port = "412",
                    MailPassword = "123",
                    Email = "taha.cod@outlook.com",
                },
              new sysBpmsEmailAccount()
                {
                    ID = new Guid("A60D9C5C-A86C-EB11-924F-54E1ADE02B17"),
                    ObjectTypeLU = 1,
                    ObjectID = null,
                    SMTP = "gmail",
                    Port = "441",
                    MailPassword = "123456",
                    Email = "taha.cod@gmail.com",
                },
              new sysBpmsEmailAccount()
                {
                    ID = new Guid("363B4625-6198-42FF-900C-DD43456EFA69"),
                    ObjectTypeLU = 3,
                    ObjectID = new Guid("406B6213-8569-EB11-924E-54E1ADE02B17"),
                    SMTP = "smtp.gmail.com",
                    Port = "587",
                    MailPassword = "13701370",
                    Email = "taha.cod@gmail.com",
                },
            });

            this.db.SaveChanges();
        }

        public void SeedUsersDepartments()
        {
            //Set Users
            this.db.sysBpmsUsers.AddRange(new List<sysBpmsUser>()
            {
              new sysBpmsUser()
                {
                    ID = new Guid("EFF13FCD-7D69-EB11-924E-54E1ADE02B17"),
                    Username = "BPMS_ProjectManager",
                    FirstName = "Project",
                    LastName = "Manager",
                    Email = "BPMS_ProjectManager@outlook.com",
                    Tel = "",
                    Mobile = ""
                },
              new sysBpmsUser()
                {
                    ID = new Guid("7AE9DA09-8369-EB11-924E-54E1ADE02B17"),
                    Username = "bpms_employee",
                    FirstName = "bpms",
                    LastName = "employee",
                    Email = "bpms.outlook.com",
                    Tel = "",
                    Mobile = ""
                },
              new sysBpmsUser()
                {
                    ID = new Guid("406B6213-8569-EB11-924E-54E1ADE02B17"),
                    Username = "bpms_manager",
                    FirstName = "bpms",
                    LastName = "manager",
                    Email = "bpms@outlook.com",
                    Tel = "",
                    Mobile = ""
                },
              new sysBpmsUser()
                {
                    ID = new Guid("B34F7A61-0973-EB11-9250-54E1ADE02B17"),
                    Username = "host",
                    FirstName = "SuperUser",
                    LastName = "Account",
                    Email = "taha.cod@outlook.com",
                    Tel = "",
                    Mobile = ""
                },
              new sysBpmsUser()
                {
                    ID = new Guid("D0909850-3B3F-4885-8647-BD85B8CEC5EB"),
                    Username = "Bpms_Seller",
                    FirstName = "",
                    LastName = "",
                    Email = "Bpms_Seller@outlook.com",
                },
            });
            //Set Departments
            this.db.sysBpmsDepartments.AddRange(new List<sysBpmsDepartment>()
            {
              new sysBpmsDepartment()
                {
                    ID = new Guid("9965C22C-8569-EB11-924E-54E1ADE02B17"),
                    Name = "Sales",
                    IsActive = true,
                },
              new sysBpmsDepartment()
                {
                    ID = new Guid("9A65C22C-8569-EB11-924E-54E1ADE02B17"),
                    Name = "Finance",
                    IsActive = true,
                },
              new sysBpmsDepartment()
                {
                    ID = new Guid("42801060-8569-EB11-924E-54E1ADE02B17"),
                    Name = "Board",
                    IsActive = true,
                },
              new sysBpmsDepartment()
                {
                    ID = new Guid("9F79AF47-0873-EB11-9250-54E1ADE02B17"),
                    Name = "Development",
                    IsActive = true,
                },
            });
            //Set Departments
            this.db.sysBpmsDepartmentMembers.AddRange(new List<sysBpmsDepartmentMember>()
            {
              new sysBpmsDepartmentMember()
                {
                    ID = new Guid("EB3655BC-8969-EB11-924E-54E1ADE02B17"),
                    DepartmentID = new Guid("42801060-8569-EB11-924E-54E1ADE02B17"),
                    UserID = new Guid("406B6213-8569-EB11-924E-54E1ADE02B17"),
                    RoleLU = 2,
                },
              new sysBpmsDepartmentMember()
                {
                    ID = new Guid("104E8C63-0873-EB11-9250-54E1ADE02B17"),
                    DepartmentID = new Guid("9A65C22C-8569-EB11-924E-54E1ADE02B17"),
                    UserID = new Guid("406B6213-8569-EB11-924E-54E1ADE02B17"),
                    RoleLU = 5,
                },
              new sysBpmsDepartmentMember()
                {
                    ID = new Guid("2A44C86E-0873-EB11-9250-54E1ADE02B17"),
                    DepartmentID = new Guid("9F79AF47-0873-EB11-9250-54E1ADE02B17"),
                    UserID = new Guid("EFF13FCD-7D69-EB11-924E-54E1ADE02B17"),
                    RoleLU = 6,
                },
              new sysBpmsDepartmentMember()
                {
                    ID = new Guid("2B44C86E-0873-EB11-9250-54E1ADE02B17"),
                    DepartmentID = new Guid("9F79AF47-0873-EB11-9250-54E1ADE02B17"),
                    UserID = new Guid("EFF13FCD-7D69-EB11-924E-54E1ADE02B17"),
                    RoleLU = 3,
                },
              new sysBpmsDepartmentMember()
                {
                    ID = new Guid("2C44C86E-0873-EB11-9250-54E1ADE02B17"),
                    DepartmentID = new Guid("9F79AF47-0873-EB11-9250-54E1ADE02B17"),
                    UserID = new Guid("7AE9DA09-8369-EB11-924E-54E1ADE02B17"),
                    RoleLU = 3,
                },
              new sysBpmsDepartmentMember()
                {
                    ID = new Guid("8959AA7C-0873-EB11-9250-54E1ADE02B17"),
                    DepartmentID = new Guid("9A65C22C-8569-EB11-924E-54E1ADE02B17"),
                    UserID = new Guid("406B6213-8569-EB11-924E-54E1ADE02B17"),
                    RoleLU = 3,
                },
              new sysBpmsDepartmentMember()
                {
                    ID = new Guid("44773199-0873-EB11-9250-54E1ADE02B17"),
                    DepartmentID = new Guid("42801060-8569-EB11-924E-54E1ADE02B17"),
                    UserID = new Guid("406B6213-8569-EB11-924E-54E1ADE02B17"),
                    RoleLU = 3,
                },
            });

            this.db.SaveChanges();
        }

        public void SeedDocuments()
        {
            //Set Documents
            this.db.sysBpmsDocumentFolders.AddRange(new List<sysBpmsDocumentFolder>()
            {
              new sysBpmsDocumentFolder()
                {
                    ID = new Guid("C9ED3C81-116F-EB11-924F-54E1ADE02B17"),
                    NameOf = "Recruitment",
                    DisplayName = "Recruitment",
                    IsActive = true
                },
            });

            this.db.sysBpmsDocumentDefs.AddRange(new List<sysBpmsDocumentDef>()
            {
              new sysBpmsDocumentDef()
                {
                    ID = new Guid("D42BD348-126F-EB11-924F-54E1ADE02B17"),
                    DocumentFolderID = new Guid("C9ED3C81-116F-EB11-924F-54E1ADE02B17"),
                    NameOf = "CV",
                    DisplayName = "CV",
                    MaxSize = 5000,
                    ValidExtentions = "pdf,docx",
                    IsMandatory = true,
                    IsSystemic = false,
                    IsActive = true
                },
              new sysBpmsDocumentDef()
                {
                    ID = new Guid("1C2C5567-126F-EB11-924F-54E1ADE02B17"),
                    DocumentFolderID = new Guid("C9ED3C81-116F-EB11-924F-54E1ADE02B17"),
                    NameOf = "",
                    DisplayName = "Motivation Letter",
                    MaxSize = 6000,
                    ValidExtentions = "pdf,docx",
                    IsMandatory = false,
                    IsSystemic = false,
                    IsActive = true
                },
            });

            this.db.SaveChanges();
        }
    }
}
