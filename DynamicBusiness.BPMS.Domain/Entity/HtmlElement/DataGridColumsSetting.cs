using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [DataContract]
    [KnownType(typeof(DataGridColumsSetting))]
    public class DataGridColumsSetting
    {
        public static List<DataGridColumsSetting> ConvertTo(JArray obj)
        {
            List<DataGridColumsSetting> items = new List<DataGridColumsSetting>();
            if (obj != null)
            {
                items = obj.Select(c => new DataGridColumsSetting()
                {
                    ClassName = ((JObject)c)["className"].ToStringObj(),
                    Id = ((JObject)c)["id"].ToStringObj(),
                    Name = ((JObject)c)["name"].ToStringObj(),
                    Order = ((JObject)c)["order"].ToIntObj(),
                    SortColumn = ((JObject)c)["sortColumn"].ToStringObj(),
                    ItemList = ((JObject)c)["itemList"] != null ?
                    JArray.Parse((((JObject)c)["itemList"]).ToStringObj()).Select(d => new ColumnItemModel((JObject)d)).ToList() : new List<ColumnItemModel>(),
                    ShowInReport = ((JObject)c)["showInReport"].ToStringObj().ToLower() == "true",
                }).OrderBy(c => c.Order).ToList();
            }
            return items;
        }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string ClassName { get; set; }
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public int Order { get; set; }
        [DataMember]
        public List<ColumnItemModel> ItemList { get; set; }
        [DataMember]
        public string SortColumn { get; set; }
        [DataMember]
        public bool ShowInReport { get; set; }
    }
    [DataContract]
    [KnownType(typeof(ColumnItemModel))]
    public class ColumnItemModel
    {
        public ColumnItemModel() { }
        public ColumnItemModel(JObject obj)
        {
            this.ID = obj["id"].ToStringObj();
            this.Type = obj["type"].ToStringObj();
            this.Name = obj["name"].ToStringObj();
            this.ClassName = obj["className"].ToStringObj();
            this.FormID = obj["formId"].ToStringObj();
            this.Params = obj["params"].ToStringObj();
            this.HasConfirm = DomainUtility.toString(obj["hasConfirm"]).ToLower() == "true";
            this.ConfirmText = DomainUtility.toString(obj["confirmText"]);
            this.HasExpressionConfirm = DomainUtility.toString(obj["hasExpressionConfirm"]).ToLower() == "true";
            this.ExpressionConfirmText = DomainUtility.toString(obj["expressionConfirmText"]);
            this.ExpressionConfirmCode = DomainUtility.toString(obj["expressionConfirmCode"]);
            this.ExpressionConfirmHasFalseAction = DomainUtility.toString(obj["expressionConfirmHasFalseAction"]).ToLower() == "true";
            this.RunCodeData = DomainUtility.toString(obj["runCodeData"]);
            this.FormWidth = obj["formWidth"].ToIntObj();
            this.FormHeight = obj["formHeight"].ToIntObj();
        }
        [DataMember]
        public string ID { get; set; }
        /// <summary>
        /// is openForm,runCode,dataBound,template
        /// </summary>
        [DataMember]
        public string Type { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string ClassName { get; set; }
        [DataMember]
        public string FormID { get; set; }
        [DataMember]
        public string Params { get; set; }
        [DataMember]
        public bool HasConfirm { get; set; }
        [DataMember]
        public string ConfirmText { get; set; }
        [DataMember]
        public bool HasExpressionConfirm { get; set; }
        [DataMember]
        public string ExpressionConfirmText { get; set; }
        [DataMember]
        public string ExpressionConfirmCode { get; set; }
        [DataMember]
        public bool ExpressionConfirmHasFalseAction { get; set; }
        [DataMember]
        public string RunCodeData { get; set; }

        [DataMember]
        public int FormWidth { get; set; }

        [DataMember]
        public int FormHeight { get; set; }
    }
}
