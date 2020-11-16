using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using DynamicBusiness.BPMS.Domain;
namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class MessageTypeRepository : IMessageTypeRepository
    {
        private Db_BPMSEntities Context;
        public MessageTypeRepository(Db_BPMSEntities context)
        {
            this.Context = context;
        }

        public void Add(sysBpmsMessageType messageType)
        {
            messageType.ID = Guid.NewGuid();
            this.Context.sysBpmsMessageTypes.Add(messageType);
        }

        public void Update(sysBpmsMessageType messageType)
        {
            sysBpmsMessageType retVal = (from p in this.Context.sysBpmsMessageTypes
                                     where p.ID == messageType.ID
                                     select p).FirstOrDefault();
            retVal.Load(messageType);
        }

        public void Delete(Guid messageTypeId)
        {
            sysBpmsMessageType messageType = this.Context.sysBpmsMessageTypes.FirstOrDefault(d => d.ID == messageTypeId);
            if (messageType != null)
                this.Context.sysBpmsMessageTypes.Remove(messageType);
        }

        public sysBpmsMessageType GetInfo(Guid ID, string[] Includes)
        {
            return (from P in this.Context.sysBpmsMessageTypes
                    where P.ID == ID
                    select P).Include(Includes).AsNoTracking().FirstOrDefault();
        }
        public sysBpmsMessageType GetInfo(string name, string[] Includes)
        {
            return (from P in this.Context.sysBpmsMessageTypes
                    where P.Name == name
                    select P).Include(Includes).AsNoTracking().FirstOrDefault();
        }

        public List<sysBpmsMessageType> GetList(string name, bool? isActive, PagingProperties currentPaging)
        {
            name = name.ToStringObj().Trim();
            var query = this.Context.sysBpmsMessageTypes.Where(d =>
                 (name == string.Empty || d.Name.Contains(name)) &&
                 (!isActive.HasValue || d.IsActive == isActive));
            if (currentPaging != null)
            {
                currentPaging.RowsCount = query.Count();
                if (Math.Ceiling(Convert.ToDecimal(currentPaging.RowsCount) / Convert.ToDecimal(currentPaging.PageSize)) < currentPaging.PageIndex)
                    currentPaging.PageIndex = 1;

                return query.OrderBy(p => p.Name).Skip((currentPaging.PageIndex - 1) * currentPaging.PageSize).Take(currentPaging.PageSize).AsNoTracking().ToList();
            }
            else return query.OrderBy(p => p.Name).AsNoTracking().ToList();
        }
    }
}
