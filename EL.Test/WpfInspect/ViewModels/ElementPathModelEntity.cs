using Automation.Inspect;
using Automation.Parser;
using EL;
using EL.UIA;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using WpfInspect.Core;

namespace WpfInspect.ViewModels
{
    /// <summary>
    /// 数据库中存储节点字段
    /// </summary>
    public class ElementPathModelEntity
    {
        public ElementPathModelEntity()
        {
        }

        public ElementPathModelEntity(ElementPath element)
        {
            ElementPathStr = JsonHelper.ToJson(element);
            Id = element.Id;
            var lastNode = element.ElementEditNodes.Last(x => x.IsChecked);
            var namePro = lastNode.GetProperty("Name");
            var indexPro = lastNode.GetProperty("Index");
            if (!namePro.IsActive && !indexPro.IsActive)
                Name = EL.UIA.ControlTypeConverter.ConvertTypeToName(element.PathNode.Parent.CurrentElementWin.ControlType) + "_" + element.ElementEditNodes.Last().Name.NormalizeString(12) + "_相似元素";
            else
                Name = EL.UIA.ControlTypeConverter.ConvertTypeToName(element.PathNode.Parent.CurrentElementWin.ControlType) + "_" + element.ElementEditNodes.Last().Name.NormalizeString(12);
        }
        private ElementPath _elmentPath;
        public ElementPath ElementPath
        {
            get
            {
                if (_elmentPath == null && !string.IsNullOrWhiteSpace(ElementPathStr))
                    _elmentPath = JsonHelper.FromJson<ElementPath>(ElementPathStr);
                return _elmentPath;
            }
            set
            {
                _elmentPath = value;
            }
        }

        /// <summary>
        /// 元素Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 元素名称
        /// </summary>
        public string Name { get; set; }

        public string ElementPathStr { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 路径
        /// </summary>
        public string PathStr => ElementPath?.Path;

        private BitmapImage _bitmapImage;

        [JsonIgnore]
        public int Width => ElementPath.BoundingRectangle.Width < 174 ? ElementPath.BoundingRectangle.Width : 174;

        [JsonIgnore]
        public int Height => ElementPath.BoundingRectangle.Height < 97 ? ElementPath.BoundingRectangle.Height : 97;

        /// <summary>
        /// 绑定图片
        /// </summary>
        [JsonIgnore]
        public BitmapImage BitmapImage
        {
            get
            {
                try
                {
                    if (_bitmapImage == null && ElementPath != null)
                    {
                        var imgage = WinPathComponentSystem.Base64ToImage(ElementPath.Img, new Size(Width, Height)) as Bitmap;
                        Bitmap ImageOriginalBase = new Bitmap(imgage);
                        _bitmapImage = new BitmapImage();
                        using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                        {
                            ImageOriginalBase.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            _bitmapImage.BeginInit();
                            _bitmapImage.StreamSource = ms;
                            _bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                            _bitmapImage.EndInit();
                            _bitmapImage.Freeze();
                        }
                    }
                }
                catch (Exception e)
                {

                }
                return _bitmapImage;
            }
            set { _bitmapImage = value; }
        }
    }
}
