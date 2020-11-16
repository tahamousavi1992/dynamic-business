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
                "this.CurrentUserID",
                "this.CurrentUserName",
                "this.ThreadUserID",
                "this.ThreadID",
            }.OrderBy(c => c).ToList();
        }

        public static Dictionary<string, string> GetSysPropertiesAsDictionary()
        {
            return new Dictionary<string, string>() {
                {"CurrentUserID" , "شناسه کاربر فعلی"},
                {"CurrentUserName" , "نام کاربری فعلی"},
                {"ThreadUserID" , "شناسه کاربر شروع کننده"},
                {"ThreadID" , "شناسه روال"}
            };
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

        public static Tuple<string, string> RenderObjectsToCode(string xmlData, IUnitOfWork unitOfWork, Guid? processId,
            Guid? applicationId, bool onlyConditional, bool addGotoLabel)
        {
            string code = string.Empty;
            List<object> listItems = GetListOfDesignCode(xmlData);
            List<Tuple<string, string, string>> addedFunctionList = new List<Tuple<string, string, string>>();
            string assemblies = string.Join(",", listItems.Where(c => c is DCExpressionModel).Select(c => ((DCExpressionModel)c).Assemblies).Where(c => !string.IsNullOrWhiteSpace(c)));
            RenderToChildren(addedFunctionList,
               listItems.Where(c => ((DCBaseModel)c).IsFirst).ToList(),
               listItems, unitOfWork, processId, applicationId, onlyConditional, addGotoLabel);

            if (addedFunctionList.Any())
            {
                if (onlyConditional)
                {
                    if (!string.IsNullOrWhiteSpace(addedFunctionList.FirstOrDefault().Item3))
                        code += $@"return {addedFunctionList.FirstOrDefault().Item3};";
                }
                else
                {
                    code += "//" + addedFunctionList.FirstOrDefault().Item2.TrimStringStart("//") + Environment.NewLine;
                    code += $"return {addedFunctionList.FirstOrDefault().Item1}();";
                    foreach (var item in addedFunctionList)
                    {
                        code += Environment.NewLine + "//" + item.Item2.TrimStringStart("//") + Environment.NewLine;
                        code += $@"object {item.Item1}(){{{Environment.NewLine + item.Item3 }}}";
                    }
                }
            }

            return new Tuple<string, string>(code, assemblies);
        }

        private static void RenderToChildren(List<Tuple<string, string, string>> addedFunctionList, List<object> children, List<object> allNodes,
          IUnitOfWork unitOfWork, Guid? processId, Guid? applicationId, bool OnlyConditional, bool addGotoLabel)
        {
            children.ForEach(c =>
            {
                if (string.IsNullOrWhiteSpace(((DCBaseModel)c).FuncName))
                    ((DCBaseModel)c).FuncName = GetFunctionName(((DCBaseModel)c).ShapeID);
            });

            foreach (var Groupitem in children.GroupBy(c => ((DCBaseModel)c).ShapeID))
            {
                string funcName = ((DCBaseModel)Groupitem.FirstOrDefault()).FuncName;

                //If the functon was previously added, do nothings. 
                if (!addedFunctionList.Any(c => c.Item1 == funcName))
                {
                    if (Groupitem.ToList().Any())
                    {
                        //if it is a rectangular shape which contains multi actions.
                        if (!Groupitem.ToList().Any(c => c is DCConditionModel))
                        {
                            string actionCode = string.Empty;
                            Groupitem.ToList().ForEach((item) =>
                            {
                                actionCode += ((DCBaseModel)item).GetRenderedCode(processId, applicationId, unitOfWork) + Environment.NewLine;
                            });
                            List<object> listChildren = allNodes.Where(c => ((DCBaseModel)c).ParentShapeID.Contains(Groupitem.Key)).ToList();
                            //Adding action function to list
                            addedFunctionList.Add(new Tuple<string, string, string>(funcName, string.Join(Environment.NewLine, Groupitem.Select(c => $"//{((DCBaseModel)c).Name}")), actionCode +
                                 //Adding next function call syntax.
                                 //Each shape in diagram has one output line ,therefore there could be only one call syntax.
                                 Environment.NewLine + (MakeCallFunction(listChildren, true))));
                            RenderToChildren(addedFunctionList, listChildren, allNodes, unitOfWork, processId, applicationId, OnlyConditional, addGotoLabel);
                        }
                        else
                        {
                            DCConditionModel item = (DCConditionModel)Groupitem.FirstOrDefault();
                            if (OnlyConditional)
                                addedFunctionList.Add(new Tuple<string, string, string>(funcName, item.Name, item.GetRenderedCode(processId, applicationId, unitOfWork).TrimStringEnd(Environment.NewLine)));
                            else
                            {
                                var yesOutputList = allNodes.Where(c => ((DCBaseModel)c).ParentShapeID.Contains(item.ShapeID) && ((DCBaseModel)c).IsOutputYes == true);
                                var noOutputList = allNodes.Where(c => ((DCBaseModel)c).ParentShapeID.Contains(item.ShapeID) && ((DCBaseModel)c).IsOutputYes == false);
                                string actionCode = $@"if({item.GetRenderedCode(processId, applicationId, unitOfWork).TrimStringEnd(Environment.NewLine)}){{" +
        Environment.NewLine + $@"{MakeCallFunction(yesOutputList)}" + Environment.NewLine + "}" +
        Environment.NewLine + "else{" + Environment.NewLine + MakeCallFunction(noOutputList) + Environment.NewLine + "}";
                                addedFunctionList.Add(new Tuple<string, string, string>(funcName, item.Name, actionCode));
                                //Adding Yes output functions 
                                RenderToChildren(addedFunctionList, yesOutputList.ToList(), allNodes, unitOfWork, processId, applicationId, OnlyConditional, addGotoLabel);
                                //Adding No output functions
                                RenderToChildren(addedFunctionList, noOutputList.ToList(), allNodes, unitOfWork, processId, applicationId, OnlyConditional, addGotoLabel);
                            }
                        }
                    }

                }
            }
        }

        private static string MakeCallFunction(IEnumerable<object> items, bool addDefaultRet = true)
        {
            return (items?.Any() ?? false) ?
                $"return {((DCBaseModel)items.FirstOrDefault()).FuncName}();"
                : (addDefaultRet ? "return true;" : "");
        }

        public static string GetFunctionName(string shapeId) => string.IsNullOrWhiteSpace(shapeId) ? "" : "func" + shapeId.Replace("-", "_").Substring(0, 8);

    }
}
