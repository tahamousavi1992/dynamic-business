using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [DataContract]
    [KnownType(typeof(DatePickerHtml))]
    public class DatePickerHtml : BindingElementBase
    {
        public DatePickerHtml() { }

        public DatePickerHtml(JObject obj, HtmlElementHelperModel _helper, Guid dynamicFormId, bool isFormReadOnly) : base(obj, _helper, dynamicFormId, isFormReadOnly)
        {
            this.IsRequired = obj["isRequired"] != null && ((bool)obj["isRequired"]);
            this.ShowType = obj["showtype"].ToStringObj();
            this.DateFormat = obj["dateformat"].ToStringObj();
            this.FillValue();
        }

        [DataMember]
        public bool IsRequired { get; set; }
        [DataMember]
        public string ShowType { get; set; }
        [DataMember]
        public string DateFormat { get; set; }

        private void FillValue()
        {
            if (this.Helper?.FormAction == HtmlElementHelperModel.e_FormAction.Onload ||
                this.Helper?.FormAction == HtmlElementHelperModel.e_FormAction.Preview)
            {
                if (string.IsNullOrWhiteSpace(this.Parameter))
                    this.FillData();
            }
            else
                if (this.Helper?.FormAction != HtmlElementHelperModel.e_FormAction.FillMode)
                this.Value = this.Helper.ListFormQueryModel?.FirstOrDefault(c => c.Key == this.Id)?.Value.ToStringObj() ?? "";
        }


        public override void FillData(List<QueryModel> listFormQueryModel = null)
        {
            this.Value = this.Helper.DataManageHelper.GetValueByBinding<DateTime?>(this.Fill, listFormQueryModel);
        }

        private string SetDate(DateTime? Date)
        {
            return this.ConvertMiladiToMiladi(Date, this.ShowType.ToStringObj().ToLower() == "datetime");
        }

        public DateTime? ConvertMiladiToMiladi(string dateObj)
        {
            if (!string.IsNullOrWhiteSpace(dateObj))
            {
                int Hour = 0;
                int Min = 0;
                if (dateObj.Contains(":"))
                {
                    Hour = Convert.ToInt32(dateObj.Split(' ')[1].Split(':')[0]);
                    Min = Convert.ToInt32(dateObj.Split(' ')[1].Split(':')[1]);
                }
                return new DateTime(Convert.ToInt32(dateObj.Split('/')[0]), Convert.ToInt32(dateObj.Split('/')[1]), Convert.ToInt32(dateObj.Split('/')[2].Substring(0, 2)), Hour, Min, 0, 0);
            }
            else
            {
                return null;
            }
        }

        public string ConvertMiladiToMiladi(DateTime? dateObj, bool includeHour)
        {
            if (dateObj.HasValue)
            {
                return dateObj.Value.Year.ToString("0#") + "/" + dateObj.Value.Month.ToString("0#") + "/" + dateObj.Value.Day.ToString("0#") + (includeHour ? (" " + dateObj.Value.Hour + ":" + dateObj.Value.Minute) : "");
            }
            else
            {
                return null;
            }
        }

        private DateTime? GetDate(string Text)
        {
            return this.ConvertMiladiToMiladi(Text); ;
        }

    }
}
