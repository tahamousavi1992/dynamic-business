using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [KnownType(typeof(DataGridHtml))]
    [DataContract]
    public class DataGridHtml : BindingElementBase
    {
        public DataGridHtml()
        {

        }

        public DataGridHtml(JObject obj, HtmlElementHelperModel _helper, Guid dynamicFormId, bool isFormReadOnly) : base(obj, _helper, dynamicFormId, isFormReadOnly)
        {
            this.ColumnSetting = DomainUtility.toString(obj["columnSetting"]);

            if (string.IsNullOrWhiteSpace(this.ColumnSetting))
                this.DataGridColumns = new List<DataGridColumsSetting>();
            else
                this.DataGridColumns = DataGridColumsSetting.ConvertTo(JArray.Parse(this.ColumnSetting));


            //paging
            this.HasPaging = DomainUtility.toString(obj["hasPaging"]).ToLower() == "true";
            this.PageSize = obj["pageSize"].ToIntObj() > 0 ? obj["pageSize"].ToIntObj() : 10;
            this.ShowExcel = DomainUtility.toString(obj["showExcel"]).ToLower() == "true";
            this.ShowPdf = obj["showPdf"].ToStringObj().ToLower() == "true";

            //report
            this.ReportHeader = obj["reportHeader"].ToStringObj();
            this.ReportFooter = obj["reportFooter"].ToStringObj();
            this.ReportGridHeaderColor = obj["reportGridHeaderColor"].ToStringObj();
            this.ReportGridFooterColor = obj["reportGridFooterColor"].ToStringObj();
            this.ReportGridEvenColor = obj["reportGridEvenColor"].ToStringObj();
            this.ReportGridOddColor = obj["reportGridOddColor"].ToStringObj();
            this.ReportPaperSize = obj["reportPaperSize"].ToStringObj();
            this.ReportShowDate = obj["reportShowDate"].ToStringObj().ToLower() == "true";

            //default sorting
            if (obj["sortColumn"] != null)
            {
                this.SortColumn = obj["sortColumn"].ToStringObj();
                this.SortType = !string.IsNullOrWhiteSpace(DomainUtility.toString(obj["sortType"])) ?
                (PagingProperties.e_OrderByType)Enum.Parse(typeof(PagingProperties.e_OrderByType), DomainUtility.toString(obj["sortType"]), true) : (PagingProperties.e_OrderByType.Asc);
            }
            //if this.Parameter is null ,it means that it is not dependent to other controls.
            if (string.IsNullOrWhiteSpace(this.Parameter))
            {
                this.FillData();
            }
        }

        [DataMember]
        public string ColumnSetting { get; set; }
        [DataMember]
        public List<DataGridColumsSetting> DataGridColumns { get; set; }
        [DataMember]
        public List<DataModel> Items { get; set; }
        [DataMember]
        public bool ShowPdf { get; set; }
        [DataMember]
        public bool ShowExcel { get; set; }
        [DataMember]
        public bool HasPaging { get; set; }
        [DataMember]
        public int PageSize { get; set; }
        [DataMember]
        public string SortColumn { get; set; }
        [DataMember]
        public string ReportHeader { get; set; }
        [DataMember]
        public string ReportFooter { get; set; }
        [DataMember]
        public string ReportGridHeaderColor { get; set; }
        [DataMember]
        public string ReportGridFooterColor { get; set; }
        [DataMember]
        public string ReportGridEvenColor { get; set; }
        [DataMember]
        public string ReportGridOddColor { get; set; }
        [DataMember]
        public string ReportPaperSize { get; set; }
        [DataMember]
        public bool ReportShowDate { get; set; }
        [DataMember]
        public PagingProperties.e_OrderByType? SortType { get; set; }
        [DataMember]
        public PagingProperties PagingProperties { get; set; }

        [DataMember]
        public string RenderedTemplateList { get; set; }

        public void FillDataItem(bool isReportType = false, List<QueryModel> listFormQueryModel = null)
        {
            if (this.Helper?.FormAction == HtmlElementHelperModel.e_FormAction.Onload ||
                this.Helper?.FormAction == HtmlElementHelperModel.e_FormAction.Preview)
            {
                this.PagingProperties = new PagingProperties();
                if (!string.IsNullOrWhiteSpace(this.Fill))
                {
                    if (this.HasPaging)
                    {
                        this.PagingProperties = this.PagingProperties.Update(true, this.PageSize, (this.PagingProperties.SortType ?? this.SortType.ToString()), (this.PagingProperties.SortColumn ?? this.SortColumn), this.HasPaging);
                    }
                    else
                    {
                        if (this.SortType.HasValue && !string.IsNullOrWhiteSpace(this.SortColumn))
                        {
                            this.PagingProperties.UpdateSort(this.SortType.ToString(), this.SortColumn);
                        }
                    }
                    this.Helper.DataManageHelper.ClearVariable(this.Fill);
                    if (isReportType)
                        this.Items = this.Helper.DataManageHelper.GetEntityByBinding(this.Fill, listFormQueryModel, null).Items;
                    else
                    {
                        this.Items = this.Helper.DataManageHelper.GetEntityByBinding(this.Fill, listFormQueryModel, this.PagingProperties).Items;
                        this.AddRowNumber();
                    }
                }
                this.Render();
            }
        }
        public override void FillData(List<QueryModel> listFormQueryModel = null)
        {
            this.FillDataItem(false, listFormQueryModel);
        }

        public override void SetValue(object value)
        {
            this.BindDataSource(value);
        }

        public override void BindDataSource(object value)
        {
            if (this.HasPaging)
            {
                this.PagingProperties = this.PagingProperties.Update(true, this.PageSize, (this.PagingProperties.SortType ?? this.SortType.ToString()), (this.PagingProperties.SortColumn ?? this.SortColumn), this.HasPaging);
            }
            else
            {
                if (this.SortType.HasValue && !string.IsNullOrWhiteSpace(this.SortColumn))
                {
                    this.PagingProperties.UpdateSort(this.SortType.ToString(), this.SortColumn);
                }
            }

            this.Items = (value is DataTable ? ((VariableModel)(DataTable)value) : ((VariableModel)value))?.Items ?? new List<DataModel>();

            this.AddRowNumber();
            this.Render();
        }

        private void AddRowNumber()
        {
            //adding RowNumber Token to DataList.
            int? i = this.HasPaging ? ((this.PagingProperties.PageIndex - 1) * this.PagingProperties.PageSize) : 0;
            this.Items.ForEach((c) => { c["RowNumber"] = ++i; });
        }

        private void Render()
        {
            this.RenderedTemplateList = "<thead><tr>";
            string columnTemplate = string.Empty;
            if (this.DataGridColumns != null)
            {
                foreach (var item in this.DataGridColumns.OrderBy(c => c.Order))
                {
                    columnTemplate += $"<th class=\"{item.ClassName}\" id=\"{item.Id}\">";
                    if (!string.IsNullOrWhiteSpace(item.SortColumn))
                    {
                        columnTemplate += $@"<a href='javascript:;' data-sortType='{this.PagingProperties.SortType}' data-sortColumn ='{item.SortColumn}' class='{(item.SortColumn == this.PagingProperties.SortColumn ? "active" : "")}'
                                                   onclick=""FormControl.get('{this.Id}').sort('{item.SortColumn}', '{(item.SortColumn == this.PagingProperties.SortColumn ? (this.PagingProperties.SortType == "Asc" ? "Desc" : "Asc") : "Asc")}');"" >
                                                        <span class='sk-sort-type-table'>
                                                        {item.Name}
                                                        {((item.SortColumn == this.PagingProperties.SortColumn) ? "<i class='fa fa-arrow-" + (this.PagingProperties.SortType == "Asc" ? "up" : "down") + "'></i>" : "")}
                                                    </span>
                                                </a> ";
                    }
                    else
                    {
                        columnTemplate += item.Name;
                    }
                    columnTemplate += "</th>";
                }
            }
            this.RenderedTemplateList += columnTemplate + "</tr></thead>";


            this.RenderedTemplateList += "<tbody>";
            if (this.Items != null)
                foreach (var item in this.Items)
                {
                    columnTemplate = "<tr>\n";
                    foreach (var column in this.DataGridColumns.OrderBy(c => c.Order))
                    {
                        columnTemplate += $"<td class=\"{column.ClassName}\">" + this.RenderTemplate(column.ItemList, item) + "</td>";
                    }
                    this.RenderedTemplateList += columnTemplate + "</tr>";
                }
            this.RenderedTemplateList += "</tbody>";
        }

        /// <summary>
        /// it is usually used to generate report.
        /// </summary>
        /// <param name="dataGridHtml"></param>
        /// <returns></returns>
        public System.Data.DataView MakeDataView()
        {
            System.Data.DataView dataView1 = new DataView();
            DataTable table = new DataTable("Demo");

            foreach (var item in this.DataGridColumns.Where(c => c.ShowInReport).OrderBy(c => c.Order))
            {
                table.Columns.Add(item.Name);
            }

            foreach (DataModel item in this.Items)
            {
                DataRow row1 = table.NewRow();
                foreach (var column in this.DataGridColumns.Where(c => c.ShowInReport).OrderBy(c => c.Order))
                {
                    row1[column.Name] = this.RenderTemplate(column.ItemList, item);
                }
                table.Rows.Add(row1);
            }

            dataView1.Table = table;
            return dataView1;
        }

        /// <summary>
        /// this is called from _DataGridHtml.cshtml  to make tbody tr for each record of data list.
        /// </summary>
        /// <param name="item">record of each data if null it comes from add button.</param>
        /// <returns>tr inner Html for tbody</returns>
        private string RenderTemplate(List<ColumnItemModel> cItemModelList, DataModel item)
        {
            string renderColumn = string.Empty;
            foreach (var cItemModel in cItemModelList)
            {
                string key = "";
                if (item != null && cItemModel.Type == "template")
                {
                    string template = cItemModel.Name;
                    MatchCollection matchs = Regex.Matches(template, @"\[(.*?)\]");
                    foreach (Match findMatch in matchs)
                    {
                        string strMatch = findMatch.Groups[1].ToString();
                        key = strMatch.Split(new string[] { "::" }, StringSplitOptions.None)[0];
                        if (item.ContainsKey(key))
                        {
                            if (strMatch.Split(new string[] { "::" }, StringSplitOptions.None).Count() == 2)
                            {
                                //if has format
                                template = template.Replace($"[{strMatch}]", item[key].ToFormat(strMatch));
                            }
                            else
                            {
                                if (item.ContainsKey(strMatch))
                                    template = template.Replace($"[{strMatch}]", item[strMatch].ToStringObj());
                            }
                        }
                        key = null;
                    }
                    renderColumn += template;
                }
                if (cItemModel.Type == "openForm")
                {
                    string paramValue = this.GetParameter(cItemModel.Params, item);

                    string openFormFunc = $@"FormControl.get('{this.Id}').openForm('{cItemModel.FormID}','{paramValue}',{(cItemModel.FormWidth <= 0 ? "null" : cItemModel.FormWidth.ToString())},{(cItemModel.FormHeight <= 0 ? "null" : cItemModel.FormHeight.ToString())})";
                    string onclickFunc = openFormFunc;
                    if (cItemModel.HasConfirm)
                        onclickFunc = $"FormControl.showConfirm(this,'{cItemModel.ConfirmText}',function(){{{openFormFunc}}})";
                    else
                        if (cItemModel.HasExpressionConfirm)
                        onclickFunc = $"FormControl.showExpressionConfirm(this,function(){{{openFormFunc}}})";

                    string linkCommand = $@"<a href='javascript:;' 
 data-hasExpressionConfirm='{cItemModel.HasExpressionConfirm.ToString().ToLower()}'
 data-expressionConfirmText='{cItemModel.ExpressionConfirmText}'
 id='{cItemModel.ID}'
 data-expressionConfirmParams='{paramValue}'
 data-expressionConfirmHasFalseAction='{cItemModel.ExpressionConfirmHasFalseAction.ToString().ToLower()}'
 onclick=""{onclickFunc}""
 data-command='true' class='{cItemModel.ClassName}' >{cItemModel.Name}</a>";

                    renderColumn += linkCommand;
                }

                if (cItemModel.Type == "runCode")
                {
                    string paramValue = this.GetParameter(cItemModel.Params, item);

                    string callOperationFunc = $@"FormControl.get('{this.Id}').callBusiness('{cItemModel.ID}','{paramValue}')";
                    string onclickFunc = callOperationFunc;
                    if (cItemModel.HasConfirm)
                        onclickFunc = $"FormControl.showConfirm(this,'{cItemModel.ConfirmText}',function(){{{callOperationFunc}}})";
                    else
                        if (cItemModel.HasExpressionConfirm)
                        onclickFunc = $"FormControl.showExpressionConfirm(this,function(){{{callOperationFunc}}})";

                    string linkCommand = $@"<a href='javascript:;' 
                data-hasExpressionConfirm='{cItemModel.HasExpressionConfirm.ToString().ToLower()}'
                data-expressionConfirmText='{cItemModel.ExpressionConfirmText}'
                id='{cItemModel.ID}'
                data-expressionConfirmParams='{paramValue}'
                data-expressionConfirmHasFalseAction='{cItemModel.ExpressionConfirmHasFalseAction.ToString().ToLower()}'
                onclick=""{onclickFunc}""
                data-command='true' class='{cItemModel.ClassName}' >{cItemModel.Name}</a>";

                    renderColumn += linkCommand;
                }
            }

            return renderColumn;
        }

        private string GetParameter(string parameters, DataModel item)
        {
            string paramValue = string.Empty;
            foreach (string param in parameters.Split(','))
            {
                if (!string.IsNullOrWhiteSpace(parameters))
                {
                    string value = string.Empty;
                    string setValue = param.Split(':')[2];
                    switch (param.Split(':')[1])
                    {
                        case "1"://field
                            if (item != null)
                                value = StringCipher.EncryptFormValues(item[(setValue.Trim())].ToStringObj(), Helper.ApiSessionId, Helper.IsEncrypted);
                            break;
                        case "2"://variable
                            value = StringCipher.EncryptFormValues(Helper.DataManageHelper.GetValueByBinding(setValue).ToStringObj(), Helper.ApiSessionId, Helper.IsEncrypted);
                            break;
                        case "3"://static
                            value = StringCipher.EncryptFormValues(setValue, Helper.ApiSessionId, Helper.IsEncrypted);
                            break;
                        case "4"://control : in js files it can use [textbox] to get control value.
                            value = $"[{setValue}]";
                            break;
                    }
                    paramValue += $",{param.Split(':')[0]}={value}";
                }
            }
            paramValue = paramValue.Trim(',');
            return paramValue;
        }

        public DesignCodeModel GetConfirmCode(string commandId)
        {
            var item = this.DataGridColumns.Select(c => c.ItemList.FirstOrDefault(d => d.ID == commandId)).FirstOrDefault(c => c != null);
            if (item != null)
            {
                if (item.Type == "openForm" || item.Type == "runCode")
                {
                    return DesignCodeUtility.GetDesignCodeFromXml(item.ExpressionConfirmCode.FromBase64());
                }
            }
            return null;
        }

        public DesignCodeModel GetCommandCode(string commandId)
        {
            var item = this.DataGridColumns.Select(c => c.ItemList.FirstOrDefault(d => d.ID == commandId)).FirstOrDefault(c => c != null);
            if (item != null && item.Type == "runCode")
                return DesignCodeUtility.GetDesignCodeFromXml(item.RunCodeData.FromBase64());
            return null;
        }
    }
}
