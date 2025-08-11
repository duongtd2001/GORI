using ExcelDataReader;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GORI.Services.DataServices
{
    public class ExcelRW
    {
        private MachineData machineData;

        int lastCol = 0;
        int colSave = 0;
        int lastRow = 1;

        //public List<(string _key, string _value)> DataEmployees = new List<(string _key, string _value)>();
        public Dictionary<string, string> Employees = new Dictionary<string, string>();

        public List<double> datas = new List<double>();

        public ExcelRW(MachineData _machineData)
        {
            machineData = _machineData;

            var fileData = new System.IO.FileInfo(machineData.PathCombineData);
            using (var packageSave = new ExcelPackage(fileData))
            {
                if (!fileData.Exists)
                {
                    var worksheetsv = packageSave.Workbook.Worksheets.Add("DATA GORI");
                    packageSave.Save();
                }
            };
            //AddColumn();
        }

        public void AddColumn(string _po)
        {
            var fileSaveData = new System.IO.FileInfo(machineData.PathCombineData);
            using (var packageSave = new ExcelPackage(fileSaveData))
            {
                if (fileSaveData.Exists)
                {
                    var worksheetSave = packageSave.Workbook.Worksheets["DATA GORI"];

                    lastCol = worksheetSave.Dimension?.End.Column ?? 0;
                    int _sttCol = lastCol;
                    lastCol++;
                    worksheetSave.Cells[1, lastCol].Value = _po;
                    _sttCol++;
                    packageSave.Save();
                }
            }
        }

        public void SaveData(Queue<double> values)
        {
            var fileInfo = new System.IO.FileInfo(machineData.PathCombineData);
            using (var packgeSave = new ExcelPackage(fileInfo))
            {
                var worksheetSave = packgeSave.Workbook.Worksheets["DATA GORI"];
               
                worksheetSave.Cells[2, lastCol, QueueToArray(values).GetLength(0), lastCol].Value = QueueToArray(values);
                packgeSave.Save();
            }
        }

        public object[,] QueueToArray(Queue<double> queue)
        {
            int rowCount = queue.Count;

            object[,] results = new object[rowCount, 1];

            int i = 0;
            foreach (var row in queue)
            {
                results[i, 0] = row;
                i++;
            }
            return results;
        }

        public List<double> ReadData(int colIndex)
        {
            using (var packgeRead = new ExcelPackage(new System.IO.FileInfo(@"C:\Users\duong\Documents\GORI\Data.xlsx")))
            {
                var worksheet = packgeRead.Workbook.Worksheets["DATA GORI"];
                int startRow = 2;
                //int colIndex = 2;

                for (int i = 0; i < 500; i++)
                {
                    var cellValue = worksheet.Cells[startRow, colIndex].Text;
                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        if (double.TryParse(cellValue, out var value))
                        {
                            datas.Add(value);
                        }
                    }
                    startRow++;
                }
                return datas;
            }
        }

        //public List<(string _key, string _value)> FindProductByID(string idToFind)
        //{
        //    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        //    using (var stream = File.Open(machineData.PathCombineEmployees, FileMode.Open, FileAccess.Read))
        //    using (var reader = ExcelReaderFactory.CreateReader(stream))
        //    {
        //        var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration()
        //        {
        //            ConfigureDataTable = (_) => new ExcelDataTableConfiguration() { UseHeaderRow = false }
        //        });

        //        var table = dataSet.Tables[0];

        //        foreach (DataRow row in table.Rows)
        //        {
        //            if (row.ItemArray.Length > 1 && row[1].ToString() == idToFind)
        //            {
        //                DataEmployees.Add(("STT", row[0].ToString()));
        //                DataEmployees.Add(("ID", row[1].ToString()));
        //                DataEmployees.Add(("NAME", row[2].ToString()));
        //                DataEmployees.Add(("ACCESS", row[6].ToString()));
        //                return DataEmployees;
        //            }
        //        }
        //    }
        //    return null;
        //}

        public Dictionary<string, string> FindProductByID(string idToFind)
        {
            System.Text.Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using (var stream = File.Open(machineData.PathCombineEmployees, FileMode.Open, FileAccess.Read))
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration() { UseHeaderRow = false }
                });

                var table = dataSet.Tables[0];

                foreach (DataRow row in table.Rows)
                {
                    if (row.ItemArray.Length > 1 && row[1].ToString() == idToFind)
                    {
                        Employees["STT"] = row[0].ToString();
                        Employees["ID"] = row[1].ToString();
                        Employees["Name"] = row[2].ToString();
                        Employees["Access"] = row[6].ToString();
                    }
                }
                return Employees;
            }
        }
    }
}
