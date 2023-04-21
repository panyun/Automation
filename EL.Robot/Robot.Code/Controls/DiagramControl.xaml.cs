using GraphX.Controls;
using GraphX.Common.Enums;
using GraphX.Logic.Algorithms.LayoutAlgorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Robot.Controls
{
    /// <summary>
    /// DiagramControl.xaml 的交互逻辑
    /// </summary>
    public partial class DiagramControl : UserControl
    {
        public DiagramControl()
        {
            InitializeComponent();

            Loaded += DiagramControl_Loaded;
        }

        private void DiagramControl_Loaded(object sender, RoutedEventArgs e)
        {
            InitDiagram();

            BulidTestData();

            tg_zoomctrl.ZoomToFill();
            tg_zoomctrl.Zoom = .95;
        }

        //初始化流程图控件
        public void InitDiagram()
        {
            var logic = new LogicCoreExample();
            tg_Area.LogicCore = logic;
            logic.DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.Tree;
            logic.DefaultOverlapRemovalAlgorithm = OverlapRemovalAlgorithmTypeEnum.FSA;
            logic.DefaultOverlapRemovalAlgorithmParams.HorizontalGap = 50;
            logic.DefaultOverlapRemovalAlgorithmParams.VerticalGap = 50;
            logic.DefaultEdgeRoutingAlgorithm = EdgeRoutingAlgorithmTypeEnum.SimpleER;
            logic.EdgeCurvingEnabled = true;
            logic.AsyncAlgorithmCompute = false;

            tg_Area.SetVerticesDrag(true);

            var tree = tg_Area.LogicCore.AlgorithmFactory.CreateLayoutParameters(LayoutAlgorithmTypeEnum.Tree) as SimpleTreeLayoutParameters;
            tree.VertexGap = 80;
            tree.LayerGap = 80;
            tg_Area.LogicCore.DefaultLayoutAlgorithmParams = tree;

            tg_Area.VertexSelected += Tg_Area_VertexSelected; ;
            tg_Area.GenerateGraphFinished += Tg_Area_GenerateGraphFinished;
            tg_Area.RelayoutFinished += Tg_Area_RelayoutFinished;


            ZoomControl.SetViewFinderVisibility(tg_zoomctrl, Visibility.Hidden);
        }

        //重新布局完成事件
        private void Tg_Area_RelayoutFinished(object sender, EventArgs e)
        {

        }

        //绘图完成事件
        private void Tg_Area_GenerateGraphFinished(object sender, EventArgs e)
        {

        }

        //节点选中事件
        private void Tg_Area_VertexSelected(object sender, GraphX.Controls.Models.VertexSelectedEventArgs args)
        {

        }

        public int DataType { get; set; }

        private static int _uid = 0;
        private static int UID => _uid++;
        private void BulidTestData()
        {
            var graph = new GraphExample();

            if (DataType == 0)
            {
                var nodes = new List<DataVertex> {
                new DataVertex { ID = UID, Name = "打开网页", ImageKey = "1", Type = 1,ShowOutLine=true },
                new DataVertex { ID = UID, Name = "流程块1", ImageKey = "2", Type = 2 },
                new DataVertex { ID = UID, Name = "流程块2", ImageKey = "3", Type = 3 },
                new DataVertex { ID = UID, Name = "打开邮件", ImageKey = "1", Type = 1 },
                new DataVertex { ID = UID, Name = "判断组件", ImageKey = "2", Type = 4,IsSelected=true }
                };
                graph.AddVertexRange(nodes);
                graph.AddEdge(new DataEdge(nodes[0], nodes[1], 1) { LineType = EdgeDashStyle.Dot, LineColor = "#358BF8", ID = UID });
                graph.AddEdge(new DataEdge(nodes[1], nodes[2], 1) { LineType = EdgeDashStyle.Solid, LineColor = "#99999B", ID = UID, ShowAddButton = true });
                graph.AddEdge(new DataEdge(nodes[2], nodes[3], 1) { LineType = EdgeDashStyle.Solid, LineColor = "#99999B", ID = UID, ShowAddButton = true });
                graph.AddEdge(new DataEdge(nodes[3], nodes[4], 1) { LineType = EdgeDashStyle.Solid, LineColor = "#358BF8", ID = UID });
            }
            else
            {
                var nodes = new List<DataVertex> {
                new DataVertex { ID = UID, Name = "打开网页", ImageKey = "1", Type = 1,ShowOutLine=true },
                new DataVertex { ID = UID, Name = "流程块1", ImageKey = "2", Type = 2 },
                new DataVertex { ID = UID, Name = "流程块2", ImageKey = "3", Type = 3 },
                new DataVertex { ID = UID, Name = "打开邮件", ImageKey = "1", Type = 1 },
                new DataVertex { ID = UID, Name = "流程2", ImageKey = "2", Type = 2  }
                };
                graph.AddVertexRange(nodes);

                graph.AddEdge(new DataEdge(nodes[0], nodes[1], 1) { LineType = EdgeDashStyle.Dot, LineColor = "#358BF8", ID = UID, ShowArrow = false });
                graph.AddEdge(new DataEdge(nodes[1], nodes[2], 1) { LineType = EdgeDashStyle.Solid, LineColor = "#00000000", ID = UID });
                graph.AddEdge(new DataEdge(nodes[2], nodes[3], 1) { LineType = EdgeDashStyle.Solid, LineColor = "#00000000", ID = UID });
                graph.AddEdge(new DataEdge(nodes[3], nodes[4], 1) { LineType = EdgeDashStyle.Solid, LineColor = "#00000000", ID = UID });
            }

            tg_Area.GenerateGraph(graph, true);
        }
    }
}
