using Automation;
using Automation.Inspect;
using Automation.Parser;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFromInspect
{
    public partial class DataTableForm : Form
    {
        public DataTableForm()
        {
            InitializeComponent();
        }

        private void DataTableForm_Load(object sender, EventArgs e)
        {
            //BindingSource mbs = new();
            //mbs.DataSource = dt;

        }

        private void dataGridView1_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            //显示在HeaderCell上
            for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
            {
                DataGridViewRow r = this.dataGridView1.Rows[i];
                r.HeaderCell.Value = string.Format("{0}", i + 1);
            }
            this.dataGridView1.Refresh();
        }

        private async void button1_ClickAsync(object sender, EventArgs e)
        {
            CatchUIRequest catchElementRequest = new CatchUIRequest()
            {
                Msg = "打开界面探测器"
            };
            var res = (CatchUIResponse)await InspectRequestManager.StartAsync(catchElementRequest);
            if (res.Error > 0 || res.ElementPath == default)
            {
                MessageBox.Show(res.Message);
                return;
            }
            GenerateTableActionRequest generateTableActionRequest = new GenerateTableActionRequest()
            {
                ElementPath = res.ElementPath,
            };
            var generateTableActionResponse = (GenerateTableActionResponse)await InspectRequestManager.StartAsync(generateTableActionRequest);
            if (generateTableActionResponse.Error != 0 || generateTableActionResponse.DataTable == null)
            {
                MessageBox.Show(generateTableActionResponse.Message);
                return;
            }
            Bind(generateTableActionResponse.DataTable);
        }
        public void Bind(DataTable dt)
        {
            this.dataGridView1.DataSource = dt;
            this.dataGridView1.AutoResizeRows();
            this.dataGridView1.AutoResizeColumns();
            this.Activate();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            CatchUIRequest catchElementRequest = new CatchUIRequest()
            {
                Msg = "打开界面探测器"
            };
            var res = (CatchUIResponse)await InspectRequestManager.StartAsync(catchElementRequest);
            if (res.Error > 0 || res.ElementPath == default)
            {
                MessageBox.Show(res.Message);
                return;
            }
            GenerateTableActionRequest generateTableActionRequest = new GenerateTableActionRequest()
            {
                ElementPath = res.ElementPath,
            };
            var generateTableActionResponse = (GenerateTableActionResponse)await InspectRequestManager.StartAsync(generateTableActionRequest);
            if (generateTableActionResponse.Error != 0 || generateTableActionResponse.DataTable == null)
            {
                MessageBox.Show(generateTableActionResponse.Message);
                return;
            }
            Bind(generateTableActionResponse.DataTable);
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            GenerateExcelDataActionRequest generateTableActionRequest = new GenerateExcelDataActionRequest()
            {
                ExcelPath = textBox1.Text.Trim()
            };
            var generateTableActionResponse = (GenerateExcelDataActionResponse)await InspectRequestManager.StartAsync(generateTableActionRequest);
            if (generateTableActionResponse.Error != 0 || generateTableActionResponse.DataTable == null)
            {
                MessageBox.Show(generateTableActionResponse.Message);
                return;
            }
            Bind(generateTableActionResponse.DataTable);
        }
    }
}
