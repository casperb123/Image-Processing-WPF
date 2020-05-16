﻿using ImageProcessing.Entities;
using ImageProcessing.Entities.Filters;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
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
            {
                for (int y = rectangle.Y; y < rectangle.Y + rectangle.Height && y < modifiedBitmap.Height; y += pixelateSize)
                {
                    int offsetX = pixelateSize / 2;
                    int offsetY = pixelateSize / 2;

                    while (x + offsetX >= modifiedBitmap.Width) offsetX--;
                    while (y + offsetY >= modifiedBitmap.Height) offsetY--;

                    Color pixel = modifiedBitmap.GetPixel(x + offsetX, y + offsetY);

                    for (int xx = x; xx < x + pixelateSize && xx < modifiedBitmap.Width; xx++)
                    {
                        for (int yy = y; yy < y + pixelateSize && yy < modifiedBitmap.Height; yy++)
                        {
                            modifiedBitmap.SetPixel(xx, yy, pixel);
                        }
                    }
                }
            }

            return modifiedBitmap;
        }

        private Bitmap MedianFilter(Bitmap bitmap, int matrixSize)
        {
            Rectangle rectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData bitmapData = bitmap.LockBits(rectangle, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            byte[] pixelBuffer = new byte[bitmapData.Stride * bitmapData.Height];
            byte[] resultBuffer = new byte[bitmapData.Stride * bitmapData.Height];

            Marshal.Copy(bitmapData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            bitmap.UnlockBits(bitmapData);

            int filterOffset = (matrixSize - 1) / 2;

            List<int> neighbourPixels = new List<int>();
            byte[] middlePixel;

            for (int offsetY = filterOffset; offsetY < bitmap.Height - filterOffset; offsetY++)
            {
                for (int offsetX = filterOffset; offsetX < bitmap.Width - filterOffset; offsetX++)
                {
                    int byteOffset = offsetY * bitmapData.Stride + offsetX * 4;
                    neighbourPixels.Clear();

                    for (int filterY = -filterOffset; filterY <= filterOffset; filterY++)
                    {
                        for (int filterX = -filterOffset; filterX <= filterOffset; filterX++)
                        {
                            int calcOffset = byteOffset + (filterX * 4) + (filterY * bitmapData.Stride);
                            neighbourPixels.Add(BitConverter.ToInt32(pixelBuffer, calcOffset));
                        }
                    }

                    neighbourPixels.Sort();
                    middlePixel = BitConverter.GetBytes(neighbourPixels[filterOffset]);

                    resultBuffer[byteOffset] = middlePixel[0];
                    resultBuffer[byteOffset + 1] = middlePixel[1];
                    resultBuffer[byteOffset + 2] = middlePixel[2];
                    resultBuffer[byteOffset + 3] = middlePixel[3];
                }
            }

            Bitmap resultBitmap = new Bitmap(bitmap.Width, bitmap.Height);
            Rectangle resultRectangle = new Rectangle(0, 0, resultBitmap.Width, resultBitmap.Height);
            BitmapData resultData = resultBitmap.LockBits(resultRectangle, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);
            resultBitmap.UnlockBits(resultData);

            return resultBitmap;
        }

        public void Modify(object obj, double hueMin, double hueMax, float brightness, float contrast, float gamma, bool grayScale, bool invert, bool sepiaTone, bool pixelate, int pixelateSize, ConvolutionFilterBase filter, bool medianFilter, int medianSize)
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

                modifiedBitmap = modifiedBitmap.ConvolutionFilter(filter);

                if (pixelate)
                    modifiedBitmap = Pixelate(modifiedBitmap, pixelateSize);

                if (medianFilter)
                    modifiedBitmap = MedianFilter(modifiedBitmap, medianSize);

                float[][] brightnessContrastArray =
                {
                    new float[] {contrast, 0, 0, 0, 0}, // scale red
                    new float[] {0, contrast, 0, 0, 0}, // scale green
                    new float[] {0, 0, contrast, 0, 0}, // scale blue
                    new float[] {0, 0, 0, 1.0f, 0}, // don't scale alpha
                    new float[] {brightness, brightness, brightness, 0, 1}
                };

                ApplyColorMatrix(modifiedBitmap, brightnessContrastArray);

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

                SetGamma(modifiedBitmap, gamma);

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
