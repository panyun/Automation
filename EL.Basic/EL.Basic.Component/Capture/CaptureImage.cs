using System.Drawing;
using System.Drawing.Imaging;

namespace EL.Capturing
{
    /// <summary>
    /// Interface for overlays that can be applied to captured images.
    /// </summary>
    public interface ICaptureOverlay
    {
        /// <summary>
        /// Draws the overlay onto the given graphics object.
        /// </summary>
        void Draw(Graphics g);
    }
    /// <summary>
    /// Class with various settings for capturing images.
    /// </summary>
    public class CaptureSettings
    {
        /// <summary>
        /// The width of the output. Set to -1 to scale it in aspect ratio to the <see cref="OutputHeight"/>.
        /// </summary>
        public int OutputWidth { get; set; } = -1;

        /// <summary>
        /// The height of the output. Set to -1 to scale it in aspect ratio to the <see cref="OutputWidth"/>.
        /// </summary>
        public int OutputHeight { get; set; } = -1;

        /// <summary>
        /// The scale of the output (1 == 100%).
        /// </summary>
        public double OutputScale { get; set; } = 1;
    }
    /// <summary>
    /// Object which is returned when the screen or parts of the screen are captured with <see cref="CaptureComponent"/>.
    /// </summary>
    public partial class CaptureImage : IDisposable
    {
        /// <summary>
        /// Creates a <see cref="CaptureImage"/> object with the given <see cref="Bitmap"/>.
        /// </summary>
        public CaptureImage(Bitmap bitmap, Rectangle originalBounds, CaptureSettings settings)
        {
            Bitmap = bitmap;
            OriginalBounds = originalBounds;
            Settings = settings;
            OnInitialized();
        }

        partial void OnInitialized();

        /// <summary>
        /// The original <see cref="Bitmap"/>.
        /// </summary>
        public Bitmap Bitmap { get; }

        /// <summary>
        /// The original bounding rectangle (relative to the whole desktop) that this image is based on.
        /// </summary>
        public Rectangle OriginalBounds { get; }

        /// <summary>
        /// The <see cref="CaptureSettings"/> used to capture the image.
        /// </summary>
        public CaptureSettings Settings { get; }

        /// <summary>
        /// Saves the image to the file with the given path.
        /// Uses the file extension as format, defaults to <see cref="ImageFormat.Png"/>.
        /// </summary>
        public void ToFile(string filePath)
        {
            var imageFormat = ImageFormat.Png;
            var ext = Path.GetExtension(filePath)?.ToLower();
            switch (ext)
            {
                case ".jpg":
                case ".jpeg":
                    imageFormat = ImageFormat.Jpeg;
                    break;
                case ".gif":
                    imageFormat = ImageFormat.Gif;
                    break;
                case ".tif":
                case ".tiff":
                    imageFormat = ImageFormat.Tiff;
                    break;
                case ".bmp":
                    imageFormat = ImageFormat.Bmp;
                    break;
            }
            Log.Debug($"Saving image to file: {filePath}");
            Bitmap.Save(filePath, imageFormat);
        }

        /// <summary>
        /// Applies all the given overlays onto the image.
        /// </summary>
        public CaptureImage ApplyOverlays(params ICaptureOverlay[] overlays)
        {
            if (overlays.Any())
            {
                using (var g = Graphics.FromImage(Bitmap))
                {
                    foreach (var overlay in overlays)
                    {
                        try
                        {
                            overlay.Draw(g);
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"Failed applying overlay '{overlay.GetType().FullName}'", ex);
                        }
                    }
                }
            }
            return this;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Bitmap?.Dispose();
        }
    }
}
