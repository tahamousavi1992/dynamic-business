using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace DynamicBusiness.BPMS.Domain
{
    [DataContract]
    [KnownType(typeof(ChartHtml))]
    public class ChartHtml : BindingElementBase
    {
        public ChartHtml() { }

        public ChartHtml(JObject obj, HtmlElementHelperModel _helper, Guid dynamicFormId, bool isFormReadOnly) : base(obj, _helper, dynamicFormId, isFormReadOnly)
        {
            this.ChartDataSet = DomainUtility.toString(obj["chartDataSet"]);
            this.ChartLabelDataField = DomainUtility.toString(obj["chartLabelDataField"]);
            this.ChartFillListLabel = DomainUtility.toString(obj["chartFillListLabel"]);
            this.DisplayLegend = obj["displayLegend"].ToBoolObj();
            this.IsSmooth = obj["isSmooth"].ToBoolObj();
            if (!string.IsNullOrWhiteSpace(obj["chartType"].ToStringObj()))
                this.ChartType = (e_ChartType)Enum.Parse(typeof(e_ChartType), obj["chartType"].ToStringObj(), true);
            if (!string.IsNullOrWhiteSpace(obj["colorType"].ToStringObj()))
                this.ColorType = (e_ColorType)Enum.Parse(typeof(e_ColorType), obj["colorType"].ToStringObj(), true);
            this.PieColorName = obj["pieColorName"].ToStringObj();
            this.FillValue();
        }

        [DataMember]
        public string ChartDataSet { get; set; }

        /// <summary>
        /// in form it is name (Label DataField) which identify which field of Label Source Variable is for labeling.
        /// </summary>
        [DataMember]
        public string ChartLabelDataField { get; set; }

        /// <summary>
        /// in form it is name (Label Source Variable) which indicate a variable list for label data source .
        /// </summary>
        [DataMember]
        public string ChartFillListLabel { get; set; }

        [DataMember]
        public bool DisplayLegend { get; set; }

        [DataMember]
        public bool IsSmooth { get; set; }

        [DataMember]
        public e_ChartType? ChartType { get; set; }
        [DataMember]
        public string ChartTypeName { get { return this.ChartType.ToStringObj(); } set { } }
        [DataMember]
        public e_ColorType? ColorType { get; set; }

        [DataMember]
        public string PieColorName { get; set; }

        [DataMember]
        public string RenderedChart { get; set; }

        public void FillValue()
        {
            if (this.Helper?.FormAction == HtmlElementHelperModel.e_FormAction.Onload ||
                this.Helper?.FormAction == HtmlElementHelperModel.e_FormAction.Preview)
            {
                if (string.IsNullOrWhiteSpace(this.Parameter))
                {
                    this.FillData();
                }
            }
        }

        public override void FillData(List<QueryModel> listFormQueryModel = null)
        {

            List<DataModel> listLabelDS = null;
            List<string> listLabelDF = new List<string>();
            if (!string.IsNullOrWhiteSpace(this.ChartFillListLabel) && !string.IsNullOrWhiteSpace(this.ChartLabelDataField))
            {
                listLabelDS = this.Helper.DataManageHelper.GetEntityByBinding(this.ChartFillListLabel, listFormQueryModel, includes: this.GetIncludes(this.ChartLabelDataField)?.ToArray())?.Items;
                //.Replace(".", "__") is for getting relationed data like if a field is Person.Name it is returned Person__Name 
                listLabelDF = listLabelDS.Select(c => "\"" + c[this.ChartLabelDataField.Replace(".", "__")].ToFormat(this.ChartLabelDataField) + "\"").ToList();
            }

            string labels = $"[{string.Join(",", listLabelDF)}]";
            string pieBackColor = "";
            if (this.ChartType == e_ChartType.Pie)
            {
                if (this.ColorType == e_ColorType.Field)
                {
                    if (listLabelDS != null)
                    {
                        pieBackColor = $"[{string.Join(",", listLabelDS.Select(c => "\"" + c[this.PieColorName].ToStringObj() + "\"").ToList()) }]";
                    }
                }
                else
                {
                    if (this.ColorType == e_ColorType.List)
                    {
                        pieBackColor = $"[{string.Join(",", this.PieColorName.Split(',').Where(c => !string.IsNullOrWhiteSpace(c)).Select(c => $"\"#{c}\"").ToList())}]";
                    }
                    else
                    {
                        Random random = new Random();
                        pieBackColor = $"[{string.Join(",", listLabelDF.Select(d => String.Format("\"#80{0:X6}\"", random.Next(0x1000000))))}]";
                    }
                }
            }

            this.RenderedChart = $@"{{
                ""labels"": {labels},
                ""datasets"": [{string.Join(",", this.ChartDataSet.Split(',').Where(c => !string.IsNullOrWhiteSpace(c)).Select(c =>
                {
                    //c is like variableList:FieldName:Color:Label
                    //label is a text not a field.
                    string item = $@"{{
                    {(this.ChartType == e_ChartType.Pie ? "" : $"\"label\": \"{c.Split(':')[3]}\",")}
                    {(this.IsSmooth ? "\"lineTension\": \"0.000001\"," : "")}
                    ""backgroundColor"": { (this.ChartType == e_ChartType.Pie ? pieBackColor : $"color({this.HexToColor(c.Split(':')[2])}).alpha(0.5).rgbString()")},
                    ""data"": { $"[{string.Join(",", this.Helper.DataManageHelper.GetEntityByBinding(c.Split(':')[0], includes: this.GetIncludes(c.Split(':')[1])?.ToArray())?.Items.Select(d => d[c.Split(':')[1].Replace(".", "__")].ToFormat(c.Split(':')[1])).ToList())}]"}
                    { (this.ChartType == e_ChartType.Line ? ",\"fill\": false" : "")}
                }}";
                    return item;
                }).ToList())}]

            }}";

        }

        //retrieve the relationship tables according to templates that added to columns.
        private List<string> GetIncludes(string fieldName)
        {
            List<string> includes = new List<string>();
            MatchCollection matchs = Regex.Matches(fieldName, @"\[(.*?)\]");
            foreach (Match findMatch in matchs)
            {
                //get first part of format wich is fieldName  Product.Price::##00
                string key = findMatch.Groups[1].ToString().Split(new string[] { "::" }, StringSplitOptions.None)[0];
                if (key.Split('.').Count() > 1)
                    includes.Add(key);
            }
            return includes.Any() ? includes : null;
        }

        public enum e_ChartType
        {
            Line,
            Bar,
            Area,
            Pie,
            Radar,
        }

        public enum e_ColorType
        {
            Automatic,
            Field,
            List,
        }

        /// <returns>rgb(255, 99, 132)</returns>
        public string HexToColor(string hexString)
        {
            //replace # occurences
            if (hexString.IndexOf('#') != -1)
                hexString = hexString.Replace("#", "");

            int r, g, b = 0;

            r = int.Parse(hexString.Substring(0, 2), NumberStyles.AllowHexSpecifier);
            g = int.Parse(hexString.Substring(2, 2), NumberStyles.AllowHexSpecifier);
            b = int.Parse(hexString.Substring(4, 2), NumberStyles.AllowHexSpecifier);

            return $"'rgb({r}, {g}, {b})'";
        }

    }
}
