using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface IEntityCodeHelper
    {
        VariableModel GetById(string entityName, Guid id);

        void DeleteById(string entityName, Guid id);

        void Save(VariableModel variableModel);

        void Save<T>(T entity);
    }
}
