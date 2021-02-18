﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [Serializable]
    public partial class sysBpmsApplicationPage
    {
        public sysBpmsApplicationPage Update(int groupLU, string description, bool showInMenu)
        {
            this.GroupLU = groupLU;
            this.Description = description;
            this.ShowInMenu = showInMenu;
            return this;
        }

       
        public enum e_OwnerTypeLU
        {
            User = 1,
            Role = 2
        }
    }
}
