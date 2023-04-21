using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EL.CollectWebPages.Excel
{
    public class ExcelData
    {
        readonly Type dateTimeType = typeof(DateTime);
        readonly Type nullableDateTimeType = typeof(DateTime?);
        readonly Type guidType = typeof(Guid);
        readonly Type nullableGuidType = typeof(Guid?);
        readonly Type stringType = typeof(string);
        /// <summary>
        /// 当前工作簿
        /// </summary>
        public IWorkbook Workbook { get; }
        public ISheet Sheet { get; }
        /// <summary>
        /// Excel类型 xls/xlsx
        /// </summary>
        public NPOIType Type { get; }
        public ExcelData(ISheet sheet, NPOIType type)
        {
            Sheet = sheet;
            Type = type;
        }

        public bool HasData()
        {
            return Sheet != null && Sheet.PhysicalNumberOfRows > 0;
        }

        public object ReadColumn(ICell cell)
        {

            if (cell == null)
            {
                return null;
            }

            try
            {

                return ReadColumnByCellType(cell);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return null;
        }

        private object ReadColumnByCellType(ICell cell)
        {
            object value = null;

            //读取Excel格式，根据格式读取数据类型
            switch (cell.CellType)
            {
                case CellType.Blank: //空数据类型处理
                    break;
                case CellType.String: //字符串类型
                    value = cell.StringCellValue;
                    break;
                case CellType.Numeric: //数字类型 - >电话号码      
                    //NPOI中数字和日期都是NUMERIC类型的，这里对其进行判断是否是日期类型
                    if (DateUtil.IsCellDateFormatted(cell))//日期类型
                    {
                        value = cell.DateCellValue;
                    }
                    else//其他数字类型
                    {
                        value = cell.NumericCellValue;
                    }

                    //cell.SetCellType(CellType.String);
                    //value = cell.StringCellValue;
                    break;
                case CellType.Formula:
                    IFormulaEvaluator e = Type == NPOIType.xlsx ?
                        new XSSFFormulaEvaluator(Workbook) : new HSSFFormulaEvaluator(Workbook);
                    value = e.Evaluate(cell).StringValue;
                    break;
                default:
                    value = cell.StringCellValue;
                    break;
            }
            return value;
        }
        public List<T> ReadSheet<T>() where T : IExcelInfo
        {
            if (HasData())
            {
                var list = new List<T>();
                //对应Excel表格中的属性信息
                PropertyInfos[] properties = null;
                IRow row;
                bool isFirstRow = true;

                IEnumerator rows = Sheet.GetRowEnumerator();
                var Count = 0;
                while (rows.MoveNext())
                {
                    row = (IRow)rows.Current;

                    if (isFirstRow)
                    {
                        isFirstRow = false;
                        //每个表格第一行，标题行
                        //将第一列作为列表头
                        if (properties == null || properties.Length == 0)
                        {
                            //并确定列数
                            //maxcols = sheet.GetRow(0).LastCellNum;
                            //row.LastCellNum --> colCount --> maxcols
                            //ColumnLength = row.Cells.Count > ColumnLength ? ColumnLength : row.Cells.Count;
                            properties = GetSortProperties<T>(row);
                        }
                    }
                    else
                    {
                        T t = ReadRow<T>(properties, row, Count);
                        list.Add(t);
                    }
                    Count++;
                }
                return list;
            }
            return default;
        }
        /// <summary>
        /// 读行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="properties"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        private T ReadRow<T>(PropertyInfos[] properties, IRow row, int count) where T : IExcelInfo
        {
            T t = Activator.CreateInstance<T>();
            t.RowNumber = count;
            //读列
            for (int i = 0; i < properties.Length; i++)
            {
                //表格
                ICell cell = row.GetCell(i);

                //i列对应的属性信息
                var prop = properties[i];

                object value = ReadColumn(cell);

                try
                {
                    //设置t的属性值为value
                    SetValue(t, prop.PropertyInfo, value);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                }
            }
            return t;
        }
        public void WriteSheet<T>(List<T> values) where T : IExcelInfo
        {
            var properties = GetSortProperties<T>(Sheet.GetRow(0)).Where(t => t.Change).ToList();
            var changeList = values?.Where(t => t.IsNotNullOrEmpty() && t.IsChange).ToList();
            foreach (var item in changeList)
            {
                var row = Sheet.GetRow(item.RowNumber);
                foreach (var prop in properties)
                {
                    var value = prop.PropertyInfo.GetValue(item)?.ToString();
                    if (!string.IsNullOrEmpty(value))
                    {
                        ICell cell = row.GetCell(prop.CellIndex);
                        if (cell == null)
                        {
                            cell = row.CreateCell(prop.CellIndex);
                        }
                        cell.SetCellValue(value?.ToString());
                    }
                }
            }
        }
        /// <summary>
        /// 设置属性值为value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="prop"></param>
        /// <param name="value"></param>
        private void SetValue<T>(T t, PropertyInfo prop, object value)
        {
            if (value == null || prop == null || !prop.CanWrite)
            {
                return;
            }

            var valueType = value.GetType();
            //DateTime类型
            if (prop.PropertyType == nullableDateTimeType || prop.PropertyType == dateTimeType)
            {
                //value是DateTime类型, 直接赋值
                if (valueType == dateTimeType)
                {
                    prop.SetValue(t, value);
                }
                //否则, 尝试转换成DateTime
                else if (valueType == stringType && DateTime.TryParse(value.ToString(), out DateTime dateTimeValue))
                {
                    prop.SetValue(t, dateTimeValue);
                }
            }
            //Guid类型
            else if ((prop.PropertyType == nullableGuidType || prop.PropertyType == guidType) && valueType == stringType)
            {
                //尝试转换成Guid
                if (Guid.TryParse(value.ToString(), out Guid guidValue))
                {
                    prop.SetValue(t, guidValue);
                }
            }
            else
            {
                object _value = Convert.ChangeType(value, prop.PropertyType);
                prop.SetValue(t, _value);
            }
        }

        /// <summary>
        /// 获取和Row.Column位置一一对应的PropertyInfo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <returns></returns>
        private PropertyInfos[] GetSortProperties<T>(IRow row)
        {
            Type type = typeof(T);

            var sortProperties = new List<PropertyInfos>();

            foreach (var porp in type.GetProperties())
            {
                var title = porp.GetCustomAttribute<TitleNameAttribute>();
                if (title != null)
                {
                    PropertyInfos PropertyInfos = new PropertyInfos();
                    PropertyInfos.Value = title.Value;
                    PropertyInfos.Change = title.Change;
                    PropertyInfos.CellIndex = title.CellIndex;
                    PropertyInfos.PropertyInfo = porp;
                    sortProperties.Add(PropertyInfos);
                }
            }
            return sortProperties.ToArray();
        }
    }
    public class PropertyInfos
    {
        public string Value { get; set; }
        public bool Change { get; set; }
        public int CellIndex { get; set; }
        public PropertyInfo PropertyInfo { get; set; }
    }
}
