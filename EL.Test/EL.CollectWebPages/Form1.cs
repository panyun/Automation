using EL.CollectWebPages.BLL;
using EL.CollectWebPages.Common;
using EL.CollectWebPages.Model;
using Interop.UIAutomationClient;
using System.Diagnostics;

namespace EL.CollectWebPages;

public partial class Form1 : Form
{
    TaskManage taskManage;
    ConfigInfo<PageConfig> pageConfig;
    ConfigInfo<List<UrlInfo>> UrlInfo;
    public Form1()
    {
        InitializeComponent();
        pageConfig = new ConfigInfo<PageConfig>("config.json");
        //UrlInfo = new ConfigInfo<List<UrlInfo>>("urltasks.json");
        taskManage = new TaskManage(
            pageConfig.CurrentConfig.ExePath,
            pageConfig.CurrentConfig.RootPath,
            new FileTaskManage(pageConfig.CurrentConfig.ExcelPath, pageConfig.CurrentConfig.startIndex, pageConfig.CurrentConfig.endIndex),
            pageConfig.CurrentConfig.UrlCount
        );

        taskManage.ShowMessage = (str) =>
        {
            this.Invoke(new Action(() =>
            {
                label2.Text = str;
            }));
        };
        Task.Run(() =>
        {
            while (true)
            {
                if (taskManage.NeedStart || taskManage.IsRun && (DateTime.Now - taskManage.LastTime).TotalMinutes > 2)
                {
                    taskManage.Close();
                    SpinWait.SpinUntil(() => false, 2000);
                    if (taskManage.NeedStart || taskManage.IsRun && (DateTime.Now - taskManage.LastTime).TotalMinutes > 2)
                    {
                        this.Invoke(() =>
                        {
                            button1_Click(null, null);
                        });
                    }
                }
                SpinWait.SpinUntil(() => false, 8000);
            }
        });
    }
    private void button1_Click(object sender, EventArgs e)
    {
        taskManage.StartView();
    }

    private void button2_Click(object sender, EventArgs e)
    {
        taskManage.Close();
    }
    private void button3_Click(object sender, EventArgs e)
    {
        new Form2().Show();
    }
}