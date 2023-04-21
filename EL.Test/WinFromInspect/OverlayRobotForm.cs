#if NETFRAMEWORK || NETCOREAPP

using EL.Input;
using EL.WindowsAPI;
using WinFromInspect.Properties;

namespace EL.Overlay
{
    public class OverlayRobotForm : Form
    {
        Color lineColor;
        Panel panel;
        private PictureBox pictureBox1;
        Label lable;
        public OverlayRobotForm()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
          //  Location = new Point(1200,600);
            Opacity = 0.8;
            Width = 250;
            Height = 250;
            Left = 1000;
            Top = 600;
            Visible = false;
            TransparencyKey = Color.White;
            BackColor = Color.Blue;
            Cursor = Cursors.Arrow;
            panel = new Panel();
            panel.Name = "panel";
            panel.TabIndex = 0;
            //panel.Paint += Panel_Paint;
            panel.BackColor = Color.Transparent;
            panel.Dock = DockStyle.Fill;
            pictureBox1.Image = Resources._03b25467442038abd24fadb58ce1f7c.GetThumbnailImage(pictureBox1.Width, pictureBox1.Height, default, default);
            //this.TopMost = true;
        }

        public void ShowEx()
        {
            this.Invoke(() =>
            {
                this.Show();
            });
        }
        public void HideEx()
        {
            this.Invoke(() =>
            {
                this.Hide();
            });
        }
        public void LightHigh(dynamic element, Color color)
        {
            var rectangle = element.BoundingRectangle;
            var type = element.ControlTypeName;
            this.Invoke(() =>
            {
                Location = new Point(rectangle.X, rectangle.Y);
                this.StartPosition = FormStartPosition.WindowsDefaultBounds;
                SetDesktopLocation(rectangle.X, rectangle.Y);
                Width = rectangle.Width;
                Height = rectangle.Height;
                lineColor = color;
                lable.Visible = false;
            });
        }
        //protected override void WndProc(ref Message m)
        //{
        //    base.WndProc(ref m);
        //    var hotkey = FormOverLayComponent.Instance.GetComponent<HotkeyComponent>();
        //    hotkey.ProcessHotKey(m);
        //}
        private void Panel_Paint(object sender, PaintEventArgs e)
        {
            System.Drawing.Graphics graphics = e.Graphics;
            ControlPaint.DrawBorder(e.Graphics,
                             ((Panel)sender).ClientRectangle,
                              lineColor,//7f9db9
                              3,
                              ButtonBorderStyle.Solid,
                              lineColor,
                              3,
                              ButtonBorderStyle.Solid,
                              lineColor,
                              3,
                              ButtonBorderStyle.Solid,
                              lineColor,
                              3,
                              ButtonBorderStyle.Solid);
        }

        private void OverlayRectangleForm_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics,
                             this.ClientRectangle,
                             lineColor,//7f9db9
                             2,
                             ButtonBorderStyle.Solid,
                             lineColor,
                             2,
                             ButtonBorderStyle.Solid,
                             lineColor,
                             2,
                             ButtonBorderStyle.Solid,
                             lineColor,
                             2,
                             ButtonBorderStyle.Solid);
        }
        protected override bool ShowWithoutActivation => true;

        protected override CreateParams CreateParams
        {
            get
            {
                var result = base.CreateParams;
                result.ExStyle |= (int)WindowStyles.WS_EX_TOOLWINDOW;
                result.ExStyle |= (int)WindowStyles.WS_EX_TRANSPARENT;
                result.ExStyle |= (int)WindowStyles.WS_EX_NOACTIVATE;
                result.ExStyle |= (int)WindowStyles.WS_EX_LAYERED;
                result.ExStyle |= (int)WindowStyles.WS_EX_TOPMOST;
                return result;
            }
        }

        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1148, 941);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // OverlayRobotForm
            // 
            this.ClientSize = new System.Drawing.Size(1148, 941);
            this.Controls.Add(this.pictureBox1);
            this.Name = "OverlayRobotForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
#endif