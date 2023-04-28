using EL.Robot.Component;
using EL.Robot.WindowApiTest.Properties;
using Google.Protobuf;

namespace EL.Robot.WindowApiTest.bin
{
    public partial class DesignRowViewForm : UserControl
    {

        public RowInfo RowInfo { get; set; }
        public Action<RowInfo> UpdateAction { get; set; }

        public DesignRowViewForm()
        {
            InitializeComponent();
            UpdateAction = Update;
            pic_block.Click += (x, y) =>
            {
                if ((bool)pic_block.Tag)
                {
                    pic_block.Image = Resources.Expansion;
                    pic_block.Tag = false;
                }
                else
                {
                    pic_block.Image = Resources.Hide;
                    pic_block.Tag = true;

                }
                DesignViewForm.Ins.HideExpansionAction?.Invoke((Node)Tag, (bool)pic_block.Tag);
            };
            Update(default);
        }
        public DesignRowViewForm(RowInfo rowInfo)
        {
            InitializeComponent();
            UpdateAction = Update;
            Update(rowInfo);
        }
        public void Update(RowInfo row)
        {
            if (row == null) return;
            lbl_Name.Text = row.Name;
            lbl_Index.Text = row.Index + "";
            lbl_DisplayCmd.Text = row.DisplayExp;
            pl_Cmd.Left = row.Left;
            pic_block.Visible = row.IsBlock;
            pic_block.Image = Resources.Hide;
            pic_block.Tag = false;
            this.Refresh();
        }
    }
    public class RowInfo
    {
        public long Id { get; set; }
        public string Index { get; set; }
        public string Name { get; set; }
        public string DisplayExp { get; set; }
        public Image Image { get; set; }
        public bool IsBlock { get; set; }
        public int Left { get; set; } = 550;

    }
}
