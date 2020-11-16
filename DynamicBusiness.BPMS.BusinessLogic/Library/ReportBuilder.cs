using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Data;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    /// <summary>
    /// Summary description for ReportBuilder
    /// </summary>

    #region Declarations
    public static class ReportGlobalParameters
    {
        //string data= Globals!PageNumber;
        public static string CurrentPageNumber = " &quot; &amp; Globals!PageNumber &amp; &quot;";
        //public static string TotalPages = "=Globals!OverallTotalPages";
        public static string TotalPages = " &quot; &amp; Globals!TotalPages";
    }
    public class ReportBuilder
    {
        public ReportPage Page { get; set; }
        public ReportBody Body { get; set; }
        public DataSet DataSource { get; set; }
        public bool AutoGenerateReport { get; set; } = true;
        public string TableHeaderColor { get; set; } = "#FFFFFF";
        public string TableFooterColor { get; set; } = "#FFFFFF";
        public string TableRowEvenColor { get; set; } = "#FFFFFF";
        public string TableRowOddColor { get; set; } = "#FFFFFF";
        public long TableRows { get; set; }
        public string FontFamily { get; set; } = "B Nazanin";
    }
    public class ReportItems
    {
        public ReportTextBoxControl[] TextBoxControls { get; set; }
        public ReportTable[] ReportTable { get; set; }
    }
    public class ReportTable
    {
        public string ReportName { get; set; }
        public ReportColumns[] ReportDataColumns { get; set; }
    }
    public class ReportColumns
    {
        public string HeaderText { get; set; }
        public ReportTextBoxControl ColumnCell { get; set; }
        public ReportDimensions HeaderColumnPadding { get; set; }
    }
    public class ReportTextBoxControl
    {
        public string Name { get; set; }
        public string[] ValueOrExpression { get; set; }
        public ReportDimensions Padding { get; set; }
        public ReportScale Size { get; set; }
    }

    public class ReportBody
    {
        public ReportItems ReportControlItems { get; set; }
    }

    public class ReportPage
    {
        public ReportScale PageSize { get; set; }
        //nadir margin

        public ReportSections ReportHeader { get; set; }
        public ReportSections ReportFooter { get; set; }
    }

    public class ReportSections
    {
        public ReportScale Size { get; set; }

        public bool PrintOnFirstPage { get; set; } = true;

        public bool PrintOnLastPage { get; set; } = true;

        public ReportItems ReportControlItems { get; set; }
    }
    public class ReportDimensions
    {
        public double Left { get; set; }
        public double Right { get; set; }
        public double Top { get; set; }
        public double Bottom { get; set; }
        public double Default { get; set; } = 2;
    }

    public class ReportScale
    {
        public double? Top { get; set; }
        public double? Left { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
    }

    #endregion Declarations
}