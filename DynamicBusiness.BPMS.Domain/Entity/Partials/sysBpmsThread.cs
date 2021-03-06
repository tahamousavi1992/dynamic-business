﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [Serializable]
    public partial class sysBpmsThread
    {
        public enum Enum_StatusLU
        {
            [Description("Draft")]
            Draft = 1,
            [Description("In Progress")]
            InProgress = 2,
            [Description("Done")]
            Done = 3,
            [Description("Inctive")]
            InActive = 4,
        }
        public void Update(Guid processID, Guid? userID, DateTime startDate, DateTime? endDate, string formattedNumber, int statusLU)
        {
            this.ProcessID = processID;
            this.UserID = userID;
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.FormattedNumber = formattedNumber;
            this.StatusLU = statusLU;
        }
        public void Update(int statusLU)
        {
            this.StatusLU = statusLU;
        }
        public void Update(Guid? userID)
        {
            this.UserID = userID;
        }
       
        [NotMapped]
        private List<ThreadGatewayStatusXmlModel> gatewayStatus { get; set; }
        [NotMapped]
        public List<ThreadGatewayStatusXmlModel> GatewayStatus
        {
            get
            {
                if (this.gatewayStatus == null)
                    this.gatewayStatus = this.GatewayStatusXml.ParseXML<List<ThreadGatewayStatusXmlModel>>() ?? new List<ThreadGatewayStatusXmlModel>();
                return gatewayStatus;
            }
            set
            {
                gatewayStatus = value;
            }
        }
         
        public sysBpmsThread Clone()
        {
            return new sysBpmsThread
            {
                ID = this.ID,
                StartDate = this.StartDate,
                EndDate = this.EndDate,
                StatusLU = this.StatusLU,
                GatewayStatusXml = this.GatewayStatusXml,
                ProcessID = this.ProcessID,
                UserID = this.UserID,
                FormattedNumber = this.FormattedNumber,
                Number = this.Number,
            };
        }
    }
}
