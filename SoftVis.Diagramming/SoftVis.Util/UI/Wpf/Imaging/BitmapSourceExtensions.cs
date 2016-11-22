using System.IO;
using System.Windows.Media.Imaging;

namespace Codartis.SoftVis.Util.UI.Wpf.Imaging
{
    public static class BitmapSourceExtensions
    {
        /// <summary>
        /// Converts a BitmapSource to PNG format and save it into the given stream.
        /// </summary>
        /// <param name="bitmapSource">A BitmapSource object.</param>
        /// <param name="stream">A stream that the PNG object will be written to.</param>
        public static void ToPng(this BitmapSource bitmapSource, Stream stream)
        {
            var pngBitmapEncoder = new PngBitmapEncoder();
            pngBitmapEncoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            pngBitmapEncoder.Save(stream);
        }
    }
}
