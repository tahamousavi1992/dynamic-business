using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace DynamicBusiness.BPMS.Domain
{
    public static class DesignCodeUtility
    {
        public static List<string> GetAllJavaMethods()
        {
            return new List<string>() {
                "FormControl.get(\"\").value",
                "FormControl.get(\"\").visibility",
                "FormControl.get(\"\").href",
                "FormControl.get(\"\").src",
                "FormControl.get(\"\").removeAt(index)",
                "FormControl.get(\"\").addItem({ text\"\", value=\"\"})",
                "FormControl.get(\"\").getItemAt(index)",
                "FormControl.get(\"\").getSelectedItem()",
                "FormControl.get(\"\").getSelectedItems()",
                "FormControl.get(\"\").updateItems()",
                "FormControl.get(\"\").checked",
                "FormControl.get(\"\").readOnly",
                "FormControl.openFormPopUp(event.target,\"formId\",paramsObject,callBackFunction,800,800)",
                "FormControl.closeModal()",
            }.OrderBy(c => c).ToList();
        }

        public static List<string> GetAllOperators()
        {
            return new List<string>() { "==", ">", ">=", "<", "<=", "&&", "||", "(", ")", "+", "*", "-", "/" }.OrderBy(c => c).ToList();
        }

        public static List<string> GetAllSysMethods()
        {
            return new List<string>() {
                "Convert.ToInt32(\"\")",
                "Convert.ToDecimal(\"\")",
                "Convert.ToDateTime(\"\")",
                "Convert.ToInt64(\"\")",
                "Convert.ToString(\"\")",
                "Convert.ToBoolean(\"true\")",
                "Convert.ToChar(\"\")",
            }.OrderBy(c => c).ToList();
        }

        public static List<string> GetDocumentMethods()
        {
            return new List<string>() {
                "DocumentHelper.GetList(documentDefID:null,documentFolderID:null,entityID:null,threadId:null)",
                "DocumentHelper.CheckMandatory(documentDefID:null,documentFolderID:null,entityID:null,threadId:null)",
                "DocumentHelper.Download(base64,fileName)",
                "DocumentHelper.DownloadByStream(streamFile,fileName)",
                "DocumentHelper.DownloadByByte(bytes,fileName)",
            }.OrderBy(c => c).ToList();
        }

        public static List<string> GetVariableMethods()
        {
            return new List<string>() {
                "VariableHelper.Save(\"VariableName\",value:null,queryModel:null)",
                "VariableHelper.Save(\"VariableName\",queryModel:null)",
                "VariableHelper.Set(\"VariableName\",value:null)",
                "VariableHelper.Get(\"VariableName\",queryModel:null)",
                "VariableHelper.GetValue(\"VariableName\",queryModel:null)",
                "VariableHelper.Clear(\"VariableName\")",
            }.OrderBy(c => c).ToList();
        }

        public static List<string> GetMessageMethods()
        {
            return new List<string>() {
                "MessageHelper.AddError(\"\")",
                "MessageHelper.AddInfo(\"\")",
                "MessageHelper.AddSuccess(\"\")",
                "MessageHelper.AddWarning(\"\")",
            }.OrderBy(c => c).ToList();
        }

        public static List<string> GetAccessMethods()
        {
            return new List<string>() {
                "AccessHelper.GetUserID(departmentID,roleCode)",
                "AccessHelper.GetUser(departmentID,roleCode)",
                "AccessHelper.GetUserList(departmentID,roleCode:null)",
                "AccessHelper.GetDepartmentList(userID)",
                "AccessHelper.GetRoleCode(userID,departmentID)",
                "AccessHelper.GetRoleCodeList(userID,departmentID:null)",
                "AccessHelper.AddRoleToUser(userID,departmentID,roleCode)",
                "AccessHelper.RemoveRoleFromUser(userID,departmentID,roleCode)",
                "AccessHelper.GetDepartmentHierarchyByUserId(userID:Guid.Empty,roleCode:0,goUpDepartment:true)",
            }.OrderBy(c => c).ToList();
        }

        public static List<string> GetHelperMethods()
        {
            return new List<string>() {
                "ControlHelper.GetValue(\"ControlId\")",
                "ControlHelper.SetValue(\"ControlId\",value:null)",
                "ControlHelper.BindDataSource(\"ControlId\",value:null)",
                "ControlHelper.SetVisibility(\"ControlId\",true)",
                "OperationHelper.RunQuery(\"operationId\",queryModel:null)",
                "UrlHelper.GetParameter(\"ParameterName\")",
                "UrlHelper.RedirectUrl(\"url\")",
                "UrlHelper.RedirectForm(\"applicationPageId\")",
                "UserHelper.CreateSiteUser(userInfo:null,password:\"\",doLogin:true)",
                "UserHelper.CreateSiteUser(\"userName\",\"firstName\",\"LastName\",\"email\",\"password\",doLogin:true,createBpms:true)",
                "UserHelper.CreateBpmsUser(\"userName\",\"firstName\",\"LastName\",\"email\",\"mobile\",\"telePhone\")",
                "UserHelper.GetSiteUser(\"userName\")",
                "UserHelper.GetUserByUserName(\"userName\")",
                "UserHelper.GetUserByID(\"id\")",
                "ProcessHelper.GetThreadProcessStatus(threadID)",
            }.OrderBy(c => c).ToList();
        }

        public static List<string> GetAllSysProperties()
        {
            return new List<string>() {
                "this.GetCurrentUserID",
                "this.GetCurrentUserName",
                "this.GetThreadUserID",
                "this.GetThreadID",
            }.OrderBy(c => c).ToList();
        }

        public static DesignCodeModel GetDesignCodeFromXml(string xmlData)
        {
            if (!string.IsNullOrWhiteSpace(xmlData))
            {
                if (xmlData.IndexOf("<Code>") > 0 && xmlData.IndexOf("</Code>") != -1 && xmlData.IndexOf("<Code><![CDATA[") == -1)
                {
                    xmlData = xmlData.Insert(xmlData.IndexOf("<Code>") + 6, "<![CDATA[");
                    xmlData = xmlData.Insert(xmlData.IndexOf("</Code>"), "]]>");
                }
                if (xmlData.IndexOf("<DesignCode>") > 0 && xmlData.IndexOf("</DesignCode>") != -1 && xmlData.IndexOf("<DesignCode><![CDATA[") == -1)
                {
                    xmlData = xmlData.Insert(xmlData.IndexOf("<DesignCode>") + 12, "<![CDATA[");
                    xmlData = xmlData.Insert(xmlData.IndexOf("</DesignCode>"), "]]>");
                }
                if (xmlData.IndexOf("<Diagram>") > 0 && xmlData.IndexOf("</Diagram>") != -1 && xmlData.IndexOf("<Diagram><![CDATA[") == -1)
                {
                    xmlData = xmlData.Insert(xmlData.IndexOf("<Diagram>") + 9, "<![CDATA[");
                    xmlData = xmlData.Insert(xmlData.IndexOf("</Diagram>"), "]]>");
                }
                return xmlData.ParseXML<DesignCodeModel>() ?? new DesignCodeModel();
            }
            else return new DesignCodeModel();
        }

        public static string ConvertToXml(DesignCodeModel designCodeModel)
        {
            string xmlData = ParseHelpers.BuildXml<DesignCodeModel>(designCodeModel);
            if (xmlData.IndexOf("<Code>") > 0 && xmlData.IndexOf("</Code>") != -1 && xmlData.IndexOf("<Code><![CDATA[") == -1)
            {
                xmlData = xmlData.Insert(xmlData.IndexOf("<Code>") + 6, "<![CDATA[");
                xmlData = xmlData.Insert(xmlData.IndexOf("</Code>"), "]]>");
            }
            return xmlData;
        }

        /// <summary>
        /// for converting a  list of objects related to design code items like DCCallMethodModel class to xml string
        /// </summary>
        public static string ConvertDesignCodeObjectToXml(object Item)
        {
            return HttpUtility.HtmlDecode(((DCBaseModel)Item).ToXmlElement().ToStringObj());
        }

        /// <summary>
        /// for converting a xml string to list of objects related to design code items like DCCallMethodModel class
        /// </summary>
        public static List<object> GetListOfDesignCode(string xmlData)
        {
            List<object> items = new List<object>();
            if (!string.IsNullOrWhiteSpace(xmlData))
            {
                if (xmlData.IndexOf("<ExpressionCode>") > 0 && xmlData.IndexOf("</ExpressionCode>") != -1 && xmlData.IndexOf("<ExpressionCode><![CDATA[") == -1)
                {
                    xmlData = xmlData.Insert(xmlData.IndexOf("<ExpressionCode>") + 16, "<![CDATA[");
                    xmlData = xmlData.Insert(xmlData.IndexOf("</ExpressionCode>"), "]]>");
                }
                XElement XElement = XElement.Parse(xmlData);
                items = (from c in XElement.Elements()
                         select
                         c.Name == nameof(DCSetVariableModel) ? new DCSetVariableModel().FillData(c) :
                         c.Name == nameof(DCConditionModel) ? new DCConditionModel().FillData(c) :
                         c.Name == nameof(DCWebServiceModel) ? new DCWebServiceModel().FillData(c) :
                         c.Name == nameof(DCExpressionModel) ? new DCExpressionModel().FillData(c) :
                         c.Name == nameof(DCSetControlModel) ? new DCSetControlModel().FillData(c) :
                         c.Name == nameof(DCEntityModel) ? new DCEntityModel().FillData(c) :
                         c.Name == nameof(DCCallMethodModel) ? new DCCallMethodModel().FillData(c) : null)
                         .Cast<object>().ToList();
                return items;
            }
            else return items;
        }

        /// <summary>
        /// for converting a xml string to list of objects related to design code items like DCCallMethodModel class
        /// </summary>
        public static T GetObjectOfDesignCode<T>(string xmlData)
        {
            if (!string.IsNullOrWhiteSpace(xmlData))
            {
                if (xmlData.IndexOf("<ExpressionCode>") > 0 && xmlData.IndexOf("</ExpressionCode>") != -1 && xmlData.IndexOf("<ExpressionCode><![CDATA[") == -1)
                {
                    xmlData = xmlData.Insert(xmlData.IndexOf("<ExpressionCode>") + 16, "<![CDATA[");
                    xmlData = xmlData.Insert(xmlData.IndexOf("</ExpressionCode>"), "]]>");
                }
                XElement XElement = XElement.Parse(xmlData);
                if (XElement.Name == nameof(DCSetVariableModel))
                    return (T)(object)new DCSetVariableModel().FillData(XElement);
                if (XElement.Name == nameof(DCConditionModel))
                    return (T)(object)new DCConditionModel().FillData(XElement);
                if (XElement.Name == nameof(DCCallMethodModel))
                    return (T)(object)new DCCallMethodModel().FillData(XElement);
                if (XElement.Name == nameof(DCExpressionModel))
                    return (T)(object)new DCExpressionModel().FillData(XElement);
                if (XElement.Name == nameof(DCSetControlModel))
                    return (T)(object)new DCSetControlModel().FillData(XElement);
                if (XElement.Name == nameof(DCEntityModel))
                    return (T)(object)new DCEntityModel().FillData(XElement);
                if (XElement.Name == nameof(DCWebServiceModel))
                    return (T)(object)new DCWebServiceModel().FillData(XElement);
                return default(T);

            }
            else return default(T);
        }

        public static Tuple<string, string> RenderObjectsToCode(string xmlData)
        {
            string code = string.Empty;
            List<DCExpressionModel> listItems = GetListOfDesignCode(xmlData).Where(c => c is DCExpressionModel).Cast<DCExpressionModel>().ToList();
            List<(string funcName, string funcBody)> addedFunctionList = new List<(string funcName, string funcBody)>();
            string assemblies = string.Join(",", listItems.Select(c => c.Assemblies).Where(c => !string.IsNullOrWhiteSpace(c)));
            RenderExpression(addedFunctionList, listItems);

            if (addedFunctionList.Any())
            {
                code += "switch (funcName){" + Environment.NewLine;
                addedFunctionList.ForEach(c =>
                {
                    code += "case \"" + c.funcName + "\" :" + Environment.NewLine;
                    code += c.funcName + "();" + Environment.NewLine;
                    code += "break;" + Environment.NewLine;
                });
                code += "}";

                foreach (var item in addedFunctionList)
                {
                    code += Environment.NewLine;
                    code += $@"object {item.funcName}(){{{Environment.NewLine + item.funcBody + Environment.NewLine + " return true; " }}}";
                }
            }

            return new Tuple<string, string>(code, assemblies);
        }

        private static void RenderExpression(List<(string funcName, string funcBody)> addedFunctionList, List<DCExpressionModel> list)
        {
            foreach (DCExpressionModel expressionModel in list)
            {
                if (string.IsNullOrWhiteSpace(expressionModel.FuncName))
                    expressionModel.FuncName = GetFunctionName(expressionModel.ShapeID);

                string actionCode = expressionModel.GetRenderedCode();
                //Adding action function to list
                addedFunctionList.Add((expressionModel.FuncName, actionCode));

            }
        }
        public static string GetFunctionName(string shapeId) => string.IsNullOrWhiteSpace(shapeId) ? "" : "func" + shapeId.Replace("-", "_").Substring(0, 8);

    }
}
