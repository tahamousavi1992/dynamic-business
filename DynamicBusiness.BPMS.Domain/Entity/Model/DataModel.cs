using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [DataContract]
    public class DataModel
    {
        [DataMember]
        /// <summary>
        /// each column of rows add to this like FirstName column with value of type string
        /// </summary>
        private Dictionary<string, object> items;


        public DataModel(Dictionary<string, object> data)
        {
            items = data;
        }

        public DataModel()
        {
            items = new Dictionary<string, object>();
        }

        public DataModel(DataRow _row, string ColumnName = "")
        {
            items = new Dictionary<string, object>();
            foreach (DataColumn _Column in _row.Table.Columns)
            {
                if (string.IsNullOrWhiteSpace(ColumnName))
                {
                    items.Add(_Column.ColumnName, _row[_Column.ColumnName] == DBNull.Value ? null : _row[_Column.ColumnName]);
                }
                else
                {
                    if (!items.ContainsKey(ColumnName))
                        items.Add(ColumnName, _row[_Column.ColumnName] == DBNull.Value ? null : _row[_Column.ColumnName]);
                }
            }
        }

        public object Value { get { return this.items.Any() ? this.items.FirstOrDefault().Value : null; } }
        public T GetValue<T>(string name)
        {
            //In format we use :: for showing formats.
            name = name.Split(new string[] { "::" }, StringSplitOptions.None).FirstOrDefault();
            if (items.Count == 1 && !items.ContainsKey(name))
                return (T)Convert.ChangeType(this.Value, Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T));
            if (items[name] == null)
                return default(T);
            return (T)Convert.ChangeType(items[name], Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T));
        }

        public void SetValue(string name, object value)
        {
            items[name] = value;
        }

        public bool ContainsKey(string name)
        {
            return items.ContainsKey(name);
        }
        [DataMember]
        public object this[string name]
        {
            get
            {
                //In format we use :: for showing formats.
                name = name.Split(new string[] { "::" }, StringSplitOptions.None).FirstOrDefault();
                return this.ContainsKey(name) ? items[name] : items.Count == 1 ? items.FirstOrDefault().Value : null;
            }
            set
            {
                items[name] = value;
            }
        }

        public Dictionary<string, object> ToList()
        {
            return this.items;
        }

        public override string ToString()
        {
            return this?.items?.FirstOrDefault().ToStringObj() ?? ""; ;
        }

        public static implicit operator string(DataModel dataModel)
        {
            return dataModel?.items?.FirstOrDefault().ToStringObj() ?? "";
        }

    }
}
