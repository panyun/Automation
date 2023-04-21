using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robot.Test
{
    public class Data1 : List<DataItem>
    {
        public Data1()
        {
            this.AddRange(new TestData().Data1);
        }
    }

    public class Data2 : List<NodeItem>
    {
        public Data2()
        {
            this.AddRange(new TestData().Data2);
        }
    }

    public class Data3 : List<string>
    {
        public Data3()
        {
            this.AddRange(
                new object[10].Select((a, x) => $"测试：{x}-111111122222ee").ToList()
                );
        }
    }

    class TestData
    {
        public List<DataItem> Data1 { get; set; } = new List<DataItem> {
            new DataItem {
              F1="测试字段1",
              F2="测试字段2",
              F3="测试字段3",
              F4="测试字段4",},
                  new DataItem {
              F1="测试字段1",
              F2="测试字段2",
              F3="测试字段3",
              F4="测试字段4",},
                   new DataItem {
              F1="测试字段1",
              F2="测试字段2",
              F3="测试字段3",
              F4="测试字段4",},
                   new DataItem {
              F1="测试字段1",
              F2="测试字段2",
              F3="测试字段3",
              F4="测试字段4",},
                    new DataItem {
              F1="测试字段1",
              F2="测试字段2",
              F3="测试字段3",
              F4="测试字段4",},
                  new DataItem {
              F1="测试字段1",
              F2="测试字段2",
              F3="测试字段3",
              F4="测试字段4",},
                  new DataItem {
              F1="测试字段1",
              F2="测试字段2",
              F3="测试字段3",
              F4="测试字段4",},
        };

        public List<NodeItem> Data2 { get; set; } = new List<NodeItem> {
              new NodeItem{ Name="层级1"},
               new NodeItem{ Name="层级2"},
               new NodeItem{ Name="层级3"},
              new NodeItem{ Name="层级4",Children=new List<NodeItem>{
                 new NodeItem{ Name="层级4-1",Children=new List<NodeItem>{
                 new NodeItem{ Name="层级4-1-1"}
              }}
              } },

        };
    }

    public class DataItem
    {
        public string F1 { get; set; }
        public string F2 { get; set; }
        public string F3 { get; set; }
        public string F4 { get; set; }
    }

    public class NodeItem
    {
        public string Name { get; set; }

        public List<NodeItem> Children { get; set; } = new List<NodeItem>();
    }
}
