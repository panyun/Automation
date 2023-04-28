using EL.Robot.Component;
using EL.Robot.WindowApiTest.bin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EL.Robot.WindowApiTest
{

    public partial class DesignViewForm : UserControl
    {
        public Action<Flow> InitAction { get; set; }
        public Action<Node, bool> HideExpansionAction { get; set; }
        public Flow Flow { get; set; }
        public List<DesignRowViewForm> designRowViewForms { get; set; }
        public static DesignViewForm Ins;
        public List<DesignRowViewForm> HideExpansionControls = new List<DesignRowViewForm>();
        public DesignViewForm()
        {
            InitializeComponent();
            Ins = this;
            InitAction = (x) =>
            {
                this.Flow = x;
                //创建控件
                DesignRowViewForm designRowViewForm;
                int index = 0;
                int left = 50;
                int leftTemp = 0;
                List<Node> nodes = new();
                foreach (var item in Flow.DesignSteps)
                {
                    index++;
                    designRowViewForm = new();
                    var isExists = nodes.Exists(x => x.Id == item.Id);
                    if (isExists)
                        leftTemp += 50;
                    var row = GetRowInfo(item, index, left - leftTemp);
                    designRowViewForm.BorderStyle = BorderStyle.None;
                    designRowViewForm.Tag = item;
                    designRowViewForm.UpdateAction(row);
                    this.pl_Content.Controls.Add(designRowViewForm);
                    if (item.LinkNode != null)
                    {
                        nodes.Add(item.LinkNode);
                        leftTemp -= 50;
                    }

                }
            };
            HideExpansionAction = (x, hide) =>
            {
                var childs = Flow.DesignSteps.Where(x => x.DesignParent != null && x.DesignParent.Id == x.Id);
                if (childs == null) return;
                if (hide)
                {
                    var controls = new List<DesignRowViewForm>();
                    foreach (var child in childs)
                    {
                        foreach (DesignRowViewForm control in pl_Content.Controls)
                        {
                            var node = (Node)control.Tag;
                            if (node.Id == child.Id)
                            {
                                controls.Add(control);
                            }
                        }
                    }
                    controls.ForEach(x => { HideExpansionControls.Add(x); this.pl_Content.Controls.Remove(x); });
                }

                if (!hide)
                {
                    var controls = new List<DesignRowViewForm>();
                    foreach (var child in childs)
                    {
                        foreach (DesignRowViewForm control in HideExpansionControls)
                        {
                            var node = (Node)control.Tag;
                            if (node.Id == child.Id)
                            {
                                pl_Content.Controls.Add(control);
                                controls.Add(control);
                            }
                        }
                    }
                    controls.ForEach(x => HideExpansionControls.Remove(x));
                }
                this.Refresh();
                this.Update();
            };
        }
        public RowInfo GetRowInfo(Node node, int index, int left = 50)
        {
            return new RowInfo()
            {
                Id = node.Id,
                DisplayExp = node.DisplayExp,
                Index = index + "",
                Name = node.Name,
                Left = left,
                IsBlock = node.IsBlock,
            };
        }
    }
}
