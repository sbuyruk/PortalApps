using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
namespace Utility.HelperClasses
{
    public static class ExcelHelper
    {
        public static DataTable ReadXLSXAsDataTable(Stream file, bool hasTitle)
        {
            DataTable dataTable = new DataTable();
            using (SpreadsheetDocument spreadSheetDocument = SpreadsheetDocument.Open(file, false))
            {
                dataTable = ConvertToDatetable(spreadSheetDocument, hasTitle);

            }
            if (hasTitle) //il satır veri içermiyor. başlık içeriyorsa
            {
                dataTable.Rows.RemoveAt(0);
            }

            return dataTable;
        }

        public static DataTable ReadXLSXAsDataTable(Stream file, int ilkKacSatirHaric, int sonKacSatirHaric, bool hasTitle)
        {
            DataTable dataTable = new DataTable();
            using (SpreadsheetDocument spreadSheetDocument = SpreadsheetDocument.Open(file, false))
            {
                dataTable = ConvertToDatetable(spreadSheetDocument, ilkKacSatirHaric, sonKacSatirHaric, hasTitle);

            }


            return dataTable;
        }
        public static DataTable ReadXLSXAsDataTableKartIle(Stream file, int ilkKacSatirHaric, int sonKacSatirHaric, bool hasTitle)
        {
            DataTable dataTable = new DataTable();
            using (SpreadsheetDocument spreadSheetDocument = SpreadsheetDocument.Open(file, false))
            {
                dataTable = ConvertToDatetableKartIle(spreadSheetDocument, ilkKacSatirHaric, sonKacSatirHaric);

            }
            if (hasTitle) //il satır veri içermiyor. başlık içeriyorsa
            {
                dataTable.Rows.RemoveAt(0);
            }

            return dataTable;
        }
        public static DataTable ReadXLSXAsDataTable(string filePath, bool hasTitle)
        {
            DataTable dataTable = new DataTable();
            using (SpreadsheetDocument spreadSheetDocument = SpreadsheetDocument.Open(filePath, false))
            {
                dataTable = ConvertToDatetable(spreadSheetDocument, hasTitle);
            }
            if (hasTitle) //il satır veri içermiyor. başlık içeriyorsa
            {
                dataTable.Rows.RemoveAt(0);
            }

            return dataTable;
        }
        private static DataTable ConvertToDatetable(SpreadsheetDocument spreadSheetDocument, bool hasTitle)
        {
            DataTable dataTable = new DataTable();

            WorkbookPart workbookPart = spreadSheetDocument.WorkbookPart;
            IEnumerable<Sheet> sheets = spreadSheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
            string relationshipId = sheets.First().Id.Value;
            WorksheetPart worksheetPart = (WorksheetPart)spreadSheetDocument.WorkbookPart.GetPartById(relationshipId);
            Worksheet workSheet = worksheetPart.Worksheet;
            SheetData sheetData = workSheet.GetFirstChild<SheetData>();
            IEnumerable<Row> rows = sheetData.Descendants<Row>();

            int count = 0;
            foreach (Cell cell in rows.ElementAt(0))
            {
                if (hasTitle) //il satır veri içermiyor. başlık içeriyorsa
                {
                    dataTable.Columns.Add(GetCellValue(spreadSheetDocument, cell));
                }
                else
                {
                    dataTable.Columns.Add("Column_" + count);
                }
                count++;
            }

            foreach (Row row in rows)
            {
                DataRow dataRow = dataTable.NewRow();
                for (int i = 0; i < row.Descendants<Cell>().Count(); i++)
                {
                    dataRow[i] = GetCellValue(spreadSheetDocument, row.Descendants<Cell>().ElementAt(i));
                    //SB boş hücreyi atlama sorununu düzeltmek için aşağısı eklendi, üstteki satır kaldırıldı
                    //Cell cell = row.Descendants<Cell>().ElementAt(i);
                    //int actualCellIndex = CellReferenceToIndex(cell);
                    //dataRow[actualCellIndex] = GetCellValue(spreadSheetDocument, cell);
                }

                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }
        private static int CellReferenceToIndex(Cell cell)
        {
            int index = 0;
            string reference = cell.CellReference.ToString().ToUpper();
            foreach (char ch in reference)
            {
                if (Char.IsLetter(ch))
                {
                    int value = (int)ch - (int)'A';
                    index = (index == 0) ? value : ((index + 1) * 26) + value;
                }
                else
                {
                    return index;
                }
            }
            return index;
        }
        private static DataTable ConvertToDatetable(SpreadsheetDocument spreadSheetDocument,
            int ilkKacSatirHaric, int sonKacSatirHaric, bool hasTitle)
        {
            DataTable dataTable = new DataTable();

            WorkbookPart workbookPart = spreadSheetDocument.WorkbookPart;
            IEnumerable<Sheet> sheets = spreadSheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
            string relationshipId = sheets.First().Id.Value;
            WorksheetPart worksheetPart = (WorksheetPart)spreadSheetDocument.WorkbookPart.GetPartById(relationshipId);
            Worksheet workSheet = worksheetPart.Worksheet;
            SheetData sheetData = workSheet.GetFirstChild<SheetData>();
            IEnumerable<Row> rows = sheetData.Descendants<Row>();
            for (int i = 0; i < ilkKacSatirHaric; i++)//başlık bilgilerini sil
            {
                if (rows.Count<Row>() > 0)
                    rows.FirstOrDefault().Remove();
            }
            for (int i = 0; i < sonKacSatirHaric; i++)//başlık bilgilerini sil
            {
                if (rows.Count<Row>() > 0)
                    rows.LastOrDefault().Remove();
            }
            int count = 0;
            foreach (Cell cell in rows.ElementAt(0))
            {
                if (hasTitle) //il satır veri içermiyor. başlık içeriyorsa
                {
                    string cellValue = GetCellValue(spreadSheetDocument, cell);
                    dataTable.Columns.Add(cellValue.Equals("0") ? "Col_" + count : cellValue);
                }
                else
                {
                    dataTable.Columns.Add("Column_" + count);
                }
                count++;
            }

            foreach (Row row in rows)
            {
                DataRow dataRow = dataTable.NewRow();
                for (int i = 0; i < row.Descendants<Cell>().Count(); i++)
                {
                    //dataRow[i] = GetCellValue(spreadSheetDocument, row.Descendants<Cell>().ElementAt(i));
                    //SB boş hücreyi atlama sorununu düzeltmek için aşağısı eklendi, üstteki satır kaldırıldı
                    Cell cell = row.Descendants<Cell>().ElementAt(i);
                    int actualCellIndex = CellReferenceToIndex(cell);
                    dataRow[actualCellIndex] = GetCellValue(spreadSheetDocument, cell);
                }

                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }
        private static DataTable ConvertToDatetableKartIle(SpreadsheetDocument spreadSheetDocument, int ilkKacSatirHaric, int sonKacSatirHaric)
        {
            DataTable dataTable = new DataTable();

            WorkbookPart workbookPart = spreadSheetDocument.WorkbookPart;
            IEnumerable<Sheet> sheets = spreadSheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
            string relationshipId = sheets.First().Id.Value;
            WorksheetPart worksheetPart = (WorksheetPart)spreadSheetDocument.WorkbookPart.GetPartById(relationshipId);
            Worksheet workSheet = worksheetPart.Worksheet;
            SheetData sheetData = workSheet.GetFirstChild<SheetData>();
            IEnumerable<Row> rows = sheetData.Descendants<Row>();
            for (int i = 0; i < ilkKacSatirHaric; i++)//başlık bilgilerini sil
            {
                if (rows.Count<Row>() > 0)
                    rows.FirstOrDefault().Remove();
            }
            for (int i = 0; i < sonKacSatirHaric; i++)//başlık bilgilerini sil
            {
                if (rows.Count<Row>() > 0)
                    rows.LastOrDefault().Remove();
            }
            int count = 0;
            foreach (Cell cell in rows.ElementAt(0))
            {

                dataTable.Columns.Add("Column_" + count);
                count++;
            }

            foreach (Row row in rows)
            {
                DataRow dataRow = dataTable.NewRow();
                for (int i = 0; i < row.Descendants<Cell>().Count(); i++)
                {
                    //dataRow[i] = GetCellValue(spreadSheetDocument, row.Descendants<Cell>().ElementAt(i));
                    //SB boş hücreyi atlama sorununu düzeltmek için aşağısı eklendi, üstteki satır kaldırıldı
                    Cell cell = row.Descendants<Cell>().ElementAt(i);
                    int actualCellIndex = CellReferenceToIndex(cell);
                    dataRow[actualCellIndex] = GetCellValue(spreadSheetDocument, cell);
                }

                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }
        private static string GetCellValue(SpreadsheetDocument document, Cell cell)
        {
            string result = string.Empty;
            try
            {
                SharedStringTablePart stringTablePart = document.WorkbookPart.SharedStringTablePart;
                if (cell != null)
                {

                    if (cell.CellValue != null)
                    {
                        string value = cell.CellValue.InnerXml;
                        result = value;
                        int index = 0;
                        if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
                        {
                            if (!string.IsNullOrEmpty(value))
                            {
                                if (Int32.TryParse(value, out index))
                                {
                                    var holder = stringTablePart.SharedStringTable.ChildElements[index];
                                    if (holder != null)
                                    {
                                        result = holder.InnerText;
                                    }
                                }
                            }
                        }
                    }

                }
            }
            catch (Exception e)
            {

                throw e;
            }
            return result;
        }


        public static DataTable ReadXLSAsDataTable(string fileName, string sheetName)
        {
            DataTable dataTable = new DataTable();
            //string connString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=Excel 8.0", fileName);
            string connString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=Excel 8.0", fileName);

            // Create the connection object 
            OleDbConnection oledbConn = new OleDbConnection(connString);
            try
            {
                // Open connection
                oledbConn.Open();

                string query = string.Format("SELECT * FROM [{0}$]", sheetName);

                // Create OleDbCommand object and select data from worksheet Sheet1
                OleDbCommand cmd = new OleDbCommand(query, oledbConn);

                // Create new OleDbDataAdapter 
                OleDbDataAdapter oleda = new OleDbDataAdapter();

                oleda.SelectCommand = cmd;

                // Create a DataSet which will hold the data extracted from the worksheet.
                DataSet ds = new DataSet();

                // Fill the DataSet from the data extracted from the worksheet.
                oleda.Fill(ds, sheetName);

                dataTable = ds.Tables[0];

            }
            catch
            {
                throw;
            }
            finally
            {
                // Close connection
                oledbConn.Close();
            }
            return dataTable;
        }
    }
}
