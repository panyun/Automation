using Automation;
using Automation.Inspect;
using Automation.Parser;
using HtmlAgilityPack;

namespace WinFromInspect
{
    public partial class PlaywrightForm : Form
    {
        public PlaywrightForm()
        {
            InitializeComponent();
        }
        ElementPath elementPath = default;
        private async void button1_Click(object sender, EventArgs e)
        {
            CatchUIRequest catchElementRequest = new CatchUIRequest() { Msg = "打开界面探测器" };
            var res = (CatchUIResponse)await InspectRequestManager.StartAsync(catchElementRequest);
            if (res == default || res.Error > 0)
            {
                return;
            }
            elementPath = res.ElementPath;
            GenerateHtmlActionRequest request = new GenerateHtmlActionRequest()
            {
                ElementPath = res.ElementPath
            };
            var resHtml = (GenerateHtmlActionResponse)await InspectRequestManager.StartAsync(request);
            this.Invoke(() =>
            {
                textBox1.Text = resHtml.Html;
            });
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml("<body>" + resHtml.Html + "</body>");
            //var node = doc.DocumentNode;
            //GenerateXpathFromHtml(resHtml.Html, "");
            //GenerateCssSelectorFromHtml(resHtml.Html, "");
            var node = BindTreeNode(doc.DocumentNode);
            this.Invoke(() =>
            {
                treeView1.Nodes.Clear();
                treeView1.Nodes.Add(node);
                treeView1.Refresh();
            });

        }
        public TreeNode BindTreeNode(HtmlNode htmlNode)
        {
            var node = new TreeNode
            {
                Name = htmlNode.Name,
                Text = htmlNode.Name + " : " + htmlNode.GetDirectInnerText(),
                Tag = htmlNode
            };
            if (!htmlNode?.ChildNodes.Any() ?? true) return node;
            foreach (var item in htmlNode.ChildNodes)
            {
                if (item.Name.Contains("script"))
                    continue;
                node.Nodes.Add(BindTreeNode(item));
            }
            return node;
        }
        public static string GenerateXpathFromHtml(string html, string elementName)
        {
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml("<body>" + html + "</body>");
            var node = doc.DocumentNode.SelectSingleNode($@"//a");
            return node.XPath;
        }
        public static string GenerateCssSelectorFromHtml(string html, string elementName)
        {
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);
            var node = doc.DocumentNode.SelectSingleNode($"//a");
            var cssSelector = node.Attributes["class"] != null ? "." + node.Attributes["class"].Value : "";
            cssSelector += node.Attributes["id"] != null ? "#" + node.Attributes["id"].Value : "";
            return cssSelector;
        }
        private async void treeView1_MouseDown(object sender, MouseEventArgs e)
        {
            Point ClickPoint = new Point(e.X, e.Y);
            TreeNode CurrentNode = treeView1.GetNodeAt(ClickPoint);
            if (CurrentNode != null && CurrentNode.Tag != null && CurrentNode.Tag is HtmlNode node)
            {

                textBox2.Text = "xpath=/html" + node.XPath;
                HighlightActionRequest highlightActionRequest = new HighlightActionRequest()
                {
                    //LightProperty = new LightProperty()
                    //{
                    //    Count = 3,
                    //    Time = 500
                    //},
                    ElementPath = elementPath,
                    Title = elementPath.NativeWindowTitle,
                    XPath = "xpath=/html" + node.XPath
                };
                var res = (HighlightActionResponse)await InspectRequestManager.StartAsync(highlightActionRequest);
                //SetForeground();
            }
        }
        private void SetForeground()
        {
            this.Invoke(() =>
            {
                this.Handle.SetForeground();
                this.Activate();
            });
        }
    }
}
