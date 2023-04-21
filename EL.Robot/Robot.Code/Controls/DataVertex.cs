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
    /// 节点
    /// </summary>
    public class DataVertex : VertexBase
    {

        /// <summary>
        /// 流程名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 流程图片
        /// </summary>
        public string ImageKey { get; set; }

        /// <summary>
        /// 流程样式
        /// </summary>
        public int Type { get; set; }

        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// 是否选中状态
        /// </summary>
        public bool IsSelected { get; set; }

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

   
    }

    public abstract class VertexBase : IGraphXVertex
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
