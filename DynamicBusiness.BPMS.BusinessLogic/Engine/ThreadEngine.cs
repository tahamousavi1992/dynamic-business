using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DynamicBusiness.BPMS.Domain;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class ThreadEngine : BaseEngine
    {
        public ThreadEngine(EngineSharedModel engineSharedModel, IUnitOfWork unitOfWork = null) : base(engineSharedModel, unitOfWork) { }

        public ResultOperation TerminateIfPossible(sysBpmsUser currentUser)
        {
            ResultOperation resultOperation = new ResultOperation();
            ThreadTaskService threadTaskService = new ThreadTaskService(base.UnitOfWork);
            ThreadEventService threadEventService = new ThreadEventService(base.UnitOfWork);
            List<sysBpmsThreadTask> listActiveTask = threadTaskService.GetList(this.EngineSharedModel.CurrentThreadID.Value, null, null, null).Where(c => c.StatusLU != (int)sysBpmsThreadTask.e_StatusLU.Done).ToList();
            //if terminated
            if (this.EngineSharedModel.CurrentThread.StatusLU == (int)sysBpmsThread.Enum_StatusLU.Done)
            {
                foreach (var itm in listActiveTask)
                {
                    itm.Update(DateTime.Now, (int)sysBpmsThreadTask.e_StatusLU.Done,
                    itm.OwnerUserID ?? currentUser?.ID);
                    resultOperation = threadTaskService.Update(itm);
                }
                List<sysBpmsThreadEvent> listEvents = threadEventService.GetActive(base.EngineSharedModel.CurrentThreadID.Value);
                foreach (var item in listEvents)
                {
                    item.Done();
                    resultOperation = threadEventService.Update(item);
                }
            }
            else
            {
                if (!listActiveTask.Any() && !threadEventService.GetActive(base.EngineSharedModel.CurrentThreadID.Value).Any())
                {
                    resultOperation = new ThreadService(this.UnitOfWork).DoneThread(base.EngineSharedModel.CurrentThreadID.Value);
                }
            }
            return resultOperation;
        }

        public ResultOperation Add(sysBpmsThread thread)
        {
            thread.Number = this.CalculateSerlialNumber(this.UnitOfWork.Repository<IThreadRepository>().MaxNumber() + 1);
            thread.FormattedNumber = this.CalculateFormatNumber(thread.Number.Value, DateTime.Now.Date);
            ResultOperation resultOperation = new ThreadService(base.UnitOfWork).Add(thread);
            return resultOperation;
        }

        private int CalculateSerlialNumber(int NumberOf)
        {
            int StartPoint = new SettingValueService(base.UnitOfWork).GetValue(sysBpmsSettingDef.e_NameType.ThreadStartPointSerlialNumber.ToString()).ToIntObj();
            if (NumberOf < StartPoint)
                NumberOf += StartPoint - NumberOf;
            return NumberOf;
        }

        private string CalculateFormatNumber(int NumberOf, DateTime OrderDateOf)
        {
            string SerlialNumberFormat = new SettingValueService(base.UnitOfWork).GetValue(sysBpmsSettingDef.e_NameType.ThreadFormatSerlialNumber.ToString());
            string NumberFormat = string.Empty;
            string Formated = string.Empty;

            if (SerlialNumberFormat.IndexOf("#") >= 0)
            {
                var Ar = SerlialNumberFormat.ToArray();
                foreach (var A in Ar)
                {
                    if (A == '#')
                    {
                        NumberFormat += "0";
                        SerlialNumberFormat = SerlialNumberFormat.Remove(SerlialNumberFormat.IndexOf("#"), 1);
                    }
                    else
                        if (NumberFormat.Length > 0) break;
                }
            }

            if (NumberFormat.Length > 0)
            {
                if (NumberFormat.Length > NumberOf.ToString().Length)
                {
                    Formated = NumberFormat.Substring(0, NumberFormat.Length - NumberOf.ToString().Length);
                }
            }

            Formated = Formated.ToString() + NumberOf.ToString();
            if (SerlialNumberFormat.Contains("yyyy"))
                SerlialNumberFormat = SerlialNumberFormat.Replace("yyyy", OrderDateOf.ToString("yyyy"));
            else
                if (SerlialNumberFormat.Contains("yy"))
                SerlialNumberFormat = SerlialNumberFormat.Replace("yy", OrderDateOf.ToString("yy"));
            if (SerlialNumberFormat.Contains("mm"))
                SerlialNumberFormat = SerlialNumberFormat.Replace("mm", OrderDateOf.ToString("MM"));
            else
                if (SerlialNumberFormat.Contains("m"))
                SerlialNumberFormat = SerlialNumberFormat.Replace("m", OrderDateOf.ToString("M"));
            if (SerlialNumberFormat.Contains("dd"))
                SerlialNumberFormat = SerlialNumberFormat.Replace("dd", OrderDateOf.ToString("dd"));
            else
                if (SerlialNumberFormat.Contains("d"))
                SerlialNumberFormat = SerlialNumberFormat.Replace("d", OrderDateOf.ToString("d"));

            Formated = SerlialNumberFormat + Formated;
            return Formated;
        }

    }
}
