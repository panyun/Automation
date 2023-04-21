using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WpfInspect.Core
{
    public static class PicConvert
    {

        public static BitmapImage ToBitmapImage(System.Drawing.Bitmap ImageOriginal)
        {
            if (ImageOriginal == null)
                return null;

            System.Drawing.Bitmap ImageOriginalBase = new System.Drawing.Bitmap(ImageOriginal);
            BitmapImage bitmapImage = new BitmapImage();
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                ImageOriginalBase.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
            }
            return bitmapImage;
        }
    }
}
