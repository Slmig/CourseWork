using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace GraphicsEditor
{
    public static class GraphicsMethods
    {
        private static bool IsBorderPixel(BitmapData imageData, int x, int y)
        {
            if (x <= 0 || y <= 0 || x >= imageData.Width - 1 || y >= imageData.Height - 1) return true;

            var _stride = imageData.Stride;

            for (var offsetY = -1; offsetY <= 1; offsetY++)
            for (var offsetX = -1; offsetX <= 1; offsetX++)
            {
                var index = (y + offsetY) * _stride + (x + offsetX) * 4;
                var alpha = Marshal.ReadByte((IntPtr)(imageData.Scan0.ToInt64() + index + 3));
                if (alpha == 0)
                    return true;
            }

            return false;
        }

        public static Bitmap GetEdgeContour(Bitmap appliedImage, Bitmap scaledImage, int scaledX, int scaledY)
        {
            var contourImage = new Bitmap(appliedImage.Width, appliedImage.Height);

            var originalData = appliedImage.LockBits(new Rectangle(0, 0, appliedImage.Width, appliedImage.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);
            var originalPixels = new byte[originalData.Stride * originalData.Height];
            Marshal.Copy(originalData.Scan0, originalPixels, 0, originalPixels.Length);

            var contourData = contourImage.LockBits(new Rectangle(0, 0, contourImage.Width, contourImage.Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb);
            var contourPixels = new byte[contourData.Stride * contourData.Height];
            Marshal.Copy(contourData.Scan0, contourPixels, 0, contourPixels.Length);

            var backgroundData = scaledImage.LockBits(new Rectangle(0, 0, scaledImage.Width, scaledImage.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);
            var backgroundPixels = new byte[backgroundData.Stride * backgroundData.Height];
            Marshal.Copy(backgroundData.Scan0, backgroundPixels, 0, backgroundPixels.Length);

            var stride = originalData.Stride;
            var width = originalData.Width;
            var height = originalData.Height;

            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
            {
                var index = y * stride + x * 4;
                if (originalPixels[index + 3] <= 0 || !IsBorderPixel(originalData, x, y))
                    continue;

                var sourceX = scaledX + x;
                var sourceY = scaledY + y;
                if (sourceX < 0 || sourceX >= scaledImage.Width || sourceY < 0 || sourceY >= scaledImage.Height)
                    continue;

                var sourceIndex = backgroundData.Stride * sourceY + sourceX * 4;
                var brightness = 0;
                var count = 0;

                for (var i = -1; i <= 1; i++)
                for (var j = -1; j <= 1; j++)
                {
                    if (sourceX + i >= 0 &&
                        sourceX + i < scaledImage.Width &&
                        sourceY + j >= 0 &&
                        sourceY + j < scaledImage.Height)
                    {
                        var value = sourceIndex + backgroundData.Stride * j + 4 * i;
                        brightness += (backgroundPixels[value] + backgroundPixels[value + 1] + backgroundPixels[value + 2]) / 3;
                        count++;
                    }
                }

                brightness /= count;

                if (brightness > 100)
                {
                    for (var d = 0; d < 3; d++) contourPixels[index + d] = 0;
                    contourPixels[index + 3] = 255;
                }
                else
                    for (var d = 0; d < 4; d++)
                        contourPixels[index + d] = 255;
            }

            Marshal.Copy(contourPixels, 0, contourData.Scan0, contourPixels.Length);
            appliedImage.UnlockBits(originalData);
            contourImage.UnlockBits(contourData);
            scaledImage.UnlockBits(backgroundData);
            return contourImage;
        }

        public static void Erase(Bitmap image, Bitmap background, Bitmap mask, int x, int y)
        {
            var imageData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb);
            var imagePixels = new byte[imageData.Stride * imageData.Height];
            Marshal.Copy(imageData.Scan0, imagePixels, 0, imagePixels.Length);

            var backgroundData = background.LockBits(new Rectangle(0, 0, background.Width, background.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);
            var backgroundPixels = new byte[backgroundData.Stride * backgroundData.Height];
            Marshal.Copy(backgroundData.Scan0, backgroundPixels, 0, backgroundPixels.Length);

            var maskData = mask.LockBits(new Rectangle(0, 0, mask.Width, mask.Height), ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);
            var maskPixels = new byte[maskData.Stride * maskData.Height];
            Marshal.Copy(maskData.Scan0, maskPixels, 0, maskPixels.Length);

            for (var i = 0; i < mask.Width; i++)
            for (var j = 0; j < mask.Height; j++)
            {
                var maskIndex = j * maskData.Stride + i * 4;

                if (maskPixels[maskIndex + 3] > 0)
                    if (x + i >= 0 && x + i < image.Width && y + j >= 0 && y + j < image.Height)
                    {
                        var index = (y + j) * imageData.Stride + (i + x) * 4;
                        for (var d = 0; d < 4; d++) imagePixels[index + d] = backgroundPixels[maskIndex + d];
                    }
            }

            Marshal.Copy(imagePixels, 0, imageData.Scan0, imagePixels.Length);
            image.UnlockBits(imageData);
            background.UnlockBits(backgroundData);
            mask.UnlockBits(maskData);
        }

        public static void Fill(Bitmap image, Point startPoint, Color fillColor)
        {
            bool compareColors(Color color1, Color color2) => color1.A == color2.A && color1.R == color2.R &&
                                                              color1.G == color2.G && color1.B == color2.B;
            var imageData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite,
                PixelFormat.Format32bppArgb);
            var imagePixels = new byte[imageData.Stride * imageData.Height];
            Marshal.Copy(imageData.Scan0, imagePixels, 0, imagePixels.Length);
            var queue = new Queue<Point>();
            queue.Enqueue(startPoint);
            var startIndex = startPoint.X * 4 + startPoint.Y * imageData.Stride;
            var startColor = Color.FromArgb(imagePixels[startIndex + 3], imagePixels[startIndex + 2],
                imagePixels[startIndex + 1], imagePixels[startIndex]);
            if (!compareColors(startColor, fillColor))
            {
                while (queue.Count > 0)
                {
                    var point = queue.Dequeue();

                    if (point.X < 0 || point.X >= image.Width || point.Y < 0 || point.Y >= image.Height)
                        continue;

                    var index = point.X * 4 + point.Y * imageData.Stride;
                    var currentColor = Color.FromArgb(imagePixels[index + 3], imagePixels[index + 2],
                        imagePixels[index + 1],
                        imagePixels[index]);

                    if (!compareColors(currentColor, startColor))
                        continue;

                    imagePixels[index] = fillColor.B;
                    imagePixels[index + 1] = fillColor.G;
                    imagePixels[index + 2] = fillColor.R;
                    imagePixels[index + 3] = fillColor.A;
                    var neighbors = new[]
                    {
                        new Point(point.X + 1, point.Y),
                        new Point(point.X - 1, point.Y),
                        new Point(point.X, point.Y + 1),
                        new Point(point.X, point.Y - 1)
                    };

                    foreach (var neighbor in neighbors)
                    {
                        queue.Enqueue(neighbor);
                    }
                }
                Marshal.Copy(imagePixels, 0, imageData.Scan0, imagePixels.Length);
            }

            image.UnlockBits(imageData);
        }

        public static void DrawCircle(Bitmap bmp, int X, int Y, int diameter, Color color)
        {
            var rect = new Rectangle(X, Y, diameter, diameter);
            var bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);
            var stride = bmpData.Stride;
            var height = bmpData.Height;

            var pixelValues = new byte[stride * height];
            var center = ((float)diameter - 1) / 2;
            var radius = (float)diameter / 2;

            for (var y = 0; y < height; y++)
            for (var x = 0; x < diameter; x++)
            {
                var deltaX = x - center;
                var deltaY = y - center;
                var distanceSquared = (int)(deltaX * deltaX + deltaY * deltaY);

                if (distanceSquared <= radius * radius)
                {
                    var index = y * stride + x * 4;
                    pixelValues[index] = color.B;
                    pixelValues[index + 1] = color.G;
                    pixelValues[index + 2] = color.R;
                    pixelValues[index + 3] = color.A;
                }
            }

            Marshal.Copy(pixelValues, 0, bmpData.Scan0, pixelValues.Length);
            bmp.UnlockBits(bmpData);
        }

        public static void SetTransparency(Bitmap bmp, byte transparency)
        {
            var bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite,
                PixelFormat.Format32bppArgb);
            var bmpPixels = new byte[bmpData.Stride * bmpData.Height];
            Marshal.Copy(bmpData.Scan0, bmpPixels, 0, bmpPixels.Length);

            for (var y = 0; y < bmp.Height; y++)
            {
                var index = y * bmpData.Stride;

                for (var x = 0; x < bmp.Width; x++)
                {
                    index += x * 4;
                    if (bmpPixels[index + 3] > 0) bmpPixels[index + 3] = transparency;
                    index -= x * 4;
                }
            }

            Marshal.Copy(bmpPixels, 0, bmpData.Scan0, bmpPixels.Length);
            bmp.UnlockBits(bmpData);
        }

        public static void SetRectParams(int x, int y, int width, int height, float widthRatio, float heightRatio,
            ref int scaledX,
            ref int scaledY, ref int scaledWidth, ref int scaledHeight)
        {
            void TransformLower(ref int scaled, int source, float ratio)
            {
                while (true)
                {
                    if ((int)(scaled * ratio) >= source) return;

                    scaled++;
                }
            }

            void TransformHigher(ref int scaled, int source, float ratio)
            {
                while (true)
                {
                    if ((int)(scaled * ratio) <= source)
                    {
                        scaled++;
                        return;
                    }

                    scaled--;
                }
            }

            scaledX = (int)((x - 1) / widthRatio);
            scaledY = (int)((y - 1) / heightRatio);
            TransformLower(ref scaledX, x, widthRatio);
            TransformLower(ref scaledY, y, heightRatio);
            var scaledXEnd = (int)((x + width) / widthRatio);
            var scaledYEnd = (int)((y + height) / heightRatio);
            TransformHigher(ref scaledXEnd, x + width - 1, widthRatio);
            TransformHigher(ref scaledYEnd, y + height - 1, heightRatio);
            scaledWidth = scaledXEnd - scaledX;
            scaledHeight = scaledYEnd - scaledY;
        }
    }
}