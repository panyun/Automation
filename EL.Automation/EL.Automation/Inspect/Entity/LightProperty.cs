using Automation.Parser;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System.Drawing;

namespace Automation.Inspect
{
    public class LightProperty
    {
        [JsonIgnore]
        [BsonIgnore]
        public CancellationTokenSource CancellationTokenSource { get; set; }
        /// <summary>
        /// 是否高亮显示
        /// </summary>
        public bool IsLight { get; set; } = true;
        public int Count { get; set; } = 1;
        public int Time { get; set; } = 100;
        public string ColorName { get; set; } = nameof(Color.Red);
        [JsonIgnore]
        [BsonIgnore]
        public Color Color
        {
            get
            {
                return Color.FromName(ColorName ?? "Red");
            }
        }
    }
    public enum ActionType
    {
        ElementEvent,
        Mouse,
    }
    public enum LocationType
    {
        Center,
        LeftUp,
        LeftDown,
        RightUp,
        RightDown,
    }
    public enum ClickType
    {
        LeftClick,
        CenterClick,
        RightClick,
        LeftDoubleClick
    }


}
