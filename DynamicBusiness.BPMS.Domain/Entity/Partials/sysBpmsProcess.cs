using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public partial class sysBpmsProcess
    {

        public sysBpmsProcess Update(string name, string description, int? parallelCountPerUser, int typeLU)
        {
            this.Name = name;
            this.Description = description;
            this.ParallelCountPerUser = parallelCountPerUser;
            this.TypeLU = typeLU;
            return this;
        }

        public void Load(sysBpmsProcess process)
        {
            this.ID = process.ID;
            this.Number = process.Number;
            this.FormattedNumber = process.FormattedNumber;
            this.Name = process.Name;
            this.Description = process.Description;
            this.DiagramXML = process.DiagramXML;
            this.ProcessVersion = process.ProcessVersion;
            this.StatusLU = process.StatusLU;
            this.CreatorUsername = process.CreatorUsername;
            this.ParentProcessID = process.ParentProcessID;
            this.CreateDate = process.CreateDate;
            this.UpdateDate = process.UpdateDate;
            this.WorkflowXML = process.WorkflowXML;
            this.SourceCode = process.SourceCode;
            this.BeginTasks = process.BeginTasks;
            this.ParallelCountPerUser = process.ParallelCountPerUser;
            this.ProcessGroupID = process.ProcessGroupID;
            this.TypeLU = process.TypeLU;
        }

        public bool AllowEdit()
        {
            return this.StatusLU == (int)sysBpmsProcess.Enum_StatusLU.Draft || this.StatusLU == (int)sysBpmsProcess.Enum_StatusLU.Inactive;
        }
        public bool AllowNextFlow()
        {
            return this.StatusLU == (int)sysBpmsProcess.Enum_StatusLU.Published || this.StatusLU == (int)sysBpmsProcess.Enum_StatusLU.OldVersion;
        }
        public enum Enum_Version
        {
            startVersion = 1
        }

        public enum Enum_StatusLU
        {
            [Description("Draft")]
            Draft = 1,
            [Description("Published")]
            Published = 2,
            [Description("Inactive")]
            Inactive = 3,
            [Description("Old Version")]
            OldVersion = 4
        }

        public enum e_TypeLU
        {
            [Description("General")]
            General = 1,
            [Description("NotShowInList")]
            NotShowInList = 2,
            [Description("SubProcess")]
            SubProcess = 3,
        }

    }
}
