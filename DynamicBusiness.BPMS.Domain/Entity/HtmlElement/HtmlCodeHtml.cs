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
    [KnownType(typeof(HtmlCodeHtml))]
    public class HtmlCodeHtml : BindingElementBase
    {
        public HtmlCodeHtml() { }
        public HtmlCodeHtml(JObject obj, HtmlElementHelperModel _helper, Guid dynamicFormId, bool isFormReadOnly) : base(obj, _helper, dynamicFormId, isFormReadOnly)
        {
            this.FillOnLoad();
        }

        private void FillOnLoad()
        {
            if (this.Helper?.FormAction == HtmlElementHelperModel.e_FormAction.Onload ||
                this.Helper?.FormAction == HtmlElementHelperModel.e_FormAction.Preview)
            {
                if (string.IsNullOrWhiteSpace(this.Parameter))
                    this.FillData();
            }
            else
                if (this.Helper?.FormAction != HtmlElementHelperModel.e_FormAction.FillMode)
                this.Value = this.Helper?.ListFormQueryModel?.FirstOrDefault(c => c.Key == this.Id)?.Value.ToStringObj() ?? "";
        }

        public override void FillData(List<QueryModel> listFormQueryModel = null)
        {
            this.Value = this.Helper.DataManageHelper?.GetValueByBinding(this.Fill, listFormQueryModel).ToStringObj();
            if (!string.IsNullOrWhiteSpace(this.Value.ToStringObj()))
                this.Label = this.Value.ToStringObj();

            MatchCollection matchsHasValue = Regex.Matches(this.Label, @"\[Has:(.*?)\]");

            for (int i = 0; i < matchsHasValue.Count; i++)
            {
                string has = matchsHasValue[i].Groups[0].ToStringObj();
                string closeHas = $"[/Has:{matchsHasValue[i].Groups[1].ToStringObj()}]";
                if (string.IsNullOrWhiteSpace(this.Helper.DataManageHelper?.GetValueByBinding(matchsHasValue[i].Groups[1].ToStringObj(), listFormQueryModel).ToStringObj()))
                {
                    if (this.Label.IndexOf(has) >= 0)
                    {
                        this.Label = this.Label.Remove(this.Label.IndexOf(has), (this.Label.LastIndexOf(closeHas) + closeHas.Length) - this.Label.IndexOf(has));
                    }
                }
                else
                {
                    if (this.Label.IndexOf(has) >= 0)
                        this.Label = this.Label.Remove(this.Label.IndexOf(has), has.Length);
                    if (this.Label.LastIndexOf(closeHas) >= 0)
                        this.Label = this.Label.Remove(this.Label.LastIndexOf(closeHas), closeHas.Length);
                }
            }

            MatchCollection matchs = Regex.Matches(this.Label, @"\[(.*?)\]");
            foreach (Match findMatch in matchs)
            {
                if (!findMatch.Groups[1].ToString().Contains("Has:"))
                {
                    string strMatch = findMatch.Groups[1].ToString();
                    if (strMatch.Split(new string[] { "::" }, StringSplitOptions.None).Count() == 2)
                        this.Label = this.Label.Replace($"[{strMatch}]", this.Helper.DataManageHelper?.GetValueByBinding(strMatch.Split(new string[] { "::" }, StringSplitOptions.None)[0], listFormQueryModel).ToNewFormat(strMatch.Split(new string[] { "::" }, StringSplitOptions.None)[1]));
                    else
                        this.Label = this.Label.ToStringObj().Replace(findMatch.Groups[0].ToString(), this.Helper.DataManageHelper.GetValueByBinding(strMatch, listFormQueryModel).ToStringObj());
                }
            }
        }
    }
}
