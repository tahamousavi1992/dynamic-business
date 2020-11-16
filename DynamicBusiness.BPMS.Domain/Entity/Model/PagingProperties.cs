using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [DataContract]
    public class PagingProperties
    {
        public PagingProperties()
        {
            this.PageSize = 10;
            this.RowsCount = 0;
            this.PageIndex = System.Web.HttpContext.Current.Request?.QueryString["PageIndex"]?.ToIntObj() ?? 1;
            this.SortColumn = System.Web.HttpContext.Current.Request?.QueryString["SortColumn"]?.ToStringObjNull() ?? null;
            this.SortType = System.Web.HttpContext.Current.Request?.QueryString["SortType"]?.ToStringObjNull() ?? null;
        }

        public PagingProperties Update(bool autoPaging, int pageSize, string sortType, string sortColumn, bool hasPaging)
        {
            this.AutoPaging = autoPaging;
            this.PageSize = pageSize;
            this.SortType = sortType;
            this.SortColumn = sortColumn;
            this.HasPaging = hasPaging;
            return this;
        }
        public PagingProperties UpdateSort(string sortType, string sortColumn)
        {
            this.SortType = sortType;
            this.SortColumn = sortColumn;
            return this;
        }
        [DataMember]
        public int PageIndex { get; set; }
        [DataMember]
        public int RowsCount { get; set; }
        [DataMember]
        public int PageSize { get; private set; }
        
        [DataMember]
        //for UI
        public int TotalPage { get { return (RowsCount / PageSize) + ((RowsCount % PageSize) > 0 ? 1 : 0); } private set { } }
        [DataMember]
        public bool HasPreviousPage { get { return PageIndex > 1; } private set { } }
        [DataMember]
        public bool HasNextPage { get { return PageIndex < TotalPage; } private set { } }
        [DataMember]
        public int StartPage
        {
            get
            {
                return (PageIndex - (PageIndex % 10)) + 1;
            }
            private set { }
        }
        [DataMember]
        public int EndPage
        {
            get
            {
                if (StartPage + 9 > TotalPage)
                    return TotalPage;
                else
                    return StartPage + 9;
            }
            private set { }
        }
        [DataMember]
        private string _controllerName { get; set; }
        [DataMember]
        public bool HasPaging { get; set; }
        [DataMember]
        /// <summary>
        /// it is used when we want to call a java script function for sending parameter to server.
        /// this function should return a object
        /// </summary>
        public string GetParameterFunction { get; private set; }
        [DataMember]
        /// <summary>
        /// it is used for data grid html element
        /// </summary>
        public bool AutoPaging { get; set; }
        [DataMember]
        /// <summary>
        /// it is used for data grid html element
        /// </summary>
        public string SortColumn { get; set; }
        [DataMember]
        /// <summary>
        /// it is used for data grid html element
        /// </summary>
        public string SortType { get; set; }
        public enum e_OrderByType
        {
            Asc,
            Desc,
        }
    }
}
