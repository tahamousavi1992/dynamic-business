using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface IVariableCodeHelper
    {
        void Set(string VariableTrace, object value);

        /// <summary>
        /// save a variable and one of its properies if it has any property.
        /// </summary>
        /// <param name="variableTrace">Name of variable with its Properties.</param>
        void Save(string variableTrace, object value, params QueryModel[] queryModel);

        /// <summary>
        /// Save a Variable after add its Properies using SetVariable
        /// </summary>
        /// <param name="variableName">Name of Variable.</param>
        void Save(string variableName, params QueryModel[] queryModel);

        /// <summary>
        /// Save a Variable
        /// </summary>
        void Save(VariableModel variableModel, params QueryModel[] queryModel);

        /// <summary>
        /// get a Variable by its Name and it can call variable name alone or with its Properties name.
        /// </summary>
        /// <param name="VariableName">Name of Variable.</param>
        VariableModel Get(string variableTrace, params QueryModel[] queryModel);

        object GetValue(string variableTrace, params QueryModel[] queryModel);

        /// <summary>
        /// this clear a variable from ListSetVariable and DataManageHelperService.GetDataList
        /// </summary>
        /// <param name="variableName"></param>
        void Clear(string variableName);
    }
}
