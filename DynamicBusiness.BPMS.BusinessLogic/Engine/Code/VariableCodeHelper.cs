using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class VariableCodeHelper : IVariableCodeHelper
    {
        private CodeBaseSharedModel CodeBaseShared { get; set; }

        public VariableCodeHelper(IDataManageEngine dataManageEngine,
            CodeBaseSharedModel codeBaseShared)
        {
            this.DataManageHelperService = dataManageEngine;
            this.CodeBaseShared = codeBaseShared;
        }

        private IDataManageEngine DataManageHelperService { get; set; }

        /// <summary>
        /// set a variable property into listSetVariable.
        /// </summary>
        /// <param name="VariableTrace">Name of variable with its Properties.</param>
        public void Set(string VariableTrace, object value)
        {
            string variableName = VariableTrace.Split('.').FirstOrDefault();
            if (!this.CodeBaseShared.ListSetVariable.Any(c => c.Name == variableName))
                this.CodeBaseShared.ListSetVariable.Add(new VariableModel(variableName, new DataModel()));
            this.CodeBaseShared.ListSetVariable.FirstOrDefault(c => c.Name == variableName)[VariableTrace] = value;
        }

        /// <summary>
        /// save a variable and one of its properies if it has any property.
        /// </summary>
        /// <param name="variableTrace">Name of variable with its Properties.</param>
        public void Save(string variableTrace, object value, params QueryModel[] queryModel)
        {
            DataModel dataModel = new DataModel();
            dataModel[variableTrace] = value;
            this.DataManageHelperService.SaveIntoDataBase(new VariableModel(variableTrace.Split('.')[0], dataModel), queryModel?.ToList());
        }

        /// <summary>
        /// Save a Variable after add its Properies using SetVariable
        /// </summary>
        /// <param name="variableName">Name of Variable.</param>
        public void Save(string variableName, params QueryModel[] queryModel)
        {
            variableName = variableName.Split('.').FirstOrDefault();
            foreach (var item in this.CodeBaseShared.ListSetVariable)
            {
                if (item.Name == variableName)
                {
                    this.DataManageHelperService.SaveIntoDataBase(item, queryModel?.ToList());
                }
            }
            if (this.CodeBaseShared.ListSetVariable.Any(c => c.Name == variableName))
                this.CodeBaseShared.ListSetVariable.Remove(this.CodeBaseShared.ListSetVariable.FirstOrDefault(c => c.Name == variableName));
        }

        /// <summary>
        /// Save a Variable
        /// </summary>
        public void Save(VariableModel variableModel, params QueryModel[] queryModel)
        {
            this.DataManageHelperService.SaveIntoDataBase(variableModel, queryModel?.ToList());
            if (this.CodeBaseShared.ListSetVariable.Any(c => c.Name == variableModel.Name))
                this.CodeBaseShared.ListSetVariable.Remove(this.CodeBaseShared.ListSetVariable.FirstOrDefault(c => c.Name == variableModel.Name));
        }

        /// <summary>
        /// get a Variable by its Name and it can call variable name alone or with its Properties name.
        /// </summary>
        /// <param name="VariableName">Name of Variable.</param>
        public VariableModel Get(string variableTrace, params QueryModel[] queryModel)
        {
            var data = this.DataManageHelperService.GetEntityByBinding(variableTrace, queryModel?.ToList());
            //if it was seted before, it will be get from latest value;
            string variableName = variableTrace.Split('.').FirstOrDefault();
            if (this.CodeBaseShared.ListSetVariable.Any(c => c.Name == variableTrace))
                foreach (var dataModel in this.CodeBaseShared.ListSetVariable.FirstOrDefault(c => c.Name == variableName).Items)
                    foreach (var val in dataModel.ToList())
                        data[val.Key] = val.Value;
            return data;
        }

        public object GetValue(string variableTrace, params QueryModel[] queryModel)
        {
            object data = null;
            //if it was seted before, it will be get from latest value;
            string variableName = variableTrace.Split('.').FirstOrDefault();
            if (this.CodeBaseShared.ListSetVariable.Any(c => c.Name == variableName))
                data = this.CodeBaseShared.ListSetVariable.FirstOrDefault(c => c.Name == variableName)[variableTrace];
            else
                data = this.DataManageHelperService.GetValueByBinding(variableTrace, queryModel?.ToList());
            return data;
        }

        /// <summary>
        /// this clear a variable from ListSetVariable and DataManageHelperService.GetDataList
        /// </summary>
        /// <param name="variableName"></param>
        public void Clear(string variableName)
        {
            this.DataManageHelperService.ClearVariable(variableName.Split('.').FirstOrDefault());
            if (this.CodeBaseShared.ListSetVariable.Any(c => c.Name == variableName))
                this.CodeBaseShared.ListSetVariable.Remove(this.CodeBaseShared.ListSetVariable.FirstOrDefault(c => c.Name == variableName));
        }
    }
}
