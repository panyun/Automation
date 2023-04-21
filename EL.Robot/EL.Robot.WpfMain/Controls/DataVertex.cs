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
using EL.Robot.WpfMain.ViewModel;

namespace Robot.Controls
{
    /// <summary>
    /// 节点
    /// </summary>
    public class DataVertex : VertexBase, INotifyPropertyChanged
    {
        public bool IsSelect { get { return GetProperty<bool>();  } set{ SetProperty<bool>(value); } }
        public long NodeId { get; set; }
        private string name;

        private string background;

        public string BackGround
        {
            get { return background; }
            set
            {
                background = value;
                OnPropertyChanged("BackGround");
            }
        }

        /// <summary>
        /// 流程名称
        /// </summary>
        /// 
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        public string imageKey;
        /// <summary>
        /// 流程图片
        /// </summary>
        public string ImageKey
        {
            get { return imageKey; }
            set
            {
                imageKey = value;
                OnPropertyChanged("imageKey");
            }
        }

        /// <summary>
        /// 流程样式
        /// </summary>
        public int Type { get; set; }

        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Default constructor for this class
        /// (required for serialization).
        /// </summary>
        public DataVertex()
        {
        }


        /// <summary>
        /// 是否显示外边框线
        /// </summary>
        public bool ShowOutLine { get; set; }
        //private void setValue<T>(ref T propertyName, T value, string fatherPropertyName)
        //{
        //    propertyName = value;
        //    NotifyPropertyChanged(fatherPropertyName);
        //}
        public void OnPropertyChanged(string PropertyName)
        {
            PropertyChangedEventArgs e = new PropertyChangedEventArgs(PropertyName);
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }   
        }
        /// <summary>
        /// 属性变化通知事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 属性变化通知
        /// </summary>
        /// <param name="e"></param>
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

    }

    public abstract class VertexBase : ObservableObject, IGraphXVertex
    {
        /// <summary>
        /// Gets or sets custom angle associated with the vertex
        /// </summary>
        public double Angle { get; set; }

        /// <summary>
        /// Gets or sets optional group identificator
        /// </summary>
        public int GroupId { get; set; }

        /// <summary>
        /// Skip vertex in algo calc and visualization
        /// </summary>
        public ProcessingOptionEnum SkipProcessing { get; set; }

        protected VertexBase()
        {
            ID = -1;
        }
        /// <summary>
        /// Unique vertex ID
        /// </summary>
        public long ID { get; set; }

        public bool Equals(IGraphXVertex other)
        {
            return Equals(this, other);
        }
    }
}
