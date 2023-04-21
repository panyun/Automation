using GraphX;
using GraphX.Controls;
using GraphX.Measure;
using GraphX.Common.Enums;
using GraphX.Common.Interfaces;
using GraphX.Common.Models;
using QuickGraph;
using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using YAXLib;

namespace Robot.Controls
{

    /// <summary>
    /// 连线
    /// </summary>
    [Serializable]
    public class DataEdge : EdgeBase<DataVertex>, INotifyPropertyChanged
    {
        [YAXCustomSerializer(typeof(YAXPointArraySerializer))]
        public override Point[] RoutingPoints { get; set; }

        public DataEdge(DataVertex source, DataVertex target, double weight = 1)
            : base(source, target, weight)
        {
            Angle = 90;
        }

        public DataEdge()
            : base(null, null, 1)
        {
            Angle = 90;
        }

        public bool ArrowTarget { get; set; }

        public double Angle { get; set; }

        /// <summary>
        /// Node main description (header)
        /// </summary>
        private string _text;
        public string Text { get { return _text; } set { _text = value; OnPropertyChanged("Text"); } }
        public string ToolTipText { get; set; }

        public override string ToString()
        {
            return Text;
        }

        public EdgeDashStyle LineType { get; set; }

        /// <summary>
        /// 线条颜色
        /// </summary>
        public string LineColor { get; set; } = "#99999B";

        /// <summary>
        /// 是否显示+按钮
        /// </summary>
        public bool ShowAddButton { get; set; }

        /// <summary>
        /// 是否显示箭头
        /// </summary>
        public bool ShowArrow { get; set; } = true;

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public sealed class YAXPointArraySerializer : ICustomSerializer<Point[]>
    {

        private Point[] Deserialize(string str)
        {
            if (string.IsNullOrEmpty(str)) return null;
            var arr = str.Split(new char[] { '~' });
            var ptlist = new Point[arr.Length];
            int cnt = 0;
            foreach (var item in arr)
            {
                var res = item.Split(new char[] { '|' });
                if (res.Length == 2) ptlist[cnt] = new Point(Convert.ToDouble(res[0]), Convert.ToDouble(res[1]));
                else ptlist[cnt] = new Point();
                cnt++;
            }
            return ptlist;
        }

        private string Serialize(Point[] list)
        {
            var sb = new StringBuilder();
            if (list != null)
            {
                var last = list.Last();
                foreach (var item in list)
                    sb.Append(string.Format("{0}|{1}{2}", item.X.ToString(), item.Y.ToString(), (item != last ? "~" : "")));
            }
            return sb.ToString();
        }

        public Point[] DeserializeFromAttribute(System.Xml.Linq.XAttribute attrib)
        {
            return Deserialize(attrib.Value);
        }

        public Point[] DeserializeFromElement(System.Xml.Linq.XElement element)
        {
            return Deserialize(element.Value);
        }

        public Point[] DeserializeFromValue(string value)
        {
            return Deserialize(value);
        }

        public void SerializeToAttribute(Point[] objectToSerialize, System.Xml.Linq.XAttribute attrToFill)
        {
            attrToFill.Value = Serialize(objectToSerialize);
        }

        public void SerializeToElement(Point[] objectToSerialize, System.Xml.Linq.XElement elemToFill)
        {
            elemToFill.Value = Serialize(objectToSerialize);
        }

        public string SerializeToValue(Point[] objectToSerialize)
        {
            return Serialize(objectToSerialize);
        }
    }
}
