using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows;
using Color = System.Drawing.Color;
using Size = System.Drawing.Size;

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

        public void ApplyColorMatrix(Bitmap bitmap, float[][] matrixArray, bool clearExistingMatrix = false)
        {
            ImageAttributes attributes = new ImageAttributes();
            if (clearExistingMatrix)
                attributes.ClearColorMatrix();

            attributes.SetColorMatrix(new ColorMatrix(matrixArray), ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            Graphics graphics = Graphics.FromImage(bitmap);
            Rectangle rectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            graphics.DrawImage(bitmap, rectangle, 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, attributes);
        }

        public void Modify(object obj, double hueMin, double hueMax, float brightness, float contrast, float gamma, bool grayScale, bool invert, bool sepiaTone)
        {
            if (obj is Bitmap bitmap)
            {
                Bitmap modifiedBitmap = new Bitmap(bitmap);

                // Use "unsafe" because C# doesn't support pointer aritmetic by default
                unsafe
                {
                    // Lock the bitmap into system memory
                    // "PixelFormat" can be "Format24bppRgb", "Format32bppArgb", etc
                    BitmapData bitmapData = modifiedBitmap.LockBits(new Rectangle(0, 0, modifiedBitmap.Width, modifiedBitmap.Height), ImageLockMode.ReadWrite, modifiedBitmap.PixelFormat);

                    // Define variables for bytes per pixel, as well as Image Width & Height
                    int bytesPerPixel = Image.GetPixelFormatSize(modifiedBitmap.PixelFormat) / 8;
                    int heightInPixels = modifiedBitmap.Height;
                    int widthInBytes = modifiedBitmap.Width * bytesPerPixel;

                    // Define a pointer to the first pixel in the locked image
                    // Scan0 gets or sets the address of the first pixel data in the bitmap
                    // This can also be thought of as the first scan line in the bitmap
                    byte* ptrFirstPixel = (byte*)bitmapData.Scan0;

                    // Step thru each pixel in the image using pointers
                    // Parallel.For execute a 'for' loop in which iterations may run in parallel
                    Parallel.For(0, heightInPixels, y =>
                    {
                        // Use the 'Stride' (scanline width) property to step line by line through the image
                        byte* currentLine = ptrFirstPixel + (y * bitmapData.Stride);

                        for (int x = 0; x < widthInBytes; x += bytesPerPixel)
                        {
                            // Get each pixel color (R, G & B)
                            int oldBlue = currentLine[x];
                            int oldGreen = currentLine[x + 1];
                            int oldRed = currentLine[x + 2];

                            Color color = Color.FromArgb(oldRed, oldGreen, oldBlue);
                            double hue = color.GetHue();

                            if (grayScale || hue < hueMin || hue > hueMax)
                            {
                                int avg = (oldRed + oldGreen + oldBlue) / 3;
                                Color newColor = Color.FromArgb(avg, avg, avg);

                                // Change each pixels color
                                currentLine[x] = newColor.B;
                                currentLine[x + 1] = newColor.G;
                                currentLine[x + 2] = newColor.R;
                            }
                        }
                    });

                    modifiedBitmap.UnlockBits(bitmapData);
                }

                float[][] matrixArray =
                {
                    new float[] {contrast, 0, 0, 0, 0}, // scale red
                    new float[] {0, contrast, 0, 0, 0}, // scale green
                    new float[] {0, 0, contrast, 0, 0}, // scale blue
                    new float[] {0, 0, 0, 1.0f, 0}, // don't scale alpha
                    new float[] {brightness, brightness, brightness, 0, 1}
                };

                ApplyColorMatrix(modifiedBitmap, matrixArray, true);

                if (invert)
                {
                    float[][] invertArray =
                    {
                        new float[] {-1, 0, 0, 0, 0},
                        new float[] {0, -1, 0, 0, 0},
                        new float[] {0, 0, -1, 0, 0},
                        new float[] {0, 0, 0, 1, 0},
                        new float[] {1, 1, 1, 0, 1}
                    };

                    ApplyColorMatrix(modifiedBitmap, invertArray);
                }

                if (sepiaTone)
                {
                    float[][] sepiaArray =
                    {
                        new float[] {.393f, .349f, .272f, 0, 0},
                        new float[] {.769f, .686f, .534f, 0, 0},
                        new float[] {.189f, .168f, .131f, 0, 0},
                        new float[] {0, 0, 0, 1, 0},
                        new float[] {0, 0, 0, 0, 1}
                    };

                    ApplyColorMatrix(modifiedBitmap, sepiaArray);
                }

                OnImageFinished(modifiedBitmap);
            }
        }

        //public Bitmap[,] MakeTiles(object obj)
        //{
        //    if (obj is Bitmap bitmap)
        //    {
        //        Size tileSize = new Size(bitmap.Width / 4, bitmap.Height / 2);
        //        Bitmap[,] bitmapArray = new Bitmap[4, 2];

        //        #region Break the original bitmap image into an array of tiles
        //        for (int i = 0; i < 4; i++)
        //        {
        //            for (int j = 0; j < 2; j++)
        //            {
        //                // Define "moving" rectangles with dimensions of the tiles to define where in the source image to pull from
        //                Rectangle movingTileFrame = new Rectangle(i * tileSize.Width, j * tileSize.Height, tileSize.Width, tileSize.Height);

        //                // Define each element of 4,2 array of bitmaps at the corrent tile size
        //                bitmapArray[i, j] = new Bitmap(tileSize.Width, tileSize.Height);

        //                // Define a graphics "canvas" object to draw on, based on each tile in the bitmap array
        //                using (Graphics canvas = Graphics.FromImage(bitmapArray[i, j]))
        //                {
        //                    // Draw a portion of the original bitmap on the canvas, defined by the moving rectangles
        //                    canvas.DrawImage(bitmap,
        //                                     new Rectangle(0, 0, tileSize.Width, tileSize.Height),
        //                                     movingTileFrame,
        //                                     GraphicsUnit.Pixel);
        //                }
        //            }
        //        }

        //        return bitmapArray;
        //        #endregion
        //    }

        //    return null;
        //}

        public Color Hue(double value)
        {
            HSLColor hslColor = new HSLColor(value, 360, 180);
            return hslColor;
        }
    }
}
