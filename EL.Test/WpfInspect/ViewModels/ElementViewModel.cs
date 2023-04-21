using Automation.Inspect;
using EL;
using EL.UIA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WpfInspect.Core;

namespace WpfInspect.ViewModels
{
    public class ElementViewModel : ObservableObject
    {
        public event Action<ElementViewModel> SelectionChanged;


        public ElementViewModel(ElementNode elementNode, int level = 0)
        {
            _elementNode = elementNode;
            Children = new ExtendedObservableCollection<ElementViewModel>();
            ItemDetails = new ExtendedObservableCollection<DetailGroupViewModel>();
            Level = level;
        }

        public int Level { get; set; }

        public ElementNode _elementNode { get; set; }

        public ElementPath ElementPath { get; set; }

        public bool IsSelected
        {
            get { return GetProperty<bool>(); }
            set
            {
                try
                {
                    if (value)
                    {
                        // Async load details
                        var unused = Task.Run(() =>
                        {
                            var details = LoadDetails();
                            return details;
                        }).ContinueWith(items =>
                        {
                            if (items.IsFaulted)
                            {
                                if (items.Exception != null)
                                {
                                    MessageBox.Show(items.Exception.ToString());
                                }
                            }
                            ItemDetails.Reset(items.Result);
                        }, TaskScheduler.FromCurrentSynchronizationContext());

                        // Fire the selection event
                        SelectionChanged?.Invoke(this);
                    }

                    SetProperty(value);
                }
                catch (Exception ex)
                {
                    Console.Write(ex.ToString());
                }
            }
        }

        public bool LoadChild = true;

        public bool IsSecond => Level == 1 || Level == 0;

        public bool HasChild => Children != null && Children.Count > 0;

        public bool IsExpanded
        {
            get { return GetProperty<bool>(); }
            set
            {
                SetProperty(value);
                if (value && LoadChild)
                {
                    LoadChildren(true);
                }
            }
        }

        public string Name => NormalizeString(_elementNode.CurrentElementWin.Name);

        public string AutomationId => NormalizeString(_elementNode.CurrentElementWin.NativeElement.CurrentAutomationId);

        public string ControlTypeName => EL.UIA.ControlTypeConverter.ConvertTypeToName(_elementNode.CurrentElementWin.ControlType);

        public ExtendedObservableCollection<ElementViewModel> Children { get; set; }

        public ExtendedObservableCollection<DetailGroupViewModel> ItemDetails { get; set; }

        /// <summary>
        /// 已经加载子节点的子节点
        /// </summary>
        public bool HasLoadChildrenTrue;

        public void LoadChildren(bool loadInnerChildren)
        {
            if (loadInnerChildren)
                HasLoadChildrenTrue = true;
            foreach (var child in Children)
            {
                child.SelectionChanged -= SelectionChanged;
            }

            var childrenViewModels = new List<ElementViewModel>();
            try
            {
                var inspect = Boot.GetComponent<InspectComponent>().GetComponent<WinFormInspectComponent>();
                var nodes = inspect.GetAllChildrens(_elementNode.CurrentElementWin.NativeElement);
                if (nodes.Count > 0)
                {
                    var groups = nodes.GroupBy(x => x.CurrentElementWin.ControlTypeName).ToList();
                    foreach (var child in nodes)
                    { 
                        child.Index = groups.First(x => x.Key == child.CurrentElementWin.ControlTypeName).ToList().FindIndex(x => x == child);
                        var childViewModel = new ElementViewModel(child, Level + 1);
                        childViewModel.SelectionChanged += SelectionChanged;
                        childrenViewModels.Add(childViewModel);
                        if (loadInnerChildren)
                        {
                            //Task.Run(() =>
                            //{
                            childViewModel.LoadChildren(false);
                            //});
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Exception: {ex.Message}");
            }

            //DispatcherHelper.UpdateUi(() =>
            //{
            Children.Reset(childrenViewModels);
            //});
        }

        private List<DetailGroupViewModel> LoadDetails()
        {
            return new List<DetailGroupViewModel>();
        }

        private string NormalizeString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }
            value = value.Replace(Environment.NewLine, " ").Replace('\r', ' ').Replace('\n', ' ');
            if (value.Length > 18)
                value = value.Substring(0, 18) + "...";
            return value;
        }
    }
}
