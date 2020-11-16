using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [DataContract]
    public class SubTypeTimerEventModel
    {
        [DataMember] 
        public int? RepeatTimes { get; set; }
        [DataMember] 
        public string MonthDays { get; set; }
        [DataMember] 
        public string WeekDays { get; set; }
        [DataMember] 
        public DateTime? DateTime { get; set; }

        /// <summary>
        /// it is used for specifying every n minutes do or after n minutes do .
        /// </summary> 
        [DataMember]
        public int? Minute { get; set; }
        [DataMember] 
        public string VariableData { get; set; }
        [DataMember] 
        public int Type { get; set; }
        [DataMember] 
        public int SetType { get; set; }
        [DataMember] 
        public int? IntervalType { get; set; }
        [DataMember] 
        public int? TimeHour { get; set; }
        [DataMember] 
        public int? TimeMinute { get; set; }
        [DataMember]
        public List<int> GetListMonthDays
        {
            get
            {
                return this.MonthDays.ToStringObj().Split(',').Where(c => !string.IsNullOrWhiteSpace(c)).Select(c => c.ToIntObj()).ToList();
            }
            set { }
        }
        [DataMember]
        public List<int> GetListWeekDays
        {
            get
            {
                return this.WeekDays.ToStringObj().Split(',').Where(c => !string.IsNullOrWhiteSpace(c)).Select(c => c.ToIntObj()).ToList();
            }
            set { }
        }
        public enum e_Type
        {
            [Description("منتظر به مدت")]
            WaitFor = 1,
            [Description("منتظر تا")]
            WaitUntil = 2,
            [Description("تکرار شود هر")]
            Interval = 3,
        }

        public enum e_IntervalType
        {
            [Description("هر هفته")]
            EveryWeek = 1,
            [Description("هر ماه")]
            EveryMonth = 2,
            [Description("بر اساس دقیقه")]
            SpecificMinute = 3,
        }

        public enum e_SetType
        {
            [Description("مقدار ثابت")]
            Static = 1,
            [Description("متغیر")]
            Variable = 2,
        }

        public void SetProperties()
        {
            if (this.Type != (int)SubTypeTimerEventModel.e_Type.Interval)
            {
                this.IntervalType = null;
                this.MonthDays =
                this.WeekDays = null;
            }
            if (this.Type == (int)SubTypeTimerEventModel.e_Type.WaitFor)
            {
                this.TimeMinute =
                this.TimeHour = null;
            }
            if (this.Type == (int)SubTypeTimerEventModel.e_Type.WaitUntil)
            {
                this.DateTime = new DateTime(this.DateTime.Value.Year, this.DateTime.Value.Month, this.DateTime.Value.Day, this.TimeHour.Value, this.TimeMinute.Value, 0);
            }
            if (this.IntervalType == (int)SubTypeTimerEventModel.e_IntervalType.EveryMonth)
                this.WeekDays = null;
            if (this.IntervalType == (int)SubTypeTimerEventModel.e_IntervalType.EveryWeek)
                this.MonthDays = null;

            if (this.SetType == (int)SubTypeTimerEventModel.e_SetType.Variable)
            {
                this.Minute = null;
                this.MonthDays =
                this.WeekDays = null;
            }
            else
                this.VariableData = null;

        }
    }
}
