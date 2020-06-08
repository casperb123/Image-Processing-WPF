using Microsoft.Win32;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace ImageProcessing
{
    public class FileOperation
    {
        public Bitmap OpenFile()
        {
            Bitmap bitmap = null;
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image Files|*.jpg;*.png;*.bmp";

            if (dialog.ShowDialog() == true)
            {
                Bitmap original = new Bitmap(Image.FromFile(dialog.FileName));
                bitmap = new Bitmap(original.Width, original.Height, PixelFormat.Format32bppArgb);

                using (Graphics graphics = Graphics.FromImage(bitmap))
                    graphics.DrawImage(original, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
            }

            return bitmap;
        }

        public BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }
    }
}
