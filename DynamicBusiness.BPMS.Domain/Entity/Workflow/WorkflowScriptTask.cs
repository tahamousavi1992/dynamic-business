﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [Serializable]
    [NodeAttribute("scriptTask")]
    public class WorkflowScriptTask : WorkflowIOElement
    {
        public WorkflowScriptTask(string id, string name, List<string> outgoings, List<string> incomings, sysBpmsTask.e_MarkerTypeLU? e_MarkerTypeLU)
            : base(id, name, outgoings, incomings)
        {
            this.MarkerTypeLU = e_MarkerTypeLU;
        }
        public sysBpmsTask.e_MarkerTypeLU? MarkerTypeLU { get; set; }
    }
}
