using Automation.Inspect;
using Automation.Parser;
using EL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using WpfInspect.ViewModels;

namespace WpfInspect.Core
{
    public class ElementPathHelper
    {
        /// <summary>
        /// 定位元素路径
        /// </summary>
        /// <param name="elementPath"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public static ElementViewModel FindElement(ElementPath elementPath, ObservableCollection<ElementViewModel> elements)
        {
            if (elementPath == null)
                return null;

            var tempViewModels = elements;
            ElementViewModel oldViewModel = null;

            var lastElementNode = elementPath.ElementEditNodes.Last(x => x.IsChecked);
            foreach (var z in elementPath.ElementEditNodes)
            {
                if (!z.IsChecked)
                    continue;

                var viewModels = tempViewModels.Where(x => (x._elementNode.CurrentElementWin.Name ?? "") == (z.Name ?? "") && x._elementNode?.CurrentElementWin?.ControlType == z.ControlType).ToList();
                if (viewModels.Count == 0)
                {
                    if (oldViewModel != null)
                        oldViewModel.IsSelected = true;
                    return null;
                }

                var viewModel = viewModels.FirstOrDefault(x => x._elementNode.Index == z.Index) ?? viewModels.FirstOrDefault();
                if (!viewModel.IsExpanded)
                {
                    viewModel.LoadChild = false;
                    viewModel.IsExpanded = true;
                    viewModel.LoadChild = true;
                    tempViewModels.Where(x => x != viewModel && x.IsExpanded).ToList().ForEach(x => x.IsExpanded = false);
                }

                if (viewModel.Children.Count == 0 || !viewModel.HasLoadChildrenTrue)
                    viewModel.LoadChildren(true);

                if (lastElementNode == z)
                {
                    viewModel.IsSelected = true;
                    return viewModel;
                }

                oldViewModel = viewModel;
                tempViewModels = viewModel.Children;
            };

            return oldViewModel;
        }

        public static List<ElementViewModel> FindSimilarElement(ElementPath elementPath, ObservableCollection<ElementViewModel> elements)
        {
            var tempViewModels = elements;

            var lastNode = elementPath.ElementEditNodes.Last();
            foreach (var z in elementPath.ElementEditNodes)
            {
                if (z == lastNode)
                    return tempViewModels.Where(x => x._elementNode.CurrentElementWin.ControlType == z.ControlType).ToList();

                var viewModels = tempViewModels.Where(x => (x._elementNode.CurrentElementWin.Name ?? "") == (z.Name ?? "") && x._elementNode?.CurrentElementWin?.ControlType == z.ControlType).ToList();
                if (viewModels.Count == 0)
                    return viewModels;

                var viewModel = viewModels.FirstOrDefault(x => x._elementNode.Index == z.Index) ?? viewModels.FirstOrDefault();
                if (!viewModel.IsExpanded)
                {
                    viewModel.LoadChild = false;
                    viewModel.IsExpanded = true;
                    viewModel.LoadChild = true;
                    tempViewModels.Where(x => x != viewModel && x.IsExpanded).ToList().ForEach(x => x.IsExpanded = false);
                }

                if (viewModel.Children.Count == 0 || !viewModel.HasLoadChildrenTrue)
                    viewModel.LoadChildren(true);

                tempViewModels = viewModel.Children;
            };
            return new List<ElementViewModel>();
        }

        /// <summary>
        /// 更新ElementPath对应字段 并返回一个新的ElementPath
        /// </summary>
        /// <param name="elementPath"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public static ElementPath UpdateElementPathWithNew(ElementPath elementPath, ObservableCollection<PathNodeViewModel> details)
        {
            var newElementPath = JsonHelper.FromJson<ElementPath>(JsonHelper.ToJson(elementPath));
            UpdateElementPath(newElementPath, details);
            return newElementPath;
        }

        /// <summary>
        /// 更新ElementPath对应字段
        /// </summary>
        /// <param name="elementPath"></param>
        /// <param name="details"></param>
        public static void UpdateElementPath(ElementPath elementPath, ObservableCollection<PathNodeViewModel> details)
        {
            if (details.Count == 0)
                return;
            var elementNodes = elementPath.ElementEditNodes;
            if (elementNodes == null || elementNodes.Count == 0)
                return;
            int i = 1;
            elementNodes.ForEach(x =>
            {
                var detail = details.FirstOrDefault(z => z.Id == x.Id);
                if (detail != null)
                {
                    var nameProperty = x.GetProperty(nameof(detail.Name));
                    nameProperty.Value = string.IsNullOrEmpty(detail.Name) ? null : detail.Name;
                    nameProperty.IsActive = detail.IsActiveName;

                    var indexProperty = x.GetProperty(nameof(detail.Index));
                    indexProperty.Value = detail.Index;
                    indexProperty.Expression = detail.Index.ToString();
                    indexProperty.IsActive = detail.IsActiveIndex;

                    var checkProperty = x.GetProperty("IsChecked");
                    checkProperty.Value = detail.IsActive;
                    checkProperty.Expression = detail.IsActive.ToString().ToLower();
                }
                i++;
            });
        }

        public static bool ComPareElementPaths(ElementPath element1, ElementPath element2)
        {
            var nodes1 = element1.ElementEditNodes;
            var nodes2 = element2.ElementEditNodes;

            if (nodes1.Count != nodes2.Count)
                return false;
            int i = 0;
            while (i < nodes1.Count)
            {
                if (i == nodes1.Count - 1)
                {
                    if (nodes1[i].ControlType == nodes2[i].ControlType)
                        return true;
                    else
                        return false;
                }

                if (nodes1[i].Name != nodes2[i].Name || nodes1[i].ControlType != nodes2[i].ControlType || nodes1[i].Index != nodes2[i].Index)
                    return false;
                i++;
            }
            return true;
        }
    }
}
