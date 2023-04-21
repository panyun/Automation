using Automation.Inspect;
using EL;
using Interop.UIAutomationClient;
using System.Data;
using System.Text.RegularExpressions;
using System.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace Automation.Parser
{
    public static class GenerateTableActionSystem
    {
        private static ElementUIA baseWin;
        // ControlType Table
        //

        public static DataTable Main(this GenerateTableActionRequest self)
        {
            var elements = self.AvigationElement();
            baseWin = (ElementUIA)elements.FirstOrDefault();
            if (elements == null || !elements.Any() || baseWin == default) throw new ParserException("无法定位到目标元素！");
            if (self.ElementPath.ProcessName == "EXCEL") // 走Excle识别
            {
                var exceleElement = self.FindExcelElement(baseWin);
                if (exceleElement == null)
                    throw new ParserException("未找到Excel元素信息！");
                self.LightProperty.LightHighMany(exceleElement);
                var excele = self.FindExcleData(exceleElement);
                var dataExcele = DataBaseComponentSystem.ConvertTable(excele);
                return dataExcele;
            }
            Enum.TryParse(baseWin.ControlTypeName, out EL.UIA.ControlTypeConverter.ControlType controlType);
            if (controlType == EL.UIA.ControlTypeConverter.ControlType.ComboBox)
            {
                var comboBoxList = self.FindComboxData(baseWin);
                var dataList = DataBaseComponentSystem.ConvertTable(comboBoxList);
                self.LightProperty.LightHighMany(baseWin);
                return dataList;
            }
            var tableElement = self.FindTableElement(baseWin);
            if (tableElement == default)
            {
                (var parentEle, var index) = self.FindListElement(baseWin, default);
                if (parentEle == default) throw new ParserException("未找到元素信息！");
                var tableList = self.FindListData(parentEle, index);
                var dataList = DataBaseComponentSystem.ConvertTable(tableList);
                self.LightProperty.LightHighMany(parentEle);
                return dataList;
            }
            self.LightProperty.LightHighMany(tableElement);
            var tables = self.FindTableData(tableElement);
            var dataTable = DataBaseComponentSystem.ConvertTable(tables);
            return dataTable;
        }

        public static ElementUIA FindExcelElement(this GenerateTableActionRequest self, ElementUIA element)
        {
            var tableElement = self.FindTableElement(element);
            return tableElement;
        }
        public static ElementUIA FindTableElement(this GenerateTableActionRequest self, ElementUIA elementWin)
        {
            var inspect = Boot.GetComponent<InspectComponent>();
            var winFromInspect = inspect.GetComponent<WinFormInspectComponent>();
            if (elementWin == default) return default;
            if (!IsTable(elementWin))
            {
                var ele = winFromInspect.GetParent(elementWin.NativeElement).Convert();
                elementWin = self.FindTableElement(ele);
            }
            return elementWin;
        }
        public static (ElementUIA, int) FindListElement(this GenerateTableActionRequest self, ElementUIA elementWin, int index)
        {
            index = index + 1;
            var inspect = Boot.GetComponent<InspectComponent>();
            var winFromInspect = inspect.GetComponent<WinFormInspectComponent>();
            var ele = winFromInspect.GetParent(elementWin.NativeElement).Convert();
            if (ele == default)
                throw new ParserException("元素路径异常！");
            Enum.TryParse(ele.ControlTypeName, out EL.UIA.ControlTypeConverter.ControlType controlType);
            if (controlType == EL.UIA.ControlTypeConverter.ControlType.List)
                return (ele, index);
            if (ele == default) return default;
            return self.FindListElement(ele, index);
        }
        public static List<TableData> FindComboxData(this GenerateTableActionRequest self, ElementUIA element)
        {
            var columns = StringHelper.Characters.ToArray();
            var expandPattern = (IUIAutomationExpandCollapsePattern)element.NativeElement.GetCurrentPattern(UIA_PatternIds.UIA_ExpandCollapsePatternId);
            //重点来了，先要将ComBox展开后才能进行选择
            expandPattern.Expand();
            Thread.Sleep(100);
            var inspect = Boot.GetComponent<InspectComponent>();
            var winFromInspect = inspect.GetComponent<WinFormInspectComponent>();
            //winFromInspect.ControlViewWalker.GetFirstChildElement
            var chids = element.NativeElement.FindAll(TreeScope.TreeScope_Children, winFromInspect.UIAFactory.CreateTrueCondition());
            List<TableData> tableDatas = new();
            for (int i = 0; i < chids.Length; i++)
            {
                var e1 = chids.GetElement(i);
                TableData tableData = default;
                (var name, var val) = GetContent(new List<IUIAutomationElement>() { e1 });
                tableData = new()
                {
                    ColumnName = columns[0] + "",
                    ColumnValue = name,
                    Index = i
                };
                tableDatas.Add(tableData);
                tableData = new()
                {
                    ColumnName = columns[1] + "",
                    ColumnValue = val,
                    Index = i
                };
                tableDatas.Add(tableData);
            }
            expandPattern.Collapse();
            return tableDatas;
        }
        public static List<TableData> FindListData(this GenerateTableActionRequest self, ElementUIA element, int index)
        {
            var columns = StringHelper.Characters.ToArray();
            List<TableData> tableDatas = new();
            var elemnts = GetChildrens(new List<IUIAutomationElement>() { element.NativeElement }, index);
            for (int i = 0; i < elemnts.Count; i++)
            {
                if (elemnts[i].CurrentControlType != baseWin.ControlType)
                    continue;
                TableData tableData = default;
                (var name, var val) = GetContent(new List<IUIAutomationElement>() { elemnts[i] });
                tableData = new()
                {
                    ColumnName = columns[0] + "",
                    ColumnValue = name,
                    Index = i
                };
                tableDatas.Add(tableData);
                tableData = new()
                {
                    ColumnName = columns[1] + "",
                    ColumnValue = val,
                    Index = i
                };
                tableDatas.Add(tableData);
            }
            return tableDatas;
        }
        private static (string, string) GetContent(List<IUIAutomationElement> uIAutomationElements)
        {
            string name = "";
            string val = "";
            foreach (var ele in uIAutomationElements)
            {
                name += ele.CurrentName;
                val += ele.GetValue();
            }

            if (!string.IsNullOrEmpty(name) || !string.IsNullOrEmpty(val))
                return (name, val);
            List<IUIAutomationElement> temp = new();
            foreach (var item in uIAutomationElements)
            {
                IUIAutomationElementArray children = item.FindAll(TreeScope.TreeScope_Children, new CUIAutomation().CreateTrueCondition());
                for (int i = 0; i < children.Length; i++)
                    temp.Add(children.GetElement(i));
            }
            return GetContent(temp);
        }
        /// <summary>
        /// ControlType Table
        /// Role  grid
        /// ClassName dataintable
        /// </summary>
        /// <param name="elementWin"></param>
        /// <returns></returns>
        private static bool IsTable(ElementUIA elementWin)
        {
            if (elementWin == default) return false;
            Enum.TryParse(elementWin.ControlTypeName, out EL.UIA.ControlTypeConverter.ControlType controlType);
            if (controlType == EL.UIA.ControlTypeConverter.ControlType.Table ||
                elementWin.Role == "grid" || controlType == EL.UIA.ControlTypeConverter.ControlType.DataGrid)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// ControlType Table
        /// Role  grid
        /// ClassName dataintable
        /// </summary>
        /// <param name="self"></param>
        /// <param name="element"></param>
        /// <returns></returns>

        public static List<TableData> FindTableData(this GenerateTableActionRequest self, ElementUIA element)
        {

            var columns = StringHelper.Characters.ToArray();
            List<TableData> tableDatas = new();
            var gridPattern = (IUIAutomationGridPattern)element.NativeElement.GetCurrentPattern(UIA_PatternIds.UIA_GridPatternId);
            if (gridPattern != null)
            {
                var columnCount = gridPattern.CurrentColumnCount;
                var rowCount = gridPattern.CurrentRowCount;
                for (int r = 0; r < rowCount; r++)
                {
                    for (int c = 0; c < columnCount; c++)
                    {
                        var e = gridPattern.GetItem(r, c);
                        (var name, var val) = GetContent(new List<IUIAutomationElement>() { e });
                        TableData tableData = new()
                        {
                            ColumnName = columns[c] + "",
                            ColumnValue = name,
                            Index = r
                        };
                        tableDatas.Add(tableData);
                    }
                }
                return tableDatas;
            }
            #region  控件遍历
            var inspect = Boot.GetComponent<InspectComponent>();
            var winFromInspect = inspect.GetComponent<WinFormInspectComponent>();
            var tree = winFromInspect.GetAllChildrenNode(element.NativeElement, 3);
            for (int r = 0; r < tree.Children.Count; r++)
            {
                try
                {
                    var col = tree.Children[r].Children.Select(x => x.CurrentElementWin.Name).ToList();
                    for (int c = 0; c < col.Count; r++)
                    {
                        TableData tableData = new()
                        {
                            ColumnName = columns[c] + "",
                            ColumnValue = col[c],
                            Index = r
                        };
                        tableDatas.Add(tableData);
                    }
                }
                catch (Exception ex)
                {
                    EL.Log.Error(ex);
                    continue;
                }

            }
            return tableDatas;
            #endregion
        }
        public static List<TableData> FindExcleData(this GenerateTableActionRequest self, ElementUIA element)
        {
            List<TableData> tableDatas = new();
            var datas = GetExcleChildrens(element.NativeElement);
            for (int i = 0; i < datas.Count; i++)
            {
                var name = datas[i].Name + "";
                var col = Regex.Replace(name, @"[^A-Z]+", "");
                var index = Regex.Replace(name, @"[^0-9]+", "");
                var colIndex = int.Parse(index);
                TableData tableData = new()
                {
                    ColumnName = col,
                    ColumnValue = datas[i].Value,
                    Index = colIndex
                };
                tableDatas.Add(tableData);
            }
            return tableDatas;
        }
        public static List<IUIAutomationElement> GetChildrens(List<IUIAutomationElement> uIAutomationElements, int index)
        {
            if (!uIAutomationElements.Any())
                throw new ParserException("路径未找到！");
            List<IUIAutomationElement> temp = new List<IUIAutomationElement>();
            foreach (var item in uIAutomationElements)
            {
                IUIAutomationElementArray children = item.FindAll(TreeScope.TreeScope_Children, new CUIAutomation().CreateTrueCondition());

                for (int i = 0; i < children.Length; i++)
                {
                    var name = children.GetElement(i).CurrentName;
                    temp.Add(children.GetElement(i));
                }
            }
            index = index - 1;
            if (index == 0)
                return temp;
            return GetChildrens(temp, index);
        }
        public static List<dynamic> GetExcleChildrens(IUIAutomationElement element)
        {
            List<dynamic> dynamics = new List<dynamic>();
            IUIAutomationElementArray children = element.FindAll(TreeScope.TreeScope_Children, new CUIAutomation().CreateTrueCondition());
            for (int i = 0; i < children.Length; i++)
            {
                var value = children.GetElement(i).GetValue();
                if (string.IsNullOrEmpty(value)) continue;
                dynamics.Add(new
                {
                    Name = children.GetElement(i).CurrentName,
                    Value = value
                });
            }
            return dynamics;
        }

    }
}
