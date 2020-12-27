using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface IDataManageEngine
    {
        EngineSharedModel GetSharedModel();
        /// <summary>
        /// It is used for Forms inside forms to change appPageID of parent form for retrieving variables.
        /// </summary>
        void SetApplicationPageID(Guid appPageID); 
        Guid? ThreadID { get; set; }
        Dictionary<string, object> FormControlValues { get; set; }
        /// <param name="containerQuery">It is generally used in combosearch which add a parent query that filter table's rows according to query parameter and text field</param>
        VariableModel GetEntityByBinding(string BindTrace, List<QueryModel> listFormQueryModel = null, PagingProperties currentPaging = null, string containerQuery = null, string[] includes = null);
        object GetValueByBinding(string BindTrace, List<QueryModel> listFormQueryModel = null);
        /// <summary>
        /// it is used when we are getting  
        /// </summary>
        T GetValueByBinding<T>(string BindTrace, List<QueryModel> listFormQueryModel = null);
        /// <summary>
        /// for calling after form post
        /// </summary>
        /// <param name="setExternalVariable">if in code a user set variable but dont save that these variables save in this part of code</param>
        /// <returns></returns>
        ResultOperation SaveIntoDataBase(ContentHtml contentHtml, sysBpmsThreadTask threadTasks, List<VariableModel> setExternalVariable);
        void SaveIntoDataBase(VariableModel variableModel, List<QueryModel> listFormQueryModel);
        /// <summary>
        /// it is used by Combo search to get name of entity using text/value field 
        /// </summary>
        VariableModel GetEntityWithKeyValue(string variableName, Dictionary<string, object> dictionary);

        void ClearVariable(string varName);
    }
}
