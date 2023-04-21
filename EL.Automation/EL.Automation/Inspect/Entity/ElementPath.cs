using Automation.Parser;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System.Drawing;

namespace Automation.Inspect
{
    public class ElementPath
    {
        public long Id { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        /// <summary>
        /// 程序名称
        /// </summary>
        public string ProcessName { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        /// <summary>
        /// 当前窗口对应title
        /// </summary>
        public string NativeWindowTitle { get; set; }
        /// <summary>
        /// 进程对应的mainwindowTtile
        /// </summary>
        public string MainWindowTitle { get; set; }
        [JsonIgnore]
        private Rectangle _boundingRectangle;
        /// <summary>
        /// 节点矩形
        /// </summary>
        public Rectangle BoundingRectangle
        {
            get
            {
                return _boundingRectangle;
            }
            set
            {
                _boundingRectangle = value;
                Width = _boundingRectangle.Width;
                Height = _boundingRectangle.Height;
                X = _boundingRectangle.X;
                Y = _boundingRectangle.Y;
            }
        }
        /// <summary>
        /// width
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// height
        /// </summary>
        public int Height { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        /// <summary>
        /// 节点图像
        /// </summary>
        public string Img { get; set; }
        /// <summary>
        /// 节点名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 节点类型
        /// </summary>
        public string ControlType { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        /// <summary>
        /// 节点值
        /// </summary>
        public string Value { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Text { get; set; }
        /// <summary>
        /// 节点路径
        /// </summary>
        public string Path { get; set; }
        public int Index { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ElementNode PathNode { get; set; }

        public ElementType ElementType
        {
            get; set;
        }
        /// <summary>
        /// 路径编辑信息
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<ElementEdit> ElementEditNodes { get; set; } = new List<ElementEdit>();
        [JsonIgnore]
        [BsonIgnore]
        public AvigationType AvigationType
        {
            get
            {
                if (PathNode.PathRuntime != null)
                {
                    return AvigationType.Runtime;
                }
                if (ElementEditNodes != default && ElementEditNodes.Count > 0)
                {
                    if (ElementEditNodes.Exists(x => x.IsSimilarity))
                    {
                        return AvigationType.Similarity;
                    }
                    else if (ElementEditNodes.Exists(x => x.IsEdit))
                    {
                        return AvigationType.Edit;
                    }
                }
                if (CosineValue != -1)
                {
                    return AvigationType.ConsineSimilarity;
                }
                return AvigationType.None;
            }
        }
        /// <summary>
        /// 余弦相似度
        /// </summary>
        public double CosineValue { get; set; } = -1;
        public double SetCosineValueFalse()
        {
            return -1;
        }
    }
    public class ElementNodeRuntime
    {
        public ElementNodeRuntime() { }
        public IntPtr Handle { get; set; }
        public int[] RuntimeId { get; set; }
        public int ControlType { get; set; }
        public string Role { get; set; }
    }
    public enum AvigationType
    {
        None = 0,
        Edit,
        Similarity,
        ConsineSimilarity,
        Runtime
    }

}
