using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [Serializable]
    public partial class sysBpmsEvent
    {
        public ResultOperation Update(int typeLU, string refElementID, string ConfigurationXML, int? subType, bool? cancelActivity, Guid? messageTypeID)
        {
            this.TypeLU = typeLU;
            this.ConfigurationXML = ConfigurationXML;
            this.SubTypeMessageEventModel = subTypeMessageEventModel;
            this.SubType = subType;
            this.RefElementID = refElementID;
            this.CancelActivity = cancelActivity;
            this.MessageTypeID = messageTypeID;
            ResultOperation resultOperation = new ResultOperation(this);
            switch ((sysBpmsEvent.e_TypeLU)this.TypeLU)
            {
                case sysBpmsEvent.e_TypeLU.IntermediateThrow:
                    if (this.SubTypeMessageEventModel.Type == (int)SubTypeMessageEventModel.e_Type.Email)
                    {
                        if (!string.IsNullOrWhiteSpace(
    this.SubTypeMessageEventModel.Email?.From +
    this.SubTypeMessageEventModel.Email?.To +
    this.SubTypeMessageEventModel.Email?.Content))
                        {
                            if (string.IsNullOrWhiteSpace(this.SubTypeMessageEventModel.Email.From))
                                resultOperation.AddError(SharedLang.GetReuired(nameof(SubTypeEmailEventModel.From), nameof(SubTypeEmailEventModel)));
                            if (string.IsNullOrWhiteSpace(this.SubTypeMessageEventModel.Email.To))
                                resultOperation.AddError(SharedLang.GetReuired(nameof(SubTypeEmailEventModel.To), nameof(SubTypeEmailEventModel)));
                            if (string.IsNullOrWhiteSpace(this.SubTypeMessageEventModel.Email.Content))
                                resultOperation.AddError(SharedLang.GetReuired(nameof(SubTypeEmailEventModel.Content), nameof(SubTypeEmailEventModel)));
                        }
                    }
                    else
                    {

                        if (string.IsNullOrWhiteSpace(this.SubTypeMessageEventModel.Key))
                            resultOperation.AddError(SharedLang.GetReuired(nameof(SubTypeMessageEventModel.Key), nameof(Domain.SubTypeMessageEventModel)));
                        if (!this.MessageTypeID.HasValue)
                            resultOperation.AddError(SharedLang.GetReuired(nameof(sysBpmsEvent.MessageTypeID), nameof(sysBpmsEvent)));
                    }
                    break;
                default:
                    break;
            }
            return resultOperation;
        }

        public void Load(sysBpmsEvent Event)
        {
            this.ID = Event.ID;
            this.ProcessID = Event.ProcessID;
            this.ElementID = Event.ElementID;
            this.TypeLU = Event.TypeLU;
            this.ConfigurationXML = Event.ConfigurationXML;
            this.SubType = Event.SubType;
            this.RefElementID = Event.RefElementID;
            this.CancelActivity = Event.CancelActivity;
            this.MessageTypeID = Event.MessageTypeID;
        }

        public enum e_TypeLU
        {
            StartEvent = 1,
            EndEvent = 2,
            IntermediateThrow = 3,
            boundary = 4,
            IntermediateCatch = 5,
        }

        private SubTypeMessageEventModel subTypeMessageEventModel { get; set; }
        public SubTypeMessageEventModel SubTypeMessageEventModel
        {
            get
            {
                if (this.subTypeMessageEventModel == null)
                    this.subTypeMessageEventModel = this.ConfigurationXML.ParseXML<SubTypeMessageEventModel>() ?? new SubTypeMessageEventModel();
                return subTypeMessageEventModel;
            }
            set
            {
                subTypeMessageEventModel = value;
            }
        }

        private SubTypeTimerEventModel subTypeTimerEventModel { get; set; }
        public SubTypeTimerEventModel SubTypeTimerEventModel
        {
            get
            {
                if (this.subTypeTimerEventModel == null)
                    this.subTypeTimerEventModel = this.ConfigurationXML.ParseXML<SubTypeTimerEventModel>() ?? new SubTypeTimerEventModel();
                return subTypeTimerEventModel;
            }
            set
            {
                subTypeTimerEventModel = value;
            }
        }
    }
}
