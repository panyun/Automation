using CommandLine;
using EL.CollectWebPages.BLL;
using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.Util;
using NPOI.Util.Collections;
using NPOI.XSSF.UserModel;
using OfficeOpenXml.Export.ToDataTable;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EL.CollectWebPages.Excel
{
    public static class NPOIHelper
    {
        public static List<T> ReadData<T>(string excelPath, string sheetName, int startRow = 1) where T : IExcelInfo
        {
            IWorkbook workbook = null;
            try
            {
                var type = NPOITypeUtils.GetType(excelPath);
                ISheet Sheet = null;
                //初始化信息
                if (type == NPOIType.xlsx)
                {
                    workbook = new XSSFWorkbook(new FileStream(excelPath, FileMode.Open, FileAccess.Read, FileShare.Read));
                }
                else
                {
                    workbook = new HSSFWorkbook(new FileStream(excelPath, FileMode.Open, FileAccess.Read, FileShare.Read));
                }
                Sheet = workbook.GetSheet(sheetName);
                ExcelData excelData = new ExcelData(Sheet, type);
                return excelData.ReadSheet<T>();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                workbook?.Close();
            }
            return null;
        }
        public static void WriteData(string excelPath, string sheetName, List<ExcelInfo> excelInfos, int startRow = 1)
        {
            IWorkbook workbook = null;
            try
            {
                var type = NPOITypeUtils.GetType(excelPath);
                using (FileStream file = new FileStream(excelPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    //初始化信息
                    if (type == NPOIType.xlsx)
                    {
                        workbook = new XSSFWorkbook(file);
                    }
                    else
                    {
                        workbook = new HSSFWorkbook(file);
                    }
                }
                var Sheet = workbook.GetSheet(sheetName);
                ExcelData excelData = new ExcelData(Sheet, type);
                excelData.WriteSheet(excelInfos);

                using (FileStream file = new FileStream(excelPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    workbook.Write(file, true);
                    file.Flush();
                    file.Close();
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                workbook?.Close();
            }
        }
    }
}
