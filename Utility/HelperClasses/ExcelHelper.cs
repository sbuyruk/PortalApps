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
            if (hasTitle) //il satir veri içermiyor. baslik içeriyorsa
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
            if (hasTitle) //il satir veri içermiyor. baslik içeriyorsa
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
            if (hasTitle) //il satir veri içermiyor. baslik içeriyorsa
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
                if (hasTitle) //il satir veri içermiyor. baslik içeriyorsa
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
                    //SB bos hücreyi atlama sorununu düzeltmek için asagisi eklendi, üstteki satir kaldirildi
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
            // Bos satirlar XML'de bulunmadigindan element-count yerine gerçek Excel satir numarasiyla filtrele
            if (ilkKacSatirHaric > 0)
            {
                var rowsToRemoveFirst = rows.Where(r => r.RowIndex != null && r.RowIndex.Value <= (uint)ilkKacSatirHaric).ToList();
                foreach (var r in rowsToRemoveFirst)
                    r.Remove();
            }
            for (int i = 0; i < sonKacSatirHaric; i++)//son satirlari sil
            {
                if (rows.Count<Row>() > 0)
                    rows.LastOrDefault().Remove();
            }
            // Build columns using the actual cell reference index to account for empty/skipped cells in the header row.
            // OpenXML omits empty cells from the XML, so sequential counting would produce fewer columns than the
            // actual sheet width, causing IndexOutOfRangeException when data rows reference a higher column index.
            // Scan ALL rows to find the true maximum column index, since data rows may be wider than the header row.
            int maxColumnIndex = -1;
            foreach (Row r in rows)
            {
                foreach (Cell cell in r.Descendants<Cell>())
                {
                    int idx = CellReferenceToIndex(cell);
                    if (idx > maxColumnIndex) maxColumnIndex = idx;
                }
            }
            var headerCells = rows.ElementAt(0).Descendants<Cell>().ToList();
            for (int col = 0; col <= maxColumnIndex; col++)
            {
                Cell headerCell = headerCells.FirstOrDefault(c => CellReferenceToIndex(c) == col);
                if (hasTitle && headerCell != null)
                {
                    string cellValue = GetCellValue(spreadSheetDocument, headerCell);
                    dataTable.Columns.Add(cellValue.Equals("0") ? "Col_" + col : cellValue);
                }
                else
                {
                    dataTable.Columns.Add("Column_" + col);
                }
            }

            foreach (Row row in rows)
            {
                DataRow dataRow = dataTable.NewRow();
                for (int i = 0; i < row.Descendants<Cell>().Count(); i++)
                {
                    //dataRow[i] = GetCellValue(spreadSheetDocument, row.Descendants<Cell>().ElementAt(i));
                    //SB bos hücreyi atlama sorununu düzeltmek için asagisi eklendi, üstteki satir kaldirildi
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
            // Bos satirlar XML'de bulunmadigindan element-count yerine gerçek Excel satir numarasiyla filtrele
            if (ilkKacSatirHaric > 0)
            {
                var rowsToRemoveFirst = rows.Where(r => r.RowIndex != null && r.RowIndex.Value <= (uint)ilkKacSatirHaric).ToList();
                foreach (var r in rowsToRemoveFirst)
                    r.Remove();
            }
            for (int i = 0; i < sonKacSatirHaric; i++)//son satirlari sil
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
                    //SB bos hücreyi atlama sorununu düzeltmek için asagisi eklendi, üstteki satir kaldirildi
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
