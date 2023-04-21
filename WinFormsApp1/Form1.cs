using Automation;
using Automation.Inspect;
using EL;
using System.Text.Json.Nodes;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string param = "10000{\"$type\":\"Automation.Inspect.CatchElementRequest, EL.Automation\",\"RpcId\":1}";
                var path = await Program.RequestInfo.StartNodeJs(param);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}