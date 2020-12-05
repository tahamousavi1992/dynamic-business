using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicBusiness.BPMS.Domain;
using Newtonsoft.Json.Linq;
using System.Web;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using System.IO;
using Microsoft.Reporting.WinForms;
using Microsoft.Reporting.WebForms;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class ReportEngine : BaseEngine
    {
        public ReportEngine(EngineSharedModel engineSharedModel, IUnitOfWork unitOfWork = null) : base(engineSharedModel, unitOfWork)
        {

        }

        public void PrintRdlcDataGrid(HttpResponse httpResponse, DataGridHtml dataGridHtml, DomainUtility.ReportExportType reportExportType)
        {

            DataView dataView = dataGridHtml.MakeDataView();
            //Report Viewer, Builder and Engine

            ReportViewer reportViewer = new ReportViewer();
            reportViewer.Reset();

            DataSet DS = new DataSet();
            DS.Tables.Add(dataView.ToTable());

            ReportBuilder reportBuilder = new ReportBuilder();
            reportBuilder.DataSource = DS;
            reportBuilder.Page = new ReportPage();
            reportBuilder.TableRows = dataView.Table.Rows.Count;

            string fontFamily = new SettingValueService(base.UnitOfWork).GetValue(sysBpmsSettingDef.e_NameType.DefaultReportFontFamily.ToString());
            if (!string.IsNullOrWhiteSpace(fontFamily))
                reportBuilder.FontFamily = fontFamily;

            if (!string.IsNullOrWhiteSpace(dataGridHtml.ReportGridHeaderColor))
                reportBuilder.TableHeaderColor = "#" + dataGridHtml.ReportGridHeaderColor.ToStringObj();
            if (!string.IsNullOrWhiteSpace(dataGridHtml.ReportGridFooterColor))
                reportBuilder.TableFooterColor = "#" + dataGridHtml.ReportGridFooterColor.ToStringObj();
            if (!string.IsNullOrWhiteSpace(dataGridHtml.ReportGridEvenColor))
                reportBuilder.TableRowEvenColor = "#" + dataGridHtml.ReportGridEvenColor.ToStringObj();
            if (!string.IsNullOrWhiteSpace(dataGridHtml.ReportGridOddColor))
                reportBuilder.TableRowOddColor = "#" + dataGridHtml.ReportGridOddColor.ToStringObj();

            //report size
            switch (dataGridHtml.ReportPaperSize)
            {
                case "A2":
                    reportBuilder.Page.PageSize = new ReportScale() { Width = 59.4, Height = 42 };
                    break;
                case "A3":
                    reportBuilder.Page.PageSize = new ReportScale() { Width = 42, Height = 29.7 };
                    break;
                case "A4":
                    reportBuilder.Page.PageSize = new ReportScale() { Width = 29.7, Height = 21 };
                    break;
                case "A5":
                    reportBuilder.Page.PageSize = new ReportScale() { Width = 21, Height = 14.8 };
                    break;
                default:
                    reportBuilder.Page.PageSize = new ReportScale() { Width = 29.7, Height = 21 };
                    break;
            }

            ReportSections reportFooter = new ReportSections();
            ReportItems reportFooterItems = new ReportItems();
            ReportTextBoxControl[] footerTxt = new ReportTextBoxControl[3];
            string footer = string.IsNullOrWhiteSpace(dataGridHtml.ReportFooter) ?
                $" page {ReportGlobalParameters.CurrentPageNumber} of {ReportGlobalParameters.TotalPages}" : dataGridHtml.ReportFooter;
            footerTxt[0] = new ReportTextBoxControl()
            { Name = "txtCopyright", ValueOrExpression = new string[] { footer } };
            reportFooterItems.TextBoxControls = footerTxt;
            reportFooter.ReportControlItems = reportFooterItems;
            reportBuilder.Page.ReportFooter = reportFooter;

            ReportSections reportHeader = new ReportSections();
            reportHeader.Size = new ReportScale();
            reportHeader.Size.Height = 0.56849;

            ReportItems reportHeaderItems = new ReportItems();
            List<ReportTextBoxControl> headerTxt = new List<ReportTextBoxControl>() { };
            headerTxt.Add(new ReportTextBoxControl()
            {
                Name = "txtReportTitle",
                Size = new ReportScale() { Height = 0.6, Width = 5, Left = (reportBuilder.Page.PageSize.Width - 5) / 2, Top = 0.5 },
                ValueOrExpression = new string[] { dataGridHtml.Label }
            });
            if (dataGridHtml.ReportShowDate)
            {
                headerTxt.Add(new ReportTextBoxControl()
                {
                    Name = "txtReportDateTitle",
                    Size = new ReportScale() { Height = 0.6, Width = 2, Left = 0.5, Top = 1 },
                    ValueOrExpression = new string[] { DateTime.Now.ToString("yyyy/MM/dd") }
                });
            }
            reportHeaderItems.TextBoxControls = headerTxt.ToArray();
            reportHeader.ReportControlItems = reportHeaderItems;
            reportBuilder.Page.ReportHeader = reportHeader;
            reportViewer.LocalReport.LoadReportDefinition(ReportBuilderEngine.GenerateReport(reportBuilder));


            dataView.Table.Columns.Cast<DataColumn>().ToList().ForEach(c => c.ColumnName = c.ColumnName.Replace(" ", "_"));

            reportViewer.LocalReport.DataSources.Add(
               new ReportDataSource(dataView.Table.TableName, dataView.Table));

            reportViewer.LocalReport.DisplayName = "WastageReport";


            Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string filenameExtension;
            byte[] bytes = reportViewer.LocalReport.Render(reportExportType.ToString(), null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);
            string caption = string.IsNullOrWhiteSpace(dataGridHtml.Label) ? new DynamicFormService(base.UnitOfWork).GetInfo(dataGridHtml.DynamicFormID).Name : dataGridHtml.Label;
            this.Response(httpResponse, bytes, reportExportType, caption, mimeType);
        }

        private void Response(HttpResponse httpResponse, byte[] bytes, DomainUtility.ReportExportType reportExportType, string caption, string mimeType)
        {
            httpResponse.Buffer = true;
            httpResponse.Clear();
            if (reportExportType == DomainUtility.ReportExportType.PDF)
            {
                httpResponse.AddHeader("content-disposition", "attachment; filename=" + caption + "." + "pdf");
                httpResponse.ContentType = mimeType;
            }
            if (reportExportType == DomainUtility.ReportExportType.Excel)
            {
                httpResponse.AddHeader("content-disposition", "attachment; filename=" + caption + "." + "xls");
                httpResponse.ContentType = mimeType;
            }

            httpResponse.BinaryWrite(bytes); // create the file
            httpResponse.Flush(); // send it to the client to download
        }

    }
}
