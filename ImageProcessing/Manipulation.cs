using ControlzEx.Standard;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Color = System.Drawing.Color;

namespace ImageProcessing
{
    public class Manipulation
    {
        public event EventHandler<ImageEventArgs> ImageFinished;

        public class ImageEventArgs : EventArgs
        {
            public Bitmap Bitmap { get; set; }

            public ImageEventArgs(Bitmap bitmap)
            {
                Bitmap = bitmap;
            }
        }

        protected virtual void OnImageFinished(Bitmap bitmap)
        {
            ImageFinished?.Invoke(this, new ImageEventArgs(bitmap));
        }

        //public void ManipulateSetPixel(object obj, bool removeRed, bool removeGreen, bool removeBlue)
        //{
        //    if (obj is Bitmap bitmap)
        //    {
        //        Bitmap modifiedBitmap = new Bitmap(bitmap);

        //        for (int w = 0; w < modifiedBitmap.Width; w++)
        //        {
        //            for (int h = 0; h < modifiedBitmap.Height; h++)
        //            {
        //                Color color = modifiedBitmap.GetPixel(w, h);
        //                Color newColor;

        //                if (removeRed && removeGreen && removeBlue)
        //                {
        //                    int gray = (color.R + color.G + color.B) / 3;
        //                    newColor = Color.FromArgb(gray, gray, gray);
        //                }
        //                else
        //                {
        //                    int r = color.R;
        //                    int g = color.G;
        //                    int b = color.B;

        //                    if (removeRed)
        //                        r = 0;
        //                    if (removeGreen)
        //                        g = 0;
        //                    if (removeBlue)
        //                        b = 0;

        //                    newColor = Color.FromArgb(r, g, b);
        //                }

        //                modifiedBitmap.SetPixel(w, h, newColor);
        //            }
        //        }

        //        OnImageFinished(modifiedBitmap);
        //    }
        //}

        public void Modify(object obj, double hueMin, double hueMax)
        {
            if (obj is Bitmap bitmap)
            {
                Bitmap modifiedBitmap = new Bitmap(bitmap);

                unsafe
                {
                    BitmapData bitmapData = modifiedBitmap.LockBits(new Rectangle(0, 0, modifiedBitmap.Width, modifiedBitmap.Height), ImageLockMode.ReadWrite, modifiedBitmap.PixelFormat);
                    int bytesPerPixel = Image.GetPixelFormatSize(modifiedBitmap.PixelFormat) / 8;
                    int heightInPixels = modifiedBitmap.Height;
                    int widthInBytes = modifiedBitmap.Width * bytesPerPixel;
                    byte* ptrFirstPixel = (byte*)bitmapData.Scan0;

                    Parallel.For(0, heightInPixels, y =>
                    {
                        byte* currentLine = ptrFirstPixel + (y * bitmapData.Stride);

                        for (int x = 0; x < widthInBytes; x += bytesPerPixel)
                        {
                            int oldBlue = currentLine[x];
                            int oldGreen = currentLine[x + 1];
                            int oldRed = currentLine[x + 2];
                            Color color = Color.FromArgb(oldRed, oldGreen, oldBlue);
                            double hue = color.GetHue();

                            if (hue < hueMin || hue > hueMax)
                            {
                                int avg = (oldRed + oldGreen + oldBlue) / 3;
                                Color newColor = Color.FromArgb(avg, avg, avg);

                                currentLine[x] = newColor.B;
                                currentLine[x + 1] = newColor.G;
                                currentLine[x + 2] = newColor.R;
                            }
                        }
                    });

                    modifiedBitmap.UnlockBits(bitmapData);
                }

                OnImageFinished(modifiedBitmap);
            }
        }

        //public void ManipulateLockBits(object obj, bool removeRed, bool removeGreen, bool removeBlue)
        //{
        //    if (obj is Bitmap bitmap)
        //    {
        //        Bitmap modifiedBitmap = new Bitmap(bitmap);

        //        // Use "unsafe" because C# doesn't support pointer aritmetic by default
        //        unsafe
        //        {
        //            // Lock the bitmap into system memory
        //            // "PixelFormat" can be "Format24bppRgb", "Format32bppArgb", etc
        //            BitmapData bitmapData = modifiedBitmap.LockBits(new Rectangle(0, 0, modifiedBitmap.Width, modifiedBitmap.Height), ImageLockMode.ReadWrite, modifiedBitmap.PixelFormat);

        //            // Define variables for bytes per pixel, as well as Image Width & Height
        //            int bytesPerPixel = Image.GetPixelFormatSize(modifiedBitmap.PixelFormat) / 8;
        //            int heightInPixels = bitmapData.Height;
        //            int widthInBytes = bitmapData.Width * bytesPerPixel;

        //            // Define a pointer to the first pixel in the locked image
        //            // Scan0 gets or sets the address of the first pixel data in the bitmap
        //            // This can also be thought of as the first scan line in the bitmap
        //            byte* PtrFirstPixel = (byte*)bitmapData.Scan0;

        //            // Step thru each pixel in the image using pointers
        //            // Parallel.For execute a 'for' loop in which iterations may run in parallel
        //            Parallel.For(0, heightInPixels, y =>
        //            {
        //                // Use the 'Stride' (scanline width) property to step line by line through the image
        //                byte* currentLine = PtrFirstPixel + (y * bitmapData.Stride);

        //                for (int x = 0; x < widthInBytes; x += bytesPerPixel)
        //                {
        //                    // GET: each pixel color (R, G & B)
        //                    int oldBlue = currentLine[x];
        //                    int oldGreen = currentLine[x + 1];
        //                    int oldRed = currentLine[x + 2];

        //                    if (removeRed && removeGreen && removeBlue)
        //                    {
        //                        int gray = (oldRed + oldGreen + oldBlue) / 3;

        //                        currentLine[x] = (byte)gray;
        //                        currentLine[x + 1] = (byte)gray;
        //                        currentLine[x + 2] = (byte)gray;
        //                    }
        //                    else
        //                    {
        //                        if (removeRed)
        //                            oldRed = 0;
        //                        if (removeGreen)
        //                            oldGreen = 0;
        //                        if (removeBlue)
        //                            oldBlue = 0;

        //                        currentLine[x] = (byte)oldBlue;
        //                        currentLine[x + 1] = (byte)oldGreen;
        //                        currentLine[x + 2] = (byte)oldRed;
        //                    }
        //                }
        //            });

        //            modifiedBitmap.UnlockBits(bitmapData);
        //        }

        //        OnImageFinished(modifiedBitmap);
        //    }
        //}

        public Bitmap[,] MakeTiles(object obj)
        {
            if (obj is Bitmap bitmap)
            {
                Size tileSize = new Size(bitmap.Width / 4, bitmap.Height / 2);
                Bitmap[,] bitmapArray = new Bitmap[4, 2];

                #region Break the original bitmap image into an array of tiles
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        // Define "moving" rectangles with dimensions of the tiles to define where in the source image to pull from
                        Rectangle movingTileFrame = new Rectangle(i * tileSize.Width, j * tileSize.Height, tileSize.Width, tileSize.Height);

                        // Define each element of 4,2 array of bitmaps at the corrent tile size
                        bitmapArray[i, j] = new Bitmap(tileSize.Width, tileSize.Height);

                        // Define a graphics "canvas" object to draw on, based on each tile in the bitmap array
                        using (Graphics canvas = Graphics.FromImage(bitmapArray[i, j]))
                        {
                            // Draw a portion of the original bitmap on the canvas, defined by the moving rectangles
                            canvas.DrawImage(bitmap,
                                             new Rectangle(0, 0, tileSize.Width, tileSize.Height),
                                             movingTileFrame,
                                             GraphicsUnit.Pixel);
                        }
                    }
                }

                return bitmapArray;
                #endregion
            }

            return null;
        }

        //public Bitmap[,] ParallelImageProcess(Bitmap[,] bitmaps)
        //{
        //    // NOTE: INCLUSIVE & EXCLUSIVE INDICES
        //    Parallel.For(0, 4, x =>
        //    {
        //        for (int y = 0; y < 2; y++)
        //        {
        //            int width = bitmaps[x, y].Width;
        //            int height = bitmaps[x, y].Height;

        //            for (int i = 0; i < width; i++)
        //            {
        //                for (int j = 0; j < height; j++)
        //                {
        //                    Color oldPixel = bitmaps[x, y].GetPixel(i, j);
        //                    Color newPixel = Color.FromArgb(oldPixel.R, oldPixel.G, 0);
        //                    bitmaps[x, y].SetPixel(i, j, newPixel);
        //                }
        //            }
        //        }
        //    });

        //    return bitmaps;
        //}

        public Color Hue(double value)
        {
            HSLColor hslColor = new HSLColor(value, 360, 180);
            return hslColor;
        }
    }
}
