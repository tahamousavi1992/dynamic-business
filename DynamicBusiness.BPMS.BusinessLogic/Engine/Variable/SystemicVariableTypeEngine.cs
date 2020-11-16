using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class SystemicVariableTypeEngine : VariableTypeEngineBase
    {
        public SystemicVariableTypeEngine(EngineSharedModel engineSharedModel, sysBpmsVariable variable, Guid? processID, Guid? threadID, List<QueryModel> additionalParams, IUnitOfWork unitOfWork = null) : base(engineSharedModel, variable, processID, threadID, additionalParams, unitOfWork)
        {
        }

        public override List<DataModel> GetResult(PagingProperties currentPaging = null, string containerQuery = null)
        {
            List<DataModel> _DataModel = new List<DataModel>();
            switch ((sysBpmsVariable.e_VarTypeLU)Variable.VarTypeLU)
            {
                case sysBpmsVariable.e_VarTypeLU.List:
                    foreach (var item in Variable.Items)
                    {
                        DataModel _Data = new DataModel();
                        _Data[nameof(item.Key)] = item.Key;
                        _Data[nameof(item.Text)] = item.Text;
                        _DataModel.Add(_Data);
                    }
                    break;
                case sysBpmsVariable.e_VarTypeLU.Integer:
                case sysBpmsVariable.e_VarTypeLU.String:
                case sysBpmsVariable.e_VarTypeLU.Decimal:
                case sysBpmsVariable.e_VarTypeLU.Boolean:
                case sysBpmsVariable.e_VarTypeLU.Uniqueidentifier:
                    {
                        if (this.ThreadID.HasValue)
                        {
                            DataModel _Data = new DataModel();
                            sysBpmsThreadVariable _ThreadVariable = new ThreadVariableService(this.UnitOfWork).GetInfo(this.ThreadID.Value, this.Variable.ID);
                            _Data[this.Variable.Name] = _ThreadVariable != null ? _ThreadVariable.Value : null;
                            _DataModel.Add(_Data);
                        }
                    }
                    break;
                case sysBpmsVariable.e_VarTypeLU.DateTime:
                    {
                        if (this.ThreadID.HasValue)
                        {
                            DataModel _Data = new DataModel();
                            sysBpmsThreadVariable _ThreadVariable = new ThreadVariableService(this.UnitOfWork).GetInfo(this.ThreadID.Value, this.Variable.ID);
                            _Data[this.Variable.Name] = !string.IsNullOrWhiteSpace(_ThreadVariable?.Value) ? Convert.ToDateTime(_ThreadVariable.Value, new CultureInfo("en-US")) : ((DateTime?)null);
                            _DataModel.Add(_Data);
                        }
                    }
                    break;
            }
            return _DataModel;
        }

        public override ResultOperation SaveValues(DataModel _DataModel, Dictionary<string, DataModel> allSavedEntities = null)
        {
            if (this.ThreadID.HasValue)
            {
                ThreadVariableService threadVariableService = new ThreadVariableService(this.UnitOfWork);
                sysBpmsThreadVariable _ThreadVariable = threadVariableService.GetInfo(this.ThreadID.Value, this.Variable.ID);
                if (_ThreadVariable == null)
                {
                    _ThreadVariable = new sysBpmsThreadVariable()
                    {
                        ThreadID = this.ThreadID.Value,
                        VariableID = this.Variable.ID,
                        Value = this.Variable.VarTypeLU == (int)sysBpmsVariable.e_VarTypeLU.DateTime && _DataModel[this.Variable.Name] != null ?
                        Convert.ToDateTime(_DataModel[this.Variable.Name]).ToString(new CultureInfo("en-US")) : _DataModel[this.Variable.Name].ToStringObj(),
                    };
                    return threadVariableService.Add(_ThreadVariable);
                }
                else
                {
                    _ThreadVariable.ThreadID = this.ThreadID.Value;
                    _ThreadVariable.VariableID = this.Variable.ID;
                    _ThreadVariable.Value = this.Variable.VarTypeLU == (int)sysBpmsVariable.e_VarTypeLU.DateTime && _DataModel[this.Variable.Name] != null ?
                        Convert.ToDateTime(_DataModel[this.Variable.Name]).ToString(new CultureInfo("en-US")) : _DataModel[this.Variable.Name].ToStringObj();
                    return threadVariableService.Update(_ThreadVariable);
                }
            }
            else return new ResultOperation();
        }

    }
}
