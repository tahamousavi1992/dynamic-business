using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using System.Data;
using System.Drawing;
using DynamicBusiness.BPMS.Domain;

namespace DynamicBusiness.BPMS.BusinessLogic
{

    /// <summary>
    /// Summary description for ReportBuilderEngine
    /// </summary>
    public static class ReportBuilderEngine
    {


        #region Initialize
        public static Stream GenerateReport(ReportBuilder reportBuilder)
        {
            Stream ret = new MemoryStream(Encoding.UTF8.GetBytes(GetReportData(reportBuilder)));
            return ret;
        }
        static ReportBuilder InitAutoGenerateReport(ReportBuilder reportBuilder)
        {
            if (reportBuilder != null && reportBuilder.DataSource != null && reportBuilder.DataSource.Tables.Count > 0)
            {
                DataSet ds = reportBuilder.DataSource;

                int _TablesCount = ds.Tables.Count;
                ReportTable[] reportTables = new ReportTable[_TablesCount];

                if (reportBuilder.AutoGenerateReport)
                {
                    for (int j = 0; j < _TablesCount; j++)
                    {
                        DataTable dt = ds.Tables[j];
                        ReportColumns[] columns = new ReportColumns[dt.Columns.Count];
                        ReportScale ColumnScale = new ReportScale();
                        ColumnScale.Width = ((reportBuilder.Page.PageSize.Width - 2) / dt.Columns.Count);
                        ColumnScale.Height = 1;
                        ReportDimensions ColumnPadding = new ReportDimensions();
                        ColumnPadding.Default = 2;
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            columns[i] = new ReportColumns() { ColumnCell = new ReportTextBoxControl() { Name = dt.Columns[i].ColumnName.Replace(" ", "_"), Size = ColumnScale, Padding = ColumnPadding }, HeaderText = dt.Columns[i].ColumnName, HeaderColumnPadding = ColumnPadding };
                        }

                        reportTables[j] = new ReportTable() { ReportName = dt.TableName, ReportDataColumns = columns };
                    }

                }
                reportBuilder.Body = new ReportBody();
                reportBuilder.Body.ReportControlItems = new ReportItems();
                reportBuilder.Body.ReportControlItems.ReportTable = reportTables;
            }
            return reportBuilder;
        }
        static string GetReportData(ReportBuilder reportBuilder)
        {
            reportBuilder = InitAutoGenerateReport(reportBuilder);
            string rdlcXML = "";
            rdlcXML += @"<?xml version=""1.0"" encoding=""utf-8""?> 
                        <Report xmlns=""http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition""  
                        xmlns:rd=""http://schemas.microsoft.com/SQLServer/reporting/reportdesigner""> 
                      <Body>";

            string _tableData = GenerateTable(reportBuilder);

            if (_tableData.Trim() != "")
            {
                rdlcXML += @"<ReportItems>" + _tableData + @"</ReportItems>";
            }

            rdlcXML += $@"<Height>2.1162cm</Height> 
                        <Style /> 
                      </Body> 
                      <Width>20.8cm</Width> 
                      <Page> 
                        " + GetPageHeader(reportBuilder) + GetFooter(reportBuilder) + GetReportPageSettings(reportBuilder) + @" 
                        <Style /> 
                      </Page> 
                      <AutoRefresh>0</AutoRefresh> 
                        " + GetDataSet(reportBuilder) + $@" 
                      <Language>{DomainUtility.GetCulture()}</Language> 
                      <ConsumeContainerWhitespace>true</ConsumeContainerWhitespace> 
                      <rd:ReportUnitType>Cm</rd:ReportUnitType> 
                      <rd:ReportID>17efa4a3-5c39-4892-a44b-fbde95c96585</rd:ReportID> 
                    </Report>";
            return rdlcXML;
        }
        #endregion

        #region Page Settings
        static string GetReportPageSettings(ReportBuilder reportBuilder)
        {
            return $@"<PageHeight>{reportBuilder.Page.PageSize.Height}cm</PageHeight> 
    <PageWidth>{reportBuilder.Page.PageSize.Width}cm</PageWidth> 
    <LeftMargin>0.1pt</LeftMargin> 
    <RightMargin>0.1pt</RightMargin> 
    <TopMargin>0.1pt</TopMargin> 
    <BottomMargin>0.1pt</BottomMargin> 
    <ColumnSpacing>1pt</ColumnSpacing>";
        }
        private static string GetPageHeader(ReportBuilder reportBuilder)
        {
            string strHeader = "";
            if (reportBuilder.Page == null || reportBuilder.Page.ReportHeader == null) return "";
            ReportSections reportHeader = reportBuilder.Page.ReportHeader;
            strHeader = @"<PageHeader> 
                          <Height>" + reportHeader.Size.Height.ToString() + @"in</Height> 
                          <PrintOnFirstPage>" + reportHeader.PrintOnFirstPage.ToString().ToLower() + @"</PrintOnFirstPage> 
                          <PrintOnLastPage>" + reportHeader.PrintOnLastPage.ToString().ToLower() + @"</PrintOnLastPage> 
                          <ReportItems>";
            ReportTextBoxControl[] headerTxt = reportBuilder.Page.ReportHeader.ReportControlItems.TextBoxControls;
            if (headerTxt != null)
                for (int i = 0; i < headerTxt.Count(); i++)
                {
                    strHeader += GetHeaderTextBox(reportBuilder, headerTxt[i], null);
                }
            strHeader += @" 

                          </ReportItems> 

                          <Style /> 
                        </PageHeader>";
            return strHeader;
        }
        private static string GetFooter(ReportBuilder reportBuilder)
        {
            string strFooter = "";
            if (reportBuilder.Page == null || reportBuilder.Page.ReportFooter == null) return "";
            strFooter = @"<PageFooter> 
                          <Height>0.68425in</Height> 
                          <PrintOnFirstPage>true</PrintOnFirstPage> 
                          <PrintOnLastPage>true</PrintOnLastPage> 
                          <ReportItems>";
            ReportTextBoxControl[] footerTxt = reportBuilder.Page.ReportFooter.ReportControlItems.TextBoxControls;
            if (footerTxt != null)
                for (int i = 0; i < footerTxt.Count(); i++)
                {
                    if (footerTxt[i] != null)
                    {
                        strFooter += GetFooterTextBox(reportBuilder, footerTxt[i].Name, null, footerTxt[i].ValueOrExpression);
                    }
                }
            strFooter += @"</ReportItems> 
                          <Style /> 
                        </PageFooter>";
            return strFooter;
        }
        #endregion

        #region Dataset
        static string GetDataSet(ReportBuilder reportBuilder)
        {
            string dataSetStr = "";
            if (reportBuilder != null && reportBuilder.DataSource != null && reportBuilder.DataSource.Tables.Count > 0)
            {
                string dsName = "rptCustomers";
                dataSetStr += @"<DataSources> 
    <DataSource Name=""" + dsName + @"""> 
      <ConnectionProperties> 
        <DataProvider>System.Data.DataSet</DataProvider> 
        <ConnectString>/* Local Connection */</ConnectString> 
      </ConnectionProperties> 
      <rd:DataSourceID>944b21fd-a128-4363-a5fc-312a032950a0</rd:DataSourceID> 
    </DataSource> 
  </DataSources> 
  <DataSets>"
                             + GetDataSetTables(reportBuilder.Body.ReportControlItems.ReportTable, dsName) +
                  @"</DataSets>";
            }
            return dataSetStr;
        }
        private static string GetDataSetTables(ReportTable[] tables, string DataSourceName)
        {
            string strTables = "";
            for (int i = 0; i < tables.Length; i++)
            {
                strTables += @"<DataSet Name=""" + tables[i].ReportName + @"""> 
      <Query> 
        <DataSourceName>" + DataSourceName + @"</DataSourceName> 
        <CommandText>/* Local Query */</CommandText> 
      </Query> 
     " + GetDataSetFields(tables[i].ReportDataColumns) + @" 
    </DataSet>";
            }
            return strTables;
        }
        private static string GetDataSetFields(ReportColumns[] reportColumns)
        {
            string strFields = "";

            strFields += @"<Fields>";
            for (int i = 0; i < reportColumns.Length; i++)
            {
                strFields += @"<Field Name=""" + reportColumns[i].ColumnCell.Name + @"""> 
          <DataField>" + reportColumns[i].ColumnCell.Name + @"</DataField> 
          <rd:TypeName>System.String</rd:TypeName> 
        </Field>";
            }
            strFields += @"</Fields>";
            return strFields;
        }
        #endregion

        #region Report Table Configuration
        static string GenerateTable(ReportBuilder reportBuilder)
        {
            string TableStr = "";
            if (reportBuilder != null && reportBuilder.DataSource != null && reportBuilder.DataSource.Tables.Count > 0)
            {
                ReportTable table = new ReportTable();
                for (int i = 0; i < reportBuilder.Body.ReportControlItems.ReportTable.Length; i++)
                {
                    table = reportBuilder.Body.ReportControlItems.ReportTable[i];
                    TableStr += @"<Tablix Name=""table_" + table.ReportName + @"""> 
        <TablixBody> 
          " + GetTableColumns(reportBuilder, table) + @" 
          <TablixRows> 
            " + GenerateTableHeaderRow(reportBuilder, table) + GenerateTableRow(reportBuilder, table) + GenerateTableFooterRow(reportBuilder, table) + @" 
          </TablixRows> 
        </TablixBody>" + GetTableColumnHeirarchy(reportBuilder, table) + @" 
        <TablixRowHierarchy> 
          <TablixMembers> 
            <TablixMember> 
              <KeepWithGroup>After</KeepWithGroup> 
            </TablixMember> 
            <TablixMember> 
              <Group Name=""" + table.ReportName + "_Details" + $@""" /> 
            </TablixMember> 
            <TablixMember>
              <KeepWithGroup>Before</KeepWithGroup>
            </TablixMember>
          </TablixMembers> 
        </TablixRowHierarchy> 
        <RepeatColumnHeaders>true</RepeatColumnHeaders> 
        <RepeatRowHeaders>true</RepeatRowHeaders> 
        <DataSetName>" + table.ReportName + @"</DataSetName>" + @" 
        <Top>0.07056cm</Top> 
        <Left>1cm</Left> 
        <Height>1.2cm</Height> 
        <Width>7.5cm</Width> 
        <ZIndex>1</ZIndex>
        <Style> 
          <Border> 
            <Style>None</Style> 
          </Border> 
        </Style> 
      </Tablix>";
                }
            }
            return TableStr;
        }

        static string GenerateTableRow(ReportBuilder reportBuilder, ReportTable table)
        {
            ReportColumns[] columns = table.ReportDataColumns;
            ReportTextBoxControl ColumnCell = new ReportTextBoxControl();
            ReportScale colHeight = ColumnCell.Size;
            ReportDimensions padding = new ReportDimensions();
            if (columns == null) return "";

            string strTableRow = "";
            strTableRow = @"<TablixRow> 
                <Height>0.6cm</Height> 
                <TablixCells>";
            for (int i = 0; i < columns.Length; i++)
            {
                ColumnCell = columns[i].ColumnCell;
                padding = ColumnCell.Padding;
                strTableRow += @"<TablixCell> 
                  <CellContents> 
                   " + GenerateTableRowTextBox(reportBuilder, "txtCell_" + table.ReportName + "_", ColumnCell.Name, "", true, padding) + @" 
                  </CellContents> 
                </TablixCell>";
            }
            strTableRow += @"</TablixCells></TablixRow>";
            return strTableRow;
        }
        static string GenerateTableHeaderRow(ReportBuilder reportBuilder, ReportTable table)
        {
            ReportColumns[] columns = table.ReportDataColumns;
            ReportDimensions padding = new ReportDimensions();
            if (columns == null) return "";
            string strTableRow = @"<TablixRow>
                <Height>0.6cm</Height> 
                <TablixCells>";
            for (int i = 0; i < columns.Length; i++)
            {
                padding = columns[i].HeaderColumnPadding;
                strTableRow += @"<TablixCell> 
                  <CellContents> 
                  
                   " + GenerateHeaderTableTextBox(reportBuilder, "txtHeader_" + table.ReportName + "_", columns[i].ColumnCell.Name, columns[i].HeaderText == null || columns[i].HeaderText.Trim() == "" ? columns[i].ColumnCell.Name : columns[i].HeaderText, padding) + @" 

                  </CellContents> 
                </TablixCell>";
            }
            strTableRow += @"</TablixCells></TablixRow>";
            return strTableRow;
        }
        static string GenerateTableFooterRow(ReportBuilder reportBuilder, ReportTable table)
        {
            ReportColumns[] columns = table.ReportDataColumns;
            ReportDimensions padding = new ReportDimensions();
            if (columns == null) return "";

            string strTableRow = "";
            strTableRow = @"<TablixRow>
                <Height>0.6cm</Height> 
                <TablixCells>";

            strTableRow += @"<TablixCell> 
                  <CellContents> 

                   " + GenerateFooterTableTextBox(reportBuilder, "txtFooter_" + table.ReportName + "_", "TableFooter158", $"تعداد - {reportBuilder.TableRows}", padding) + $@" 
                  <ColSpan>{columns.Length}</ColSpan>
                  </CellContents> 
                </TablixCell>{string.Join(" ", Enumerable.Repeat("<TablixCell />", (columns.Length - 1)))}";

            strTableRow += @"</TablixCells></TablixRow>";
            return strTableRow;
        }
        static string GetTableColumns(ReportBuilder reportBuilder, ReportTable table)
        {

            ReportColumns[] columns = table.ReportDataColumns;
            ReportTextBoxControl ColumnCell = new ReportTextBoxControl();

            if (columns == null) return "";

            string strColumnHeirarchy = "";
            strColumnHeirarchy = @" 
            <TablixColumns>";
            for (int i = 0; i < columns.Length; i++)
            {
                ColumnCell = columns[i].ColumnCell;

                strColumnHeirarchy += @" <TablixColumn> 
                                          <Width>" + ColumnCell.Size.Width.ToString() + @"cm</Width>  
                                        </TablixColumn>";
            }
            strColumnHeirarchy += @"</TablixColumns>";
            return strColumnHeirarchy;
        }
        static string GetTableColumnHeirarchy(ReportBuilder reportBuilder, ReportTable table)
        {
            ReportColumns[] columns = table.ReportDataColumns;
            if (columns == null) return "";

            string strColumnHeirarchy = "";
            strColumnHeirarchy = @" 
            <TablixColumnHierarchy> 
          <TablixMembers>";
            for (int i = 0; i < columns.Length; i++)
            {
                strColumnHeirarchy += "<TablixMember />";
            }
            strColumnHeirarchy += @"</TablixMembers> 
        </TablixColumnHierarchy>";
            return strColumnHeirarchy;
        }
        #endregion

        #region Report TextBox
        /// <summary>
        /// Generate Table's Row Textbox.
        /// </summary>
        static string GenerateTableRowTextBox(ReportBuilder reportBuilder, string strControlIDPrefix, string strName, string strValueOrExpression = "", bool isFieldValue = true, ReportDimensions padding = null)
        {
            string strTextBox = "";
            strTextBox = @" <Textbox Name=""" + strControlIDPrefix + strName.Replace(" ", "_") + @"""> 
                      <CanGrow>true</CanGrow> 
                      <KeepTogether>true</KeepTogether> 
                      <Paragraphs> 
                        <Paragraph> 
                          <TextRuns> 
                            <TextRun>";
            if (isFieldValue) strTextBox += @"<Value>=Fields!" + strName + @".Value</Value>";
            else strTextBox += @"<Value>" + strValueOrExpression + "</Value>";
            strTextBox += $@"<Style><FontFamily>{reportBuilder.FontFamily}</FontFamily></Style> 
                            </TextRun> 
                          </TextRuns> 
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph> 
                      </Paragraphs> 
                      <rd:DefaultName>" + strControlIDPrefix + strName + $@"</rd:DefaultName> 
                      <Style> 
                       <BackgroundColor>=iif(RowNumber(Nothing) Mod 2 = 0,""{reportBuilder.TableRowEvenColor}"",""{reportBuilder.TableRowOddColor}"")</BackgroundColor>
                        <Border> 
                          <Color>LightGrey</Color> 
                          <Style>Solid</Style> 
                        </Border>" + GetDimensions(padding) + @"</Style> 
                    </Textbox>";
            return strTextBox;
        }
        static string GenerateHeaderTableTextBox(ReportBuilder reportBuilder, string strControlIDPrefix, string name, string strValueOrExpression = "", ReportDimensions padding = null)
        {
            string strTextBox = "";
            strTextBox = $@" <Textbox Name=""" + strControlIDPrefix + name + @"""> 
                      <CanGrow>true</CanGrow> 
                      <KeepTogether>true</KeepTogether> 
                      <Paragraphs> 
                        <Paragraph> 
                          <TextRuns> 
                            <TextRun>";
            strTextBox += @"<Value>" + strValueOrExpression + "</Value>";
            strTextBox += $@"<Style><FontFamily>{reportBuilder.FontFamily}</FontFamily></Style>
                            </TextRun> 
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph> 
                      </Paragraphs> 
                      <rd:DefaultName>" + strControlIDPrefix + name + $@"</rd:DefaultName> 
                      <Style> 
   <BackgroundColor>{reportBuilder.TableHeaderColor}</BackgroundColor>
<FontFamily>{reportBuilder.FontFamily}</FontFamily>
<FontWeight>Bold</FontWeight>
                        <Border> 
                          <Color>LightGrey</Color> 
                          <Style>Solid</Style> 
                        </Border>" + GetDimensions(padding) + @"</Style> 
                    </Textbox>";
            return strTextBox;
        }
        static string GenerateFooterTableTextBox(ReportBuilder reportBuilder, string strControlIDPrefix, string name, string strValueOrExpression = "", ReportDimensions padding = null)
        {
            string strTextBox = "";
            strTextBox = $@" <Textbox Name=""" + strControlIDPrefix + name + @"""> 
                      <CanGrow>true</CanGrow> 
                      <KeepTogether>true</KeepTogether> 
                      <Paragraphs> 
                        <Paragraph> 
                          <TextRuns> 
                            <TextRun>";
            strTextBox += @"<Value>" + strValueOrExpression + "</Value>";
            strTextBox += $@"<Style><FontFamily>{reportBuilder.FontFamily}</FontFamily></Style> 
                            </TextRun> 
                          </TextRuns>
                          <Style>
                            <TextAlign>Center</TextAlign>
                          </Style>
                        </Paragraph> 
                      </Paragraphs> 
                      <rd:DefaultName>" + strControlIDPrefix + name + $@"</rd:DefaultName> 
                      <Style> 
   <BackgroundColor>{reportBuilder.TableFooterColor}</BackgroundColor>
<FontFamily>{reportBuilder.FontFamily}</FontFamily>
<FontWeight>Bold</FontWeight>
                        <Border> 
                          <Color>LightGrey</Color> 
                          <Style>Solid</Style> 
                        </Border>" + GetDimensions(padding) + @"</Style> 
                    </Textbox>";
            return strTextBox;
        }
        static string GetHeaderTextBox(ReportBuilder reportBuilder, ReportTextBoxControl reportHeader, ReportDimensions padding = null)
        {
            string strTextBox = "";
            strTextBox = @" <Textbox Name=""" + reportHeader.Name + @"""> 
          <CanGrow>true</CanGrow> 
          <KeepTogether>true</KeepTogether> 
          <Paragraphs> 
            <Paragraph> 
              <TextRuns>";

            for (int i = 0; i < reportHeader.ValueOrExpression.Length; i++)
            {
                strTextBox += GetHeaderTextRun(reportBuilder, reportHeader.ValueOrExpression[i].ToString());
            }

            strTextBox += @"</TextRuns> 
              <Style>
                <TextAlign>Center</TextAlign>
              </Style> 
            </Paragraph> 
          </Paragraphs> 
          <rd:DefaultName>" + reportHeader.Name + $@"</rd:DefaultName> 
          <Top>{reportHeader.Size.Top}cm</Top> 
          <Left>{reportHeader.Size.Left}cm</Left> 
          <Height>{reportHeader.Size.Height}cm</Height> 
          <Width>{reportHeader.Size.Width}cm</Width> 
          <ZIndex>2</ZIndex> 
          <Style> 
            <Border> 
              <Style>None</Style> 
            </Border>";

            strTextBox += GetDimensions(padding) + @"</Style> 
        </Textbox>";
            return strTextBox;
        }
        static string GetFooterTextBox(ReportBuilder reportBuilder, string textBoxName, ReportDimensions padding = null, params string[] strValues)
        {

            string strTextBox = "";
            strTextBox = @" <Textbox Name=""" + textBoxName + @"""> 
          <CanGrow>true</CanGrow> 
          <KeepTogether>true</KeepTogether> 
          <Paragraphs> 
            <Paragraph> 
              <TextRuns>";

            for (int i = 0; i < strValues.Length; i++)
            {
                strTextBox += GetTextRun_fot(strValues[i].ToString());
            }

            strTextBox += $@"</TextRuns> 
                <Style>
                <TextAlign>Center</TextAlign>
              </Style>
            </Paragraph> 
          </Paragraphs> 
          <rd:DefaultName>" + textBoxName + $@"</rd:DefaultName> 
          <Top>1.0884cm</Top> 
          <Left>1cm</Left> 
          <Height>0.6cm</Height> 
          <Width>{reportBuilder.Page.PageSize.Width}cm</Width> 
          <ZIndex>2</ZIndex> 
          <Style> 
            <Border> 
              <Style>None</Style> 
            </Border>";

            strTextBox += GetDimensions(padding) + @"</Style> 
        </Textbox>";
            return strTextBox;
        }

        static string GetTextRun_fot(string ValueOrExpression)
        {
            //<Value>=""Page "" &amp; Globals!PageNumber &amp; "" of "" &amp; Globals!TotalPages</Value> 
            return "<TextRun>"
                      + "<Value>=&quot;" + ValueOrExpression + "</Value>"
                      + "<Style>"
                        + "<FontSize>8pt</FontSize>"
                      + "</Style>"
                    + "</TextRun>";
        }


        static string GetTextRun(string ValueOrExpression)
        {
            return @"<TextRun> 
                  <Value>" + ValueOrExpression + @"</Value> 
                  <Style> 
                    <FontSize>8pt</FontSize> 
                  </Style> 
                </TextRun>";
        }

        static string GetHeaderTextRun(ReportBuilder reportBuilder, string ValueOrExpression)
        {
            return $@"<TextRun> 
                  <Value>" + ValueOrExpression + $@"</Value> 
                  <Style> 
                    <FontSize>10pt</FontSize> 
                    <FontFamily>{reportBuilder.FontFamily}</FontFamily>
                    <FontWeight>Bold</FontWeight>
                  </Style> 
                </TextRun>";
        }
        #endregion

        #region Images
        static void GenerateReportImage(ReportBuilder reportBuilder)
        {
        }
        #endregion

        #region Settings
        private static string GetDimensions(ReportDimensions padding = null)
        {
            string strDimensions = "";
            if (padding != null)
            {
                if (padding.Default == 0)
                {
                    strDimensions += string.Format("<PaddingLeft>{0}pt</PaddingLeft>", padding.Left);
                    strDimensions += string.Format("<PaddingRight>{0}pt</PaddingRight>", padding.Right);
                    strDimensions += string.Format("<PaddingTop>{0}pt</PaddingTop>", padding.Top);
                    strDimensions += string.Format("<PaddingBottom>{0}pt</PaddingBottom>", padding.Bottom);
                }
                else
                {
                    strDimensions += string.Format("<PaddingLeft>{0}pt</PaddingLeft>", padding.Default);
                    strDimensions += string.Format("<PaddingRight>{0}pt</PaddingRight>", padding.Default);
                    strDimensions += string.Format("<PaddingTop>{0}pt</PaddingTop>", padding.Default);
                    strDimensions += string.Format("<PaddingBottom>{0}pt</PaddingBottom>", padding.Default);
                }
            }
            return strDimensions;
        }
        #endregion

    }
}