﻿using ImageProcessing.Entities;
using ImageProcessing.Entities.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static ImageProcessing.Entities.ImageEffect;
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

        private void ApplyColorMatrix(Bitmap bitmap, float[][] matrixArray)
        {
            ImageAttributes attributes = new ImageAttributes();
            attributes.SetColorMatrix(new ColorMatrix(matrixArray), ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            Graphics graphics = Graphics.FromImage(bitmap);
            Rectangle rectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            graphics.DrawImage(bitmap, rectangle, 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, attributes);
        }

        private void SetGamma(Bitmap bitmap, float gamma)
        {
            ImageAttributes attributes = new ImageAttributes();
            attributes.SetGamma(gamma);

            Graphics graphics = Graphics.FromImage(bitmap);
            Rectangle rectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            graphics.DrawImage(bitmap, rectangle, 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, attributes);
        }

        private Bitmap Pixelate(Bitmap bitmap, int pixelateSize)
        {
            Bitmap modifiedBitmap = new Bitmap(bitmap);
            Rectangle rectangle = new Rectangle(0, 0, modifiedBitmap.Width, modifiedBitmap.Height);

            for (int x = rectangle.X; x < rectangle.X + rectangle.Width && x < modifiedBitmap.Width; x += pixelateSize)
                for (int y = rectangle.Y; y < rectangle.Y + rectangle.Height && y < modifiedBitmap.Height; y += pixelateSize)
                {
                    int offsetX = pixelateSize / 2;
                    int offsetY = pixelateSize / 2;

                    while (x + offsetX >= modifiedBitmap.Width) offsetX--;
                    while (y + offsetY >= modifiedBitmap.Height) offsetY--;

                    Color pixel = modifiedBitmap.GetPixel(x + offsetX, y + offsetY);

                    for (int xx = x; xx < x + pixelateSize && xx < modifiedBitmap.Width; xx++)
                        for (int yy = y; yy < y + pixelateSize && yy < modifiedBitmap.Height; yy++)
                            modifiedBitmap.SetPixel(xx, yy, pixel);
                }

            return modifiedBitmap;
        }

        public Bitmap MedianFilter(Bitmap bitmap, int matrixSize)
        {
            Rectangle rectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData bitmapData = bitmap.LockBits(rectangle, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            byte[] pixelBuffer = new byte[bitmapData.Stride * bitmapData.Height];
            byte[] resultBuffer = new byte[bitmapData.Stride * bitmapData.Height];

            Marshal.Copy(bitmapData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            bitmap.UnlockBits(bitmapData);

            int filterOffset = (matrixSize - 1) / 2;

            //List<int> neighbourPixels = new List<int>();

            int imageWidth = bitmap.Width;
            int imageHeight = bitmap.Height;

            Parallel.For(filterOffset, imageHeight - filterOffset, offsetY =>
            {
                Parallel.For(filterOffset, imageWidth - filterOffset, offsetX =>
                {
                    List<int> neighbourPixels = new List<int>();
                    int byteOffset = offsetY * bitmapData.Stride + offsetX * 4;

                    for (int filterY = -filterOffset; filterY <= filterOffset; filterY++)
                    {
                        for (int filterX = -filterOffset; filterX <= filterOffset; filterX++)
                        {
                            int calcOffset = byteOffset + (filterX * 4) + (filterY * bitmapData.Stride);
                            neighbourPixels.Add(BitConverter.ToInt32(pixelBuffer, calcOffset));
                        }
                    }

                    neighbourPixels.Sort();
                    byte[] middlePixel = BitConverter.GetBytes(neighbourPixels[filterOffset]);

                    resultBuffer[byteOffset] = middlePixel[0];
                    resultBuffer[byteOffset + 1] = middlePixel[1];
                    resultBuffer[byteOffset + 2] = middlePixel[2];
                    resultBuffer[byteOffset + 3] = middlePixel[3];
                });
            });

            Bitmap resultBitmap = new Bitmap(bitmap.Width, bitmap.Height);
            Rectangle resultRectangle = new Rectangle(0, 0, resultBitmap.Width, resultBitmap.Height);
            BitmapData resultData = resultBitmap.LockBits(resultRectangle, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);
            resultBitmap.UnlockBits(resultData);

            return resultBitmap;
        }

        private void InvertImage(Bitmap bitmap)
        {
            float[][] invertArray =
            {
                new float[] {-1, 0, 0, 0, 0},
                new float[] {0, -1, 0, 0, 0},
                new float[] {0, 0, -1, 0, 0},
                new float[] {0, 0, 0, 1, 0},
                new float[] {1, 1, 1, 0, 1}
            };

            ApplyColorMatrix(bitmap, invertArray);
        }

        private void SepiaTone(Bitmap bitmap)
        {
            float[][] sepiaArray =
            {
                new float[] {.393f, .349f, .272f, 0, 0},
                new float[] {.769f, .686f, .534f, 0, 0},
                new float[] {.189f, .168f, .131f, 0, 0},
                new float[] {0, 0, 0, 1, 0},
                new float[] {0, 0, 0, 0, 1}
            };

            ApplyColorMatrix(bitmap, sepiaArray);
        }

        private Bitmap Emboss(Bitmap bitmap, FilterType filterType)
        {
            ConvolutionFilterBase filter = null;

            if (filterType == FilterType.Emboss)
                filter = new EmbossFilter();
            else if (filterType == FilterType.Emboss45Degree)
                filter = new Emboss45DegreeFilter();
            else if (filterType == FilterType.EmbossTopLeft)
                filter = new EmbossTopLeftFilter();
            else if (filterType == FilterType.EmbossIntense)
                filter = new EmbossIntenseFilter();

            if (filter is null)
                return bitmap;

            return bitmap.ConvolutionFilter(filter);
        }

        private Bitmap BoxBlur(Bitmap bitmap, int amount)
        {
            BoxBlurFilter boxBlur = new BoxBlurFilter(amount);
            return bitmap.ConvolutionFilter(boxBlur);
        }

        private Bitmap GaussianBlur(Bitmap bitmap, int amount)
        {
            GaussianBlurFilter gaussianBlur = new GaussianBlurFilter(bitmap);
            return gaussianBlur.Process(amount);
        }

        private Bitmap EdgeDetection(Bitmap bitmap, FilterType filterType)
        {
            ConvolutionFilterBase filter = null;

            if (filterType == FilterType.EdgeDetection)
                filter = new EdgeDetectionFilter();
            else if (filterType == FilterType.EdgeDetection45Degree)
                filter = new EdgeDetection45DegreeFilter();
            else if (filterType == FilterType.EdgeDetectionHorizontal)
                filter = new EdgeDetectionHorizontalFilter();
            else if (filterType == FilterType.EdgeDetectionTopLeft)
                filter = new EdgeDetectionTopLeftFilter();
            else if (filterType == FilterType.EdgeDetectionVertical)
                filter = new EdgeDetectionVerticalFilter();

            if (filter is null)
                return bitmap;

            return bitmap.ConvolutionFilter(filter);
        }

        public void Modify(Bitmap bitmap,
                           double hueMin,
                           double hueMax,
                           float brightness,
                           float contrast,
                           float gamma,
                           bool grayScale,
                           System.Windows.Media.Color pixelColor,
                           bool replaceGrayColor,
                           bool imageEffects,
                           List<ImageEffect> enabledFilters,
                           List<ImageEffect> filters)
        {
            Bitmap modifiedBitmap = new Bitmap(bitmap);

            // Use "unsafe" because C# doesn't support pointer aritmetic by default
            unsafe
            {
                // Lock the bitmap into system memory
                // "PixelFormat" can be "Format24bppRgb", "Format32bppArgb", etc
                BitmapData bitmapData = modifiedBitmap.LockBits(new Rectangle(0, 0, modifiedBitmap.Width, modifiedBitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

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
                            Color newColor;

                            if (replaceGrayColor)
                                newColor = Color.FromArgb(pixelColor.A, pixelColor.R, pixelColor.G, pixelColor.B);
                            else
                            {
                                int avg = (oldRed + oldGreen + oldBlue) / 3;
                                newColor = Color.FromArgb(avg, avg, avg);
                            }

                            // Change each pixels color
                            currentLine[x] = newColor.B;
                            currentLine[x + 1] = newColor.G;
                            currentLine[x + 2] = newColor.R;
                            currentLine[x + 3] = newColor.A;
                        }
                    }
                });

                modifiedBitmap.UnlockBits(bitmapData);
            }

            if (imageEffects)
            {
                foreach (ImageEffect effect in enabledFilters)
                {
                    if (effect.Filter.ToString().Contains("EdgeDetection"))
                        modifiedBitmap = EdgeDetection(modifiedBitmap, effect.Filter);
                    else if (effect.Filter.ToString().Contains("Emboss"))
                        modifiedBitmap = Emboss(modifiedBitmap, effect.Filter);

                    switch (effect.Filter)
                    {
                        case FilterType.Invert:
                            InvertImage(modifiedBitmap);
                            break;
                        case FilterType.SepiaTone:
                            SepiaTone(modifiedBitmap);
                            break;
                        case FilterType.Pixelate:
                            modifiedBitmap = Pixelate(modifiedBitmap, effect.CurrentValue);
                            break;
                        case FilterType.Median:
                            modifiedBitmap = MedianFilter(modifiedBitmap, effect.CurrentValue);
                            break;
                        case FilterType.BoxBlur:
                            modifiedBitmap = BoxBlur(modifiedBitmap, effect.CurrentValue);
                            break;
                        case FilterType.GaussianBlur:
                            modifiedBitmap = GaussianBlur(modifiedBitmap, effect.CurrentValue);
                            break;
                        default:
                            break;
                    }
                }
            }

            float[][] brightnessContrastArray =
            {
                new float[] {contrast, 0, 0, 0, 0}, // scale red
                new float[] {0, contrast, 0, 0, 0}, // scale green
                new float[] {0, 0, contrast, 0, 0}, // scale blue
                new float[] {0, 0, 0, 1, 0}, // don't scale alpha
                new float[] {brightness, brightness, brightness, 0, 1}
            };

            ApplyColorMatrix(modifiedBitmap, brightnessContrastArray);
            SetGamma(modifiedBitmap, gamma);

            OnImageFinished(modifiedBitmap);
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
