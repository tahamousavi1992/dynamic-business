using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface IMessageCodeHelper
    {
        /// <summary>
        /// it is used to show an error message at the end to the client 
        /// </summary>
        void AddError(string message);

        /// <summary>
        /// it is used to show an info message at the end to the client 
        /// </summary>
        void AddInfo(string message);

        /// <summary>
        /// it is used to show a success message at the end to the client 
        /// </summary>
        void AddSuccess(string message);
        /// <summary>
        /// it is used to show a warning message at the end to the client 
        /// </summary>
        void AddWarning(string message);
    }
}
