using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BrandImageGenerator
{
    public static class Helpers
    {
        public static void SaveTo(this UserControl control, string filename, Size size)
        {
            var controlWidth= (int) size.Width;
            var controlHeight= (int) size.Height;
 
            // Set size of the control
            control.Height= controlHeight;
            control.Width= controlWidth;
 
            control.Arrange(new Rect(0, 0, controlWidth, controlHeight));
 
            // Create writeable bitmap to save as image

            var bmp = new RenderTargetBitmap(controlWidth, controlHeight, 120, 120, PixelFormats.Pbgra32);
            bmp.Render(control);
            
            var png = new PngBitmapEncoder();
            png.Frames.Add(BitmapFrame.Create(bmp));
            var stream = new MemoryStream();
            png.Save(stream);

            var fileStream = File.Create(Directory.GetCurrentDirectory()+string.Format(@"\{0}.png", filename));
            stream.Seek(0, SeekOrigin.Begin);
            stream.CopyTo(fileStream);
            fileStream.Close();
        }

       
    }
}
