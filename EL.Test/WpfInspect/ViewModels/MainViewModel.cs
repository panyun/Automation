using Automation;
using Automation.Inspect;
using Automation.Parser;
using CommandLine;
using EL;
using EL.Basic.Component.Clipboard;
using EL.Sqlite;
using EL.UIA;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WpfInspect.Controls;
using WpfInspect.Core;

namespace WpfInspect.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        #region 执行命令
        /// <summary>
        /// 刷新目录树
        /// </summary>
        public ICommand RefreshCommand { get; set; }

        /// <summary>
        /// 树节点详情显示
        /// </summary>
        public ICommand LoadElement { get; set; }

        /// <summary>
        /// 捕获目标元素
        /// </summary>
        public ICommand CatchElement { get; set; }

        /// <summary>
        /// 捕获相似元素
        /// </summary>
        public ICommand CatchSimilarElement { get; set; }

        /// <summary>
        /// 高亮目标
        /// </summary>
        public ICommand HighLightElement { get; set; }

        /// <summary>
        /// 保存元素到历史元素表中
        /// </summary>
        public ICommand SaveElmenet { get; set; }

        /// <summary>
        /// 加载历史元素到界面
        /// </summary>
        public ICommand LoadHistoryElemnt { get; set; }

        /// <summary>
        /// 删除历史元素
        /// </summary>
        public ICommand DeleteHistoryElemnt { get; set; }

        /// <summary>
        /// 初始化字段详情到界面
        /// </summary>
        public ICommand Initialized { get; set; }

        /// <summary>
        /// 加载指定节点到节点详情界面
        /// </summary>
        public ICommand LoadNodeDetails { get; set; }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        public ICommand CloseWindow { get; set; }

        public Action Hide;
        public Action Show;
        public Action Close;
        #endregion

        public Action<Automation.Inspect.Element, ElementPath> CallBackAction;

        /// <summary>
        /// 节点树
        /// </summary>
        public ObservableCollection<ElementViewModel> Elements { get; set; } = new ObservableCollection<ElementViewModel>();

        /// <summary>
        /// 当前节点详情(筛选特征)
        /// </summary>
        public ObservableCollection<PathNodeViewModel> PathNodeDetails { get; set; } = new ObservableCollection<PathNodeViewModel>();

        /// <summary>
        /// 当前节点中指定节点详细信息(属性和属性名)
        /// </summary>
        public ObservableCollection<PathNodeDetails> NodePropertyDetails { get; set; } = new ObservableCollection<PathNodeDetails>();

        /// <summary>
        /// 筛选结果
        /// </summary>
        public ObservableCollection<PathNodeViewModel> NodeFiltDetails { get; set; } = new ObservableCollection<PathNodeViewModel>();

        /// <summary>
        /// 选中节点图像集合(适配多元素时显示)
        /// </summary>
        public ObservableCollection<ImagesViewModel> ImagesViewModels { get; set; } = new ObservableCollection<ImagesViewModel>();

        /// <summary>
        /// 历史节点元素
        /// </summary>
        public ObservableCollection<ElementPathModelEntity> HistoryElements { get; set; } = new ObservableCollection<ElementPathModelEntity>();

        /// <summary>
        /// 当前选中节点
        /// </summary>
        public ElementViewModel SelectedItemInTree
        {
            get { return GetProperty<ElementViewModel>(); }
            private set { SetProperty(value); }
        }

        /// <summary>
        /// 界面加载节点Tree
        /// </summary>
        public ElementViewModel LocalItemTree;

        public System.Windows.Controls.Button HighLightButton;
        public Window window;


        public string HistorySortField = "CreateTime";
        public string HistorySortDesc = "DESC";


        ElSqliteComponent SqlComponent;
        WinFormInspectComponent winInspect;
        WinPathComponent winPathInspect;
        ClipboardComponent clipboardComponent;

        public MainViewModel()
        {
            SqlComponent = Boot.GetComponent<ElSqliteComponent>();
            var inspect = Boot.GetComponent<InspectComponent>();
            winInspect = inspect.GetComponent<WinFormInspectComponent>();
            clipboardComponent = inspect.GetComponent<ClipboardComponent>();
            winPathInspect = winInspect.GetComponent<WinPathComponent>();

            RefreshCommand = new RelayCommand(x => RefreshTree());
            LoadElement = new RelayCommand(x => LoadElementMethod(false));
            CatchElement = new RelayCommand(x => CatchElementMethod());
            CatchSimilarElement = new RelayCommand(x => CatchSimilarElementMethod());
            HighLightElement = new RelayCommand(x => HighLightElementMethods());
            SaveElmenet = new RelayCommand(x => SaveElmenetMethod());
            DeleteHistoryElemnt = new RelayCommand(x => DeleteHistoryElemntMethod(x));
            LoadHistoryElemnt = new RelayCommand(x => LoadHistoryElemntMethod(x));
            Initialized = new RelayCommand(x => InitializedMethod());
            LoadNodeDetails = new RelayCommand(x => LoadNodeDetailsMethod(x));
            CloseWindow = new RelayCommand(x => CloseWindowMethod());
        }

        #region ICommand 具体绑定方法 
        /// <summary>
        /// 节点高亮
        /// </summary>
        async void HighLightElementMethods()
        {
            if (LocalItemTree == null) // 当前节点为空不高亮显示
                return;
            GetSelectElementPath();

            if (LocalItemTree?.ElementPath == null)
                return;

            var path = ElementPathHelper.UpdateElementPathWithNew(LocalItemTree.ElementPath, PathNodeDetails);

            var request = new ElementVerificationActionRequest()
            {
                ElementPath = path,
                ElementEditNodes = path.ElementEditNodes,
                LightProperty = new LightProperty()
                {
                    Time = 500,
                    Count = 3,
                }
            };
            Hide();
            var parser = Boot.GetComponent<ParserComponent>();
            var respose = (ElementVerificationActionResponse)await InspectRequestManager.StartAsync(request);
            //var json = clipboardComponent.GetFromClipboard();
            HighLightButton.Tag = "/Resources/高亮验证@3x.png";
            ShowMessage("验证成功");
            Show();
        }

        /// <summary>
        /// 展示信息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="error"></param>
        void ShowMessage(string msg, bool error = false)
        {
            var toast = new ToastOptions()
            {
                Icon = ToastIcons.Information,
                ToastMargin = new Thickness(0, 120, 0, 0),
                Background = (System.Windows.Media.Brush)new BrushConverter().ConvertFromString("#43A047"),
                Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFromString("#FFFFFF"),
                IconForeground = (System.Windows.Media.Brush)new BrushConverter().ConvertFromString("#FFFFFF"),
                FontSize = 16,
                IconSize = 16,
                Location = ToastLocation.OwnerTopCenter,
                Time = 3000
            };
            if (error)
            {
                toast.Icon = ToastIcons.Warning;
                toast.Background = (System.Windows.Media.Brush)new BrushConverter().ConvertFromString("#FF5722");
            }
            Task.Run(() =>
            {
                DispatcherHelper.UpdateUi(() => Toast.Show(msg, toast));
            });
        }

        /// <summary>
        /// 捕获元素
        /// </summary>
        public async void CatchElementMethod()
        {
            Hide();
            CatchUIRequest catchElementRequest = new CatchUIRequest();
            var res = (CatchUIResponse)await InspectRequestManager.StartAsync(catchElementRequest);
            Show();
            if (res.Error != 0)
            {
                ShowMessage(res.Message);
                return;
            }
            SelectedItemInTree = new ElementViewModel(res.ElementPath.PathNode) { ElementPath = res.ElementPath };
            LoadElementMethod();
        }

        /// <summary>
        /// 捕获相似元素
        /// </summary>
        public async void CatchSimilarElementMethod()
        {
            Hide();
            CatchUIRequest catchElementRequest = new CatchUIRequest()
            {
                Msg = "捕获相似元素启动成功！",
            };
            string param = "10000{\"$type\":\"Automation.Inspect.CatchElementRequest, EL.Automation\",\"RpcId\":1,\"Msg\":\"捕获相似元素启动成功！\"}";
            CatchUIRequest request = new CatchUIRequest()
            {
                Msg = "捕获相似元素启动成功"
            };
            var secondPath = SelectedItemInTree?.ElementPath;
            var respose = (CatchUIResponse)await InspectRequestManager.StartAsync(catchElementRequest);
            if (respose.Error != 0)
            {
                Show();
                ShowMessage(respose.Message);
                return;

            }
            var GenerateCosineSimilarActionRequest = new GenerateCosineSimilarActionRequest()
            {
                ElementPath = respose.ElementPath,
                TimeOut = default,
                CosineValue = 0.8,
                LightProperty = new LightProperty
                {
                    Count = 3,
                    Time = 500
                }
            };
            var generateSimilarElementActionResponse = (GenerateCosineSimilarActionResponse)await InspectRequestManager.StartAsync(GenerateCosineSimilarActionRequest);
            if (generateSimilarElementActionResponse.Error != default || generateSimilarElementActionResponse.ElementPath == default)
            {
                Show();
                ShowMessage("相似元素捕获失败！");
                return;
            }
            LocalItemTree = SelectedItemInTree = new ElementViewModel(generateSimilarElementActionResponse.ElementPath.PathNode) { ElementPath = generateSimilarElementActionResponse.ElementPath };
            LoadElementMethod();
            //HighLightElementMethods();
            Show();
            //RequestManager.Start(param, new Action<Element, ElementPath>((x, y) =>
            //{
            //    if (x == default || y == default)
            //        return;
            //    firstPath = y;
            //    if (secondPath == default)
            //    {
            //        ShowMessage("相似元素捕获失败！");
            //        return;
            //    }
            //    var request = new GenerateSimilarElementActionRequest()
            //    {
            //        ElementPath = firstPath,
            //        LastElementPath = secondPath,
            //        TimeOut = default,
            //    };
            //    param = $"{(int)RequestType.GenerateSimilarElementActionRequest}{JsonHelper.ToJson(request)}";
            //    var parser = Boot.GetComponent<ParserComponent>();

            //    parser.CallBack = (x, y) =>
            //    {
            //        if (!x)
            //        {
            //            ShowMessage($"相似元素捕获失败！{y}");
            //            return;
            //        }
            //        var generateSimilarElementResponse = y as GenerateSimilarElementActionResponse;
            //        LocalItemTree = SelectedItemInTree = new ElementViewModel(generateSimilarElementResponse.ElementPath.PathNode) { ElementPath = generateSimilarElementResponse.ElementPath };
            //        LoadElementMethod();
            //        parser.CallBack = default;
            //        HighLightElementMethods();
            //        Show();
            //    };
            //    RequestManager.Start(param);
            //}));

            //RequestManager.Start(param, new Action<Automation.Inspect.Element, ElementPath>((x, y) =>
            //{
            //    if (x == default || y == default)
            //        return;
            //    LocalItemTree = SelectedItemInTree = new ElementViewModel(y.PathNode) { ElementPath = y };
            //    var lastNode = LocalItemTree.ElementPath.ElementEditNodes.Last();
            //    lastNode.GetProperty("Name").IsActive = false;
            //    lastNode.GetProperty("Index").IsActive = false;

            //    LoadElementMethod();
            //    HighLightElementMethods();
            //    Show();
            //}));
            //RequestManager.Start(param, new Action<Element, ElementPath>((x, y) =>
            //{
            //    if (x == default || y == default)
            //        return;
            //    firstPath = y;
            //}));


        }

        /// <summary>
        /// 加载元素详细信息
        /// </summary>
        /// <param name="locateElementTree">是否需要定位到目录树对应节点 从目录树节点来的不需要</param>
        void LoadElementMethod(bool locateElementTree = true)
        {
            if (SelectedItemInTree?._elementNode == null)
                return;
            if (SelectedItemInTree.ElementPath == null)
                SelectedItemInTree.ElementPath = winPathInspect.GetPathInfo(SelectedItemInTree._elementNode.CurrentElementWin.NativeElement);

            LocalItemTree = SelectedItemInTree;
            LoadPathNodeDetails();
            LoadImg();
            if (locateElementTree)
            {
                ElementPathHelper.FindElement(SelectedItemInTree.ElementPath, Elements); // 定位到目标节点
                SelectedItemInTree = LocalItemTree;
            }
            HighLightButton.Tag = "/Resources/高亮验证@3x.png";
        }

        /// <summary>
        /// 恢复详细信息
        /// </summary>
        void InitializedMethod()
        {
            LoadImg();
            LoadPathNodeDetails();
            HighLightButton.Tag = "/Resources/高亮验证@3x.png";
        }
        void LoadImg()
        {
            if (LocalItemTree?.ElementPath == null)
                return;
            ImagesViewModels.Clear();
            ImagesViewModels.Add(new ImagesViewModel
            {
                ElementPathId = LocalItemTree.ElementPath.Id,
                BoundingRectangle = LocalItemTree.ElementPath.BoundingRectangle,
                Img = LocalItemTree.ElementPath.Img
            });
        }

        /// <summary>
        /// 加载详细元素信息
        /// </summary>
        private void LoadPathNodeDetails()
        {
            var list = new List<PathNodeViewModel>();
            if (LocalItemTree == null)
                return;
            var node = LocalItemTree.ElementPath.ElementEditNodes;
            if (node == null)
                return;
            PathNodeDetails.Clear();
            NodeFiltDetails.Clear();
            int i = 1;
            node.ForEach(x =>
            {
                var viewModel = new PathNodeViewModel
                {
                    SortNumber = i,
                    IsActive = x.IsChecked,
                    Name = x.Name,
                    IsActiveName = x.GetProperty("Name").IsActive,
                    Index = x.Index,
                    IsActiveIndex = x.GetProperty("Index").IsActive,
                    Id = x.Id,
                    ControTypeName = EL.UIA.ControlTypeConverter.ToControlType(x.ControlType).ToString(),
                    ActiveChangeMethod = () =>
                    {
                        NodeFiltDetails.Clear();
                        foreach (var x in PathNodeDetails)
                        {
                            if (x.IsActive)
                                NodeFiltDetails.Add(x);
                        }
                        HighLightButton.Tag = "/Resources/高亮验证黄@2x.png";
                    }
                };
                viewModel.SetFeiltRes();
                PathNodeDetails.Add(viewModel);
                if (x.IsChecked)
                    NodeFiltDetails.Add(viewModel);
                i++;
            });
            LoadNodeDetailsMethod(PathNodeDetails.LastOrDefault());
        }

        /// <summary>
        /// 加载元素详细信息节点
        /// </summary>
        void LoadNodeDetailsMethod(object sender)
        {
            var nodeDetails = sender as PathNodeViewModel;
            if (nodeDetails is null)
                return;

            NodePropertyDetails.Clear();
            NodePropertyDetails.Add(new PathNodeDetails { Id = nodeDetails.Id, IsActive = nodeDetails.IsActiveName, PropertyName = nameof(nodeDetails.Name), PropertyValue = nodeDetails.Name, SortNumber = nodeDetails.SortNumber, ChangeMethod = new Action<PathNodeDetails>(PathNodeDetailsChangedMethod) });
            NodePropertyDetails.Add(new PathNodeDetails { Id = nodeDetails.Id, IsActive = nodeDetails.IsActiveIndex, PropertyName = nameof(nodeDetails.Index), PropertyValue = nodeDetails.Index, SortNumber = nodeDetails.SortNumber, ChangeMethod = new Action<PathNodeDetails>(PathNodeDetailsChangedMethod) });
        }

        /// <summary>
        /// 元素细节变更后修改筛选特征和筛选结果
        /// </summary>
        /// <param name="details"></param>
        void PathNodeDetailsChangedMethod(PathNodeDetails details)
        {
            var node = PathNodeDetails.FirstOrDefault(z => z.Id == details.Id);
            if (node == null)
                return;

            if (details.PropertyName == nameof(node.Name))
            {
                node.Name = details.PropertyValue as string;
                node.IsActiveName = details.IsActive;
            }
            else if (details.PropertyName == nameof(node.Index))
            {
                try
                {
                    node.Index = int.Parse(details.PropertyValue.ToString());
                }
                catch { }
                node.IsActiveIndex = details.IsActive;
            }
            node.SetFeiltRes();
            HighLightButton.Tag = "/Resources/高亮验证黄@2x.png";
        }

        /// <summary>
        /// 保存当前元素
        /// </summary>
        void SaveElmenetMethod()
        {
            GetSelectElementPath();
            if (LocalItemTree?.ElementPath == null)
                return;

            ElementPathHelper.UpdateElementPath(LocalItemTree.ElementPath, PathNodeDetails);
            var imgModel = ImagesViewModels.FirstOrDefault(x => x.ElementPathId == LocalItemTree.ElementPath.Id);
            if (imgModel != null)
            {
                LocalItemTree.ElementPath.Img = imgModel.Img;
                LocalItemTree.ElementPath.BoundingRectangle = imgModel.BoundingRectangle;
            }
            var entity = new ElementPathModelEntity(LocalItemTree.ElementPath);
            entity.CreateTime = entity.UpdateTime = DateTime.Now;
            var list = HistoryElements.ToList();
            var hasEntity = HistoryElements.FirstOrDefault(x => x.Id == entity.Id);
            if (hasEntity != null)
            {
                SqlComponent.ExcuteWithEtity($"UPDATE ElementPath SET UpdateTime=@UpdateTime,Name=@Name,ElementPathStr=@ElementPathStr WHERE Id=@Id;", entity);
                hasEntity = SqlComponent.GetEntity<ElementPathModelEntity>($"SELECT * FROM ElementPath WHERE Id={entity.Id}");
                LoadHistoryElement();
                ShowMessage("修改成功");
            }
            else
            {
                int i = 1;
                while (true)
                {
                    if (list.Exists(x => x.Name != null && x.Name == entity.Name))
                        entity.Name += i.ToString();
                    else
                        break;
                }
                HistoryElements.Insert(0, entity);
                SqlComponent.ExcuteWithEtity("INSERT INTO ElementPath VALUES (@Id ,@Name ,@ElementPathStr,@CreateTime,@UpdateTime);", entity);
                ShowMessage("保存成功");
            }

            if (CallBackAction == null)
                return;
            else
            {
                CallBackAction(LocalItemTree.ElementPath.PathNode.CurrentElementWin, LocalItemTree.ElementPath);
                window.Hide();
            }
        }

        void CloseWindowMethod()
        {
            if (CallBackAction == null)
                window.Close();
            else
            {
                CallBackAction(LocalItemTree.ElementPath.PathNode.CurrentElementWin, LocalItemTree.ElementPath);
                window.Hide();
            }
        }

        /// <summary>
        /// 加载选定历史元素到界面
        /// </summary>
        /// <param name="obj"></param>
        public void LoadHistoryElemntMethod(object obj)
        {
            var entity = obj as ElementPathModelEntity;
            if (entity == null)
                return;
            if (entity.Id == LocalItemTree?.ElementPath?.Id)
                return;

            LocalItemTree = SelectedItemInTree = new ElementViewModel(entity.ElementPath.PathNode) { ElementPath = entity.ElementPath };
            LoadElementMethod();
        }

        /// <summary>
        /// 外部调用接口 通过ElementPath展示路径详细信息
        /// </summary>
        /// <param name="elementPath"></param>
        public void LoadElementModelByRobot(ElementPath elementPath, Action<Automation.Inspect.Element, ElementPath> callback)
        {
            if (elementPath == null)
                return;
            var elementModel = SqlComponent.GetEntity<ElementPathModelEntity>($"select * from ElementPath where Id={elementPath.Id}");
            if (elementModel == null)
                elementModel = new ElementPathModelEntity(elementPath);
            LoadHistoryElemntMethod(elementModel);
            CallBackAction = callback;
        }


        /// <summary>
        /// 获取当前选中元素（加载前使用）
        /// </summary>
        void GetSelectElementPath()
        {
            if (LocalItemTree == null) // 当前节点为空不高亮显示
                return;
            if (LocalItemTree.ElementPath == null)
            {
                DispatcherHelper.UpdateUi(Hide);
                LocalItemTree.ElementPath = winPathInspect.GetPathInfo(LocalItemTree._elementNode.CurrentElementWin.NativeElement);
                Show();
            }
        }

        #region 左侧历史元素显示及删除

        public bool HistoryOrderByName
        {
            get { return GetProperty<bool>(); }
            set
            {
                SetProperty(value);
                if (value)
                {
                    HistoryOrderByCreateTime = false;
                    HistoryOrderByUpdateTime = false;
                    LoadHistoryElement();
                }
            }
        }
        public bool HistoryOrderByCreateTime
        {
            get { return GetProperty<bool>(); }
            set
            {
                SetProperty(value);
                if (value)
                {
                    HistoryOrderByName = false;
                    HistoryOrderByUpdateTime = false;
                    LoadHistoryElement();
                }
            }
        }
        public bool HistoryOrderByUpdateTime
        {
            get { return GetProperty<bool>(); }
            set
            {
                SetProperty(value);
                if (value)
                {
                    HistoryOrderByName = false;
                    HistoryOrderByCreateTime = false;
                    LoadHistoryElement();
                }
            }
        }
        public bool HistoryOrderUp
        {
            get { return GetProperty<bool>(); }
            set
            {
                SetProperty(value);
                if (value)
                {
                    HistoryOrderByDown = false;
                    LoadHistoryElement();
                }
            }
        }
        public bool HistoryOrderByDown
        {
            get { return GetProperty<bool>(); }
            set
            {
                SetProperty(value);
                if (value)
                {
                    HistoryOrderUp = false;
                    LoadHistoryElement();
                }
            }
        }

        /// <summary>
        /// 加载历史元素列表
        /// </summary>
        /// <param name="desField"></param>
        /// <param name="isDesc"></param>
        void LoadHistoryElement()
        {
            string sortField = "UpdateTime";
            if (HistoryOrderByName)
                sortField = "Name";
            else if (HistoryOrderByCreateTime)
                sortField = "CreateTime";

            string sortDesc = "DESC";
            if (HistoryOrderUp)
                sortDesc = "ASC";

            HistoryElements.Clear();
            var entityList = SqlComponent.GetEntitys<ElementPathModelEntity>($"SELECT * FROM ElementPath ORDER BY {sortField} {sortDesc}");
            entityList.ForEach(x =>
            {
                HistoryElements.Add(x);
            });
        }

        /// <summary>
        /// 删除历史元素
        /// </summary>
        /// <param name="obj"></param>
        void DeleteHistoryElemntMethod(object obj)
        {
            var entity = obj as ElementPathModelEntity;
            if (entity == null)
                return;

            SqlComponent.Excute($"DELETE FROM ElementPath WHERE Id={entity.Id}");
            HistoryElements.Remove(entity);
            ShowMessage("删除成功");
        }

        #endregion
        #endregion

        public void Initialize()
        {
            LoadHistoryElement();
            var node = new ElementNode();
            node.LevelIndex = 0;
            node.CurrentElementWin = winInspect.RootElement.Convert();
            node.GenerateCompareId();
            var desktopViewModel = new ElementViewModel(node);
            desktopViewModel.SelectionChanged += DesktopViewModel_SelectionChanged;

            Elements.Add(desktopViewModel);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Elements[0].IsExpanded = true;
            stopwatch.Stop();
            ShowMessage($"耗时{stopwatch.ElapsedMilliseconds}ms");

            //// Initialize TreeWalker
            //_treeWalker = _automation.TreeWalkerFactory.GetControlViewWalker();

            //// Initialize hover
            //_hoverMode = new HoverMode(_automation);
            //_hoverMode.ElementHovered += ElementToSelectChanged;

            //// Initialize focus tracking
            //_focusTrackingMode = new FocusTrackingMode(_automation);
            //_focusTrackingMode.ElementFocused += ElementToSelectChanged;
        }

        //private void ElementToSelectChanged(AutomationElement obj)
        //{
        //    // Build a stack from the root to the hovered item
        //    var pathToRoot = new Stack<AutomationElement>();
        //    while (obj != null)
        //    {
        //        // Break on circular relationship (should not happen?)
        //        if (pathToRoot.Contains(obj) || obj.Equals(_rootElement)) { break; }

        //        pathToRoot.Push(obj);
        //        try
        //        {
        //            obj = _treeWalker.GetParent(obj);
        //        }
        //        catch (Exception ex)
        //        {
        //            // TODO: Log
        //            Console.WriteLine($"Exception: {ex.Message}");
        //        }
        //    }

        //    // Expand the root element if needed
        //    if (!Elements[0].IsExpanded)
        //    {
        //        Elements[0].IsExpanded = true;
        //        System.Threading.Thread.Sleep(1000);
        //    }

        //    var elementVm = Elements[0];
        //    while (pathToRoot.Count > 0)
        //    {
        //        var elementOnPath = pathToRoot.Pop();
        //        var nextElementVm = FindElement(elementVm, elementOnPath);
        //        if (nextElementVm == null)
        //        {
        //            // Could not find next element, try reloading the parent
        //            elementVm.LoadChildren(true);
        //            // Now search again
        //            nextElementVm = FindElement(elementVm, elementOnPath);
        //            if (nextElementVm == null)
        //            {
        //                // The next element is still not found, exit the loop
        //                Console.WriteLine("Could not find the next element!");
        //                break;
        //            }
        //        }
        //        elementVm = nextElementVm;
        //        if (!elementVm.IsExpanded)
        //        {
        //            elementVm.IsExpanded = true;
        //        }
        //    }
        //    // Select the last element
        //    elementVm.IsSelected = true;
        //}

        private void DesktopViewModel_SelectionChanged(ElementViewModel obj)
        {
            SelectedItemInTree = obj;
            //OnPropertyChanged(() => SelectedItemDetails);
        }

        private void RefreshTree()
        {
            Elements.Clear();
            Initialize();
        }

        public bool EnableXPath
        {
            get { return GetProperty<bool>(); }
            set { SetProperty(value); }
        }

        public bool IsInitialized
        {
            get { return GetProperty<bool>(); }
            private set { SetProperty(value); }
        }
    }
}
