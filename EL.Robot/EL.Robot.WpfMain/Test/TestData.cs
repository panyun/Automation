using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robot.Test
{
    //public class Data1 : List<DataItem>
    //{
    //    public Data1()
    //    {
    //        this.AddRange(new TestData().Data1);
    //    }
    //}

    //public class Data2 : List<NodeItem>
    //{
    //    public Data2()
    //    {
    //        this.AddRange(new TestData().Data2);
    //    }
    //}

    //public class Data3 : List<string>
    //{
    //    public Data3()
    //    {
    //        this.AddRange(
    //            new object[10].Select((a, x) => $"测试：{x}-111111122222ee").ToList()
    //            );
    //    }
    //}

    class TestData
    {
        public List<Scrpit> Data1 { get; set; } = new List<Scrpit> {

           new Scrpit
           {
                Name="脚本1",
                Children=new List<Plan>(){
                    new Plan { F1= "00:00:00，每天" ,F2= "预计下次2022-11-28 00:00:00执行，还有10分钟" , Stoped = false},
                    new Plan { F1= "00:00:00,周一、周二、周日" ,F2= "预计下次2022-11-28 00:00:00执行，还有10分钟" , Stoped = true},
                    new Plan { F1= "21:20，每月28" ,F2= "预计下次2022-12-28 21:20执行，还有30天" , Stoped = true},
                }
           }
        };

        public List<Scrpit> Data2 { get; set; } = new List<Scrpit> {

           new Scrpit
           {
                Name="这是一个运行中的脚本1",
                Dscribe="2022-11-28 18:00开始，已运行5分钟",
                Type=1
           },
            new Scrpit
           {
                Name="这是一个运行中的脚本2",
                Dscribe="2022-11-28 18:00开始，已运行10分钟",
                Type=1
           }
        };

        public List<Scrpit> Data3 { get; set; } = new List<Scrpit> {

           new Scrpit
           {
                Name="这是一个排队中的脚本1",
                Dscribe="前面还有1个脚本",
                Type=2
           },
            new Scrpit
           {
                Name="这是一个排队中的脚本2",
                Dscribe="前面还有2个脚本",
                Type=2
           },
            new Scrpit
           {
                Name="这是一个排队中的脚本3",
                Dscribe="前面还有3个脚本",
                Type=2
           }
        };

       /* public List<EL.Robot.WpfMain.ViewModel.HistoryItem> Data4 { get; set; } = new List<EL.Robot.WpfMain.ViewModel.HistoryItem> {
         new EL.Robot.WpfMain.ViewModel.HistoryItem{ StartTime =DateTime.Now.AddDays(-10),EndTime = DateTime.Now.AddDays(-3), ScriptName="流程脚本123",State="执行异常" },
               new EL.Robot.WpfMain.ViewModel.HistoryItem{ StartTime =DateTime.Now.AddDays(-10),EndTime = DateTime.Now.AddDays(-3), ScriptName="流程脚本121",State="执行异常" },
                     new EL.Robot.WpfMain.ViewModel.HistoryItem{ StartTime =DateTime.Now.AddDays(-10),EndTime = DateTime.Now.AddDays(-3), ScriptName="流程脚本11",State="执行异常" },
                           new EL.Robot.WpfMain.ViewModel.HistoryItem{ StartTime =DateTime.Now.AddDays(-10),EndTime = DateTime.Now.AddDays(-3), ScriptName="流程脚本443",State="执行异常" },
                                 new EL.Robot.WpfMain.ViewModel.HistoryItem{ StartTime =DateTime.Now.AddDays(-10),EndTime = DateTime.Now.AddDays(-3), ScriptName="流程脚本2123",State="执行异常" },
                                       new EL.Robot.WpfMain.ViewModel.HistoryItem{ StartTime =DateTime.Now.AddDays(-10),EndTime = DateTime.Now.AddDays(-3), ScriptName="流程脚本43",State="执行异常" },
                                             new EL.Robot.WpfMain.ViewModel.HistoryItem{ StartTime =DateTime.Now.AddDays(-10),EndTime = DateTime.Now.AddDays(-3), ScriptName="流程脚本1223",State="执行异常" },

        };*/
    }

    //public class DataItem
    //{
    //    public string F1 { get; set; }
    //    public string F2 { get; set; }
    //    public string F3 { get; set; }
    //    public string F4 { get; set; }
    //}    

    //public class NodeItem
    //{
    //    public string Name { get; set; }

    //    public List<NodeItem> Children { get; set; } = new List<NodeItem>();
    //}

    public class Scrpit
    {
        public int Type { get; set; }
        public string Name { get; set; }

        public string Dscribe { get; set; }

        public List<Plan> Children { get; set; } = new List<Plan>();
    }

    public class Plan
    {

        public string F1 { get; set; }
        public string F2 { get; set; }

        public bool Stoped { get; set; }
    }


}
