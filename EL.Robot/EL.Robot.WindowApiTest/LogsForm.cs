using EL.Robot.Core;
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
    public partial class LogsForm : Form
    {
        public Action<DesignMsg> LogsAction { get; set; }
        public Action LogsClearAction { get; set; }
        public LogsForm()
        {
            InitializeComponent();
            LogsClearAction = () => { richTextBox1.Text = ""; };
            LogsAction = (x) =>
            {
                this.Invoke(new Action(() =>
                {
                    AppendTextColorful(richTextBox1, x.ShowMsg, Color.Black, new Font("黑体", 10), true);

                }));
            };
        }
        public void AppendTextColorful(RichTextBox rtBox, string addtext, Color color, Font font, bool IsaddNewLine)
        {
            if (IsaddNewLine)
            {
                addtext += Environment.NewLine;
            }
            rtBox.SelectionStart = rtBox.TextLength;
            rtBox.SelectionLength = 0;
            rtBox.SelectionFont = font;
            rtBox.SelectionColor = color;
            rtBox.AppendText(addtext);
            rtBox.SelectionColor = rtBox.ForeColor;
        }
    }
}
