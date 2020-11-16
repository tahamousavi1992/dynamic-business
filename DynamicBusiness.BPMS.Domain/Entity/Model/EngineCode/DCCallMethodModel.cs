using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DynamicBusiness.BPMS.Domain
{
    [DataContract]
    public class DCCallMethodModel : DCBaseModel
    {
        public DCCallMethodModel() { }
        public DCCallMethodModel(string id, string name, string shapeid, string parentShapeId, bool? isOutputYes,
             bool isFirst, string methodID, int methodGroupType, Func<Guid, List<string>> getOperationParam)
            : base(id, name, e_ActionType.CallMethod, parentShapeId, shapeid, isOutputYes, isFirst, null)
        {

            this.MethodID = methodID;
            this.MethodGroupType = (e_MethodGroupType)methodGroupType;
            switch (this.MethodGroupType)
            {
                case e_MethodGroupType.User:
                    switch (this.MethodID)
                    {
                        case "CreateBpmsUser":
                            this.Rows = this.GetListByDef("userName:String,firstName:String,LastName:String,email:String,mobile:String,telePhone:String");
                            break;
                        case "CreateSiteUser":
                            this.Rows = this.GetListByDef("userName:String,firstName:String,LastName:String,email:String,password:String,doLogin:Boolean,createBpms:Boolean");
                            break;
                        case "GetUserPropertyByID":
                            this.Rows = this.GetListByDef("id:Guid,propertyName:String");
                            break;
                        case "GetUserPropertyByUserName":
                            this.Rows = this.GetListByDef("userName:String,propertyName:String");
                            break;
                    }
                    break;
                case e_MethodGroupType.Message:
                    switch (this.MethodID)
                    {
                        case "AddError":
                            this.Rows = this.GetListByDef("message:String");
                            break;
                        case "AddInfo":
                            this.Rows = this.GetListByDef("message:String");
                            break;
                        case "AddSuccess":
                            this.Rows = this.GetListByDef("message:String");
                            break;
                        case "AddWarning":
                            this.Rows = this.GetListByDef("message:String");
                            break;
                    }
                    break;
                case e_MethodGroupType.Url:
                    switch (this.MethodID)
                    {
                        case "RedirectUrl":
                            this.Rows = this.GetListByDef("url:String");
                            break;
                        case "RedirectForm":
                            this.Rows = this.GetListByDef("applicationPageId:Guid");
                            break;
                    }
                    break;
                case e_MethodGroupType.Operation:
                    this.Rows = getOperationParam(this.MethodID.ToGuidObj()).Select(c => new DCMethodParaetersModel() { Value = string.Empty, ValueType = e_ValueType.Static, ParameterName = c }).ToList();
                    break;
                case e_MethodGroupType.AccessRule:
                    switch (this.MethodID)
                    {
                        case "GetDepartmentHierarchyByUserId":
                            this.Rows = this.GetListByDef("userID:Guid,roleCode:Integer,goUpDepartment:Boolean");
                            break;
                        case "GetUserID":
                            this.Rows = this.GetListByDef("departmentID:Guid,roleCode:Integer");
                            break;
                        case "GetRoleCode":
                            this.Rows = this.GetListByDef("userID:Guid,departmentID:Guid");
                            break;
                        case "GetRoleCodeList":
                            this.Rows = this.GetListByDef("userID:Guid,departmentID:Guid");
                            break;
                        case "AddRoleToUser":
                            this.Rows = this.GetListByDef("userID:Guid,departmentID:Guid,roleCode:Integer");
                            break;
                        case "RemoveRoleFromUser":
                            this.Rows = this.GetListByDef("userID:Guid,departmentID:Guid,roleCode:Integer");
                            break;
                    }
                    break;
            }
        }

        [DataMember] 
        public e_MethodGroupType MethodGroupType { get; set; }
        /// <summary>
        /// set method name in it like CreateBpmsUser
        /// </summary>
        [DataMember]
        public string MethodID { get; set; }
        [DataMember]
        public string RetVariableName { get; set; }
        [DataMember]
        public List<DCMethodParaetersModel> Rows { get; set; }

        public enum e_MethodGroupType
        {
            [Description("User")]
            User = 1,
            [Description("Message")]
            Message = 2,
            [Description("Operation")]
            Operation = 3,
            [Description("AccessRule")]
            AccessRule = 4,
            [Description("Url")]
            Url = 5,
        }
        public override object FillData(XElement xElement)
        {
            base.FillData(xElement);
            this.MethodGroupType = (DCCallMethodModel.e_MethodGroupType)xElement.GetValue(nameof(DCCallMethodModel.MethodGroupType)).ToIntObj();
            this.MethodID = xElement.GetValue(nameof(DCCallMethodModel.MethodID));
            this.RetVariableName = xElement.GetValue(nameof(DCCallMethodModel.RetVariableName));
            this.Rows = (from c in xElement.Element(nameof(DCCallMethodModel.Rows)).Elements(nameof(DCMethodParaetersModel))
                         select new DCMethodParaetersModel()
                         {
                             ParameterName = c.GetValue(nameof(DCMethodParaetersModel.ParameterName)),
                             Value = c.GetValue(nameof(DCMethodParaetersModel.Value)),
                             ValueType = (DCBaseModel.e_ValueType)c.GetValue(nameof(DCMethodParaetersModel.ValueType)).ToIntObj(),
                         }).ToList();
            return this;
        }

        public override XElement ToXmlElement()
        {
            return new XElement(nameof(DCCallMethodModel),
                     base.ToXmlElementArray(),
                     new XElement(nameof(DCCallMethodModel.MethodGroupType), (int)this.MethodGroupType),
                     new XElement(nameof(DCCallMethodModel.MethodID), this.MethodID),
                      new XElement(nameof(DCCallMethodModel.RetVariableName), this.RetVariableName),
                     new XElement(nameof(DCCallMethodModel.Rows),
                     from c in this.Rows
                     select new XElement(nameof(DCMethodParaetersModel),
                         new XElement(nameof(DCMethodParaetersModel.ParameterName), c.ParameterName),
                         new XElement(nameof(DCMethodParaetersModel.ValueType), (int)c.ValueType),
                         new XElement(nameof(DCMethodParaetersModel.Value), c.Value)
                         ))
                     );
        }

        public override string GetRenderedCode(Guid? processId, Guid? applicationPageId, IUnitOfWork unitOfWork)
        {
            string code = string.Empty;
            switch (this.MethodGroupType)
            {
                case e_MethodGroupType.User:
                    code += $@"UserHelper.{this.MethodID}({string.Join(",", this.Rows.Select(item => this.GetParameterCode(item)).ToList())});";
                    break;
                case e_MethodGroupType.Message:
                    code += $@"MessageHelper.{this.MethodID}({string.Join(",", this.Rows.Select(item => this.GetParameterCode(item)).ToList())});";
                    break;
                case e_MethodGroupType.Url:
                    code += $@"UrlHelper.{this.MethodID}({string.Join(",", this.Rows.Select(item => this.GetParameterCode(item)).ToList())});";
                    break;
                case e_MethodGroupType.Operation:
                    code += $@"OperationHelper.RunQuery(new Guid(""{this.MethodID}""),{string.Join(",",
                        this.Rows.Select(item => $" new QueryModel(\"{item.ParameterName}\", {this.GetParameterCode(item)})").ToList())
                        });";
                    break;
                case e_MethodGroupType.AccessRule:
                    code += $@"AccessHelper.{this.MethodID}({string.Join(",", this.Rows.Select(item => this.GetParameterCode(item)).ToList())});";
                    break;
            }
            if (!string.IsNullOrWhiteSpace(this.RetVariableName))
            {
                DCBaseModel.e_ConvertType e_Convert = DCBaseModel.GetVariableConvertType(this.RetVariableName, processId, applicationPageId, unitOfWork);
                code = DCBaseModel.WrapCodeWithConvert(code.TrimEnd(';'), e_Convert);
                code = $"VariableHelper.Set(\"{this.RetVariableName}\",{code});";
            }
            return code;
        }

        private string GetParameterCode(DCMethodParaetersModel paraetersModel)
        {
            e_ConvertType e_Convert = e_ConvertType.String;
            string pn = paraetersModel.ParameterName;
            switch (this.MethodGroupType)
            {
                case e_MethodGroupType.User:
                    switch (this.MethodID)
                    {
                        case "CreateBpmsUser":
                            e_Convert = this.ConvertStrToType("userName:String,firstName:String,LastName:String,email:String,mobile:String,telePhone:String".Split(',').FirstOrDefault(c => c.Split(':')[0] == pn).Split(':')[1]);
                            break;
                        case "CreateSiteUser":
                            e_Convert = this.ConvertStrToType("userName:String,firstName:String,LastName:String,email:String,password:String,doLogin:Boolean,createBpms:Boolean".Split(',').FirstOrDefault(c => c.Split(':')[0] == pn).Split(':')[1]);
                            break;
                        case "GetUserPropertyByID":
                            e_Convert = this.ConvertStrToType("id:Guid,propertyName:String".Split(',').FirstOrDefault(c => c.Split(':')[0] == pn).Split(':')[1]);
                            break;
                        case "GetUserPropertyByUserName":
                            e_Convert = this.ConvertStrToType("userName:String,propertyName:String".Split(',').FirstOrDefault(c => c.Split(':')[0] == pn).Split(':')[1]);
                            break;
                    }
                    break;
                case e_MethodGroupType.Message:
                    switch (this.MethodID)
                    {
                        case "AddError":
                            e_Convert = this.ConvertStrToType("message:String".Split(',').FirstOrDefault(c => c.Split(':')[0] == pn).Split(':')[1]);
                            break;
                        case "AddInfo":
                            e_Convert = this.ConvertStrToType("message:String".Split(',').FirstOrDefault(c => c.Split(':')[0] == pn).Split(':')[1]);
                            break;
                        case "AddSuccess":
                            e_Convert = this.ConvertStrToType("message:String".Split(',').FirstOrDefault(c => c.Split(':')[0] == pn).Split(':')[1]);
                            break;
                        case "AddWarning":
                            e_Convert = this.ConvertStrToType("message:String".Split(',').FirstOrDefault(c => c.Split(':')[0] == pn).Split(':')[1]);
                            break;
                    }
                    break;
                case e_MethodGroupType.Url:
                    switch (this.MethodID)
                    {
                        case "RedirectUrl":
                            e_Convert = this.ConvertStrToType("url:String".Split(',').FirstOrDefault(c => c.Split(':')[0] == pn).Split(':')[1]);
                            break;
                        case "RedirectForm":
                            e_Convert = this.ConvertStrToType("applicationPageId:Guid".Split(',').FirstOrDefault(c => c.Split(':')[0] == pn).Split(':')[1]);
                            break;
                    }
                    break;
                case e_MethodGroupType.Operation:
                    e_Convert = e_ConvertType.Nothing;
                    break;
                case e_MethodGroupType.AccessRule:
                    switch (this.MethodID)
                    {
                        case "GetDepartmentHierarchyByUserId":
                            e_Convert = this.ConvertStrToType("userID:Guid,roleCode:Integer,goUpDepartment:Boolean".Split(',').FirstOrDefault(c => c.Split(':')[0] == pn).Split(':')[1]);
                            break;
                        case "GetUserID":
                            e_Convert = this.ConvertStrToType("departmentID:Guid,roleCode:Integer".Split(',').FirstOrDefault(c => c.Split(':')[0] == pn).Split(':')[1]);
                            break;
                        case "GetRoleCode":
                            e_Convert = this.ConvertStrToType("userID:Guid,departmentID:Guid".Split(',').FirstOrDefault(c => c.Split(':')[0] == pn).Split(':')[1]);
                            break;
                        case "GetRoleCodeList":
                            e_Convert = this.ConvertStrToType("userID:Guid,departmentID:Guid".Split(',').FirstOrDefault(c => c.Split(':')[0] == pn).Split(':')[1]);
                            break;
                        case "AddRoleToUser":
                            e_Convert = this.ConvertStrToType("userID:Guid,departmentID:Guid,roleCode:Integer".Split(',').FirstOrDefault(c => c.Split(':')[0] == pn).Split(':')[1]);
                            break;
                        case "RemoveRoleFromUser":
                            e_Convert = this.ConvertStrToType("userID:Guid,departmentID:Guid,roleCode:Integer".Split(',').FirstOrDefault(c => c.Split(':')[0] == pn).Split(':')[1]);
                            break;
                    }
                    break;
            }
            return paraetersModel.GetRendered(e_Convert);
        }

        private e_ConvertType ConvertStrToType(string convert)
        {
            return convert == "Integer" ? e_ConvertType.Integer :
                convert == "Boolean" ? e_ConvertType.Boolean :
                convert == "DateTime" ? e_ConvertType.DateTime :
                convert == "Guid" ? e_ConvertType.Uniqueidentifier :
                convert == "Decimal" ? e_ConvertType.Decimal : e_ConvertType.String;
        }

        private List<DCMethodParaetersModel> GetListByDef(string definition)
        {
            return definition.Split(',').Where(c => !string.IsNullOrWhiteSpace(c)).Select(c => new DCMethodParaetersModel()
            {
                ValueType = e_ValueType.Static,
                Value = string.Empty,
                ParameterName = c.Split(':')[0]
            }).ToList();
        }
    }

    [DataContract]
    public class DCMethodParaetersModel
    {
        [DataMember]
        public string ParameterName { get; set; }
        [DataMember]
        public DCBaseModel.e_ValueType ValueType { get; set; }
        [DataMember]
        public string Value { get; set; }

        public string GetRendered(DCBaseModel.e_ConvertType e_Convert)
        {
            return DCBaseModel.RenderValueType(null, null, null, this.Value, this.ValueType, e_Convert, false);
        }
    }
}
