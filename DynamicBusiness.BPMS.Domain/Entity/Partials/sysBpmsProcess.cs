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
 
        public bool AllowEdit()
        {
            return this.StatusLU == (int)sysBpmsProcess.Enum_StatusLU.Draft || this.StatusLU == (int)sysBpmsProcess.Enum_StatusLU.Inactive;
        }
        public bool AllowNextFlow()
        {
            return this.StatusLU == (int)sysBpmsProcess.Enum_StatusLU.Published || this.StatusLU == (int)sysBpmsProcess.Enum_StatusLU.OldVersion;
        }

        public sysBpmsProcess Clone()
        {
            return new sysBpmsProcess
            {
                ID = this.ID,
                FormattedNumber = this.FormattedNumber,
                Number = this.Number,
                Name = this.Name,
                Description = this.Description,
                ProcessVersion = this.ProcessVersion,
                StatusLU = this.StatusLU,
                CreatorUsername = this.CreatorUsername,
                CreateDate = this.CreateDate,
                UpdateDate = this.UpdateDate,
                DiagramXML = this.DiagramXML,
                WorkflowXML = this.WorkflowXML,
                BeginTasks = this.BeginTasks,
                ParentProcessID = this.ParentProcessID,
                PublishDate = this.PublishDate,
                ParallelCountPerUser = this.ParallelCountPerUser,
                SourceCode = this.SourceCode,
                ProcessGroupID = this.ProcessGroupID,
                TypeLU = this.TypeLU,  
            };
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
            [Description("Stop/Inactive")]
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
