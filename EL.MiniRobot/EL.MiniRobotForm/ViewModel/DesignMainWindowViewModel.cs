using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EL.Robot.Core;
using EL;
using MiniRobotForm.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using MiniRobotForm.Mode;
using EL.Robot.Component;
using static EL.Robot.Core.ComponentSystem;
using System.Threading.Tasks;
using SixLabors.Fonts.Tables.AdvancedTypographic;
using System.Collections.Generic;
using EL.Robot;
using EL.Async;
using System.Windows.Input;

namespace ViewModel
{
    public abstract class ModelBase : ObservableObject
    {

    }

    public class DesignMainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<FeatureModel> _RobotListModels = new();
        public ObservableCollection<FeatureModel> RobotListModels { get => _RobotListModels; set => SetProperty(ref _RobotListModels, value); }


        private ObservableCollection<NodeModel> _SelectNodeModels = new();
        public ObservableCollection<NodeModel> SelectNodeModels
        {
            get => _SelectNodeModels;
            set => SetProperty(ref _SelectNodeModels, value);
        }


        public ObservableCollection<NodeComponentModel> SelectNodeComponent
        {
            get; set;
        }
        private NodeModel _SelectedItem;
        public NodeModel SelectedItem
        {
            get => _SelectedItem;
            set => SetProperty(ref _SelectedItem, value);
        }
        private FeatureModel _CurrentModel;
        public FeatureModel CurrentModel
        {
            get => _CurrentModel;
            set
            {
                SetProperty(ref _CurrentModel, value);
                StartDesign();
            }
        }

        public ObservableCollection<NodeComponentModel> NodeComponentModels { get; set; } = new ObservableCollection<NodeComponentModel>();


        public async void StartDesign()
        {
            if (CurrentModel == null) return;
            var flow = designComponent.StartDesign(CurrentModel.Id);
            CurrentRobotModel = ModelConvert.To<Flow, FlowModel>(flow);
        }
        private FlowModel _CurrentRobotModel;
        public FlowModel CurrentRobotModel
        {
            get => _CurrentRobotModel;
            set
            {
                SetProperty(ref _CurrentRobotModel, value);
            }
        }
        /// <summary>
        /// 实用案例
        /// </summary>
        private UserControl _ComponentWindow = new ComponentWindow();
        public UserControl ComponentWindow
        {
            get { return _ComponentWindow; }
            set { SetProperty(ref _ComponentWindow, value); }
        }
        private DesignComponent designComponent = Boot.GetComponent<RobotComponent>().GetComponent<DesignComponent>();
        public DesignMainWindowViewModel()
        {
            var list = designComponent.LoadRobots();
            RobotListModels = ModelConvert.TosUI<Feature, FeatureModel>(list);
            GetCompoenntTreeAsync().Coroutine();
        }
        public RelayCommand<object> ComponentMouseDownCommand => new((e) =>
        {
            //System.Windows.Forms.MessageBox.Show(SelectedItem.Name);
        });
        public RelayCommand CreateRobotCommand => new(() =>
        {
            var createRobot = new CreateRobotWindow();
            var rtn = createRobot.ShowDialog();
            if (!rtn ?? true) return;
            RobotListModels = ModelConvert.TosUI<Feature, FeatureModel>(designComponent.Features);
        });
        private async ELTask GetCompoenntTreeAsync()
        {
            CommponetRequest commponetRequest = new CommponetRequest()
            {
                ComponentName = nameof(ComponentSystem),
                Data = new ComponentSystemReuqest
                {

                    Action = nameof(ComponentSystem.GetCategorys)
                }
            };
            var categorys = await RequestManager.StartAsync(commponetRequest);
            foreach (var item in categorys.Data)
            {
                NodeComponentModel temp = new NodeComponentModel()
                {
                    Id = Helper.GetValue<int>(item, "CategoryId") + "",
                    Name = Helper.GetValue<string>(item, "CategoryName")
                };
                CommponetRequest commponetRequest1 = new CommponetRequest()
                {
                    ComponentName = nameof(ComponentSystem),
                    Data = new ComponentSystemReuqest
                    {
                        Action = nameof(ComponentSystem.GetComponentsById),
                        Paramters = new Dictionary<string, object>()
                                {
                                    {"Id",  Helper.GetValue<int>(item, "CategoryId") }
                                }
                    }
                };
                var componentConfigs = await RequestManager.StartAsync(commponetRequest1);
                if (componentConfigs == null || componentConfigs.Data == null) continue;
                foreach (Config component in componentConfigs.Data)
                {
                    temp.Children.Add(new NodeComponentModel()
                    {
                        Name = component.ButtonDisplayName,
                        Id = component.ComponentName
                    });
                }
                NodeComponentModels.Add(temp);
            }

        }

    }
    public class NodeComponentModel : ModelBase
    {
        public string Id { get; set; }
        public string Name { get; set; }
        private ObservableCollection<NodeComponentModel> _Children = new();
        public ObservableCollection<NodeComponentModel> Children
        {
            get => _Children;
            set => SetProperty(ref _Children, value);
        }
    }
    public class NodeModel : ModelBase
    {
        private int _Index;
        public int Index { get => _Index; set => SetProperty(ref _Index, value); }

        private string _Name;
        public string Name
        {
            get => _Name; set => SetProperty(ref _Name, value);
        }
        private long _Id;
        private string _Note;
        public string Note { get => _Note; set => SetProperty(ref _Note, value); }

        public long Id { get => _Id; set => SetProperty(ref _Id, value); }
        private ObservableCollection<NodeModel> _Children = new();
        public ObservableCollection<NodeModel> Children
        {
            get => _Children;
            set => SetProperty(ref _Children, value);
        }
    }
    public class RobotFlowModel : ModelBase
    {
        private long _Id;
        public long Id { get => _Id; set => SetProperty(ref _Id, value); }
        private string _Image;
        public string Image
        {
            get => _Image;
            set => SetProperty(ref _Image, value);
        }
        private string _Name;
        public string Name
        {
            get => _Name;
            set => SetProperty(ref _Name, value);
        }
        private string _Note;
        public string Note
        {
            get => _Note;
            set => SetProperty(ref _Note, value);
        }
        private long _Update;
        public long Update
        {
            get => _Update;
            set => SetProperty(ref _Update, value);
        }
        private ObservableCollection<NodeModel> _Children = new();
        public ObservableCollection<NodeModel> Children
        {
            get => _Children;
            set => SetProperty(ref _Children, value);
        }
    }
}
