using Automation.Inspect;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WpfInspect.Core;

namespace WpfInspect.ViewModels
{
    public class ImagesViewModel : ObservableObject
    {
        public BitmapImage BitmapImage { get { return GetProperty<BitmapImage>(); } set { SetProperty(value); } }

        private string _img;
        /// <summary>
        /// 图片str
        /// </summary>
        public string Img
        {
            get
            {
                return _img;
            }
            set
            {
                _img = value;
                if (string.IsNullOrEmpty(_img))
                    return;
                Width = BoundingRectangle.Width;
                if (Width < 1 || Width > 200)
                    Width = 200;
                Height = BoundingRectangle.Height;
                if (Height < 1 || Height > 100)
                    Height = 100;
                Bitmap imgage = WinPathComponentSystem.Base64ToImage(_img, new Size(Width, Height)) as Bitmap;
                BitmapImage = PicConvert.ToBitmapImage(imgage);
            }
        }

        /// <summary>
        /// ElementPathId
        /// </summary>
        public long ElementPathId { get; set; }

        /// <summary>
        /// 坐标
        /// </summary>
        public Rectangle BoundingRectangle { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }
    }
}
