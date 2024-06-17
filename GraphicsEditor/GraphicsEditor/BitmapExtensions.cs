using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace GraphicsEditor
{
    public static class BitmapExtensions
    {
        public static Bitmap NearestNeighbour(this Bitmap source, int newWidth, int newHeight)
        {
            var result = new Bitmap(newWidth, newHeight);

            var sourceData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);
            var sourcePixels = new byte[sourceData.Stride * sourceData.Height];
            Marshal.Copy(sourceData.Scan0, sourcePixels, 0, sourcePixels.Length);
            source.UnlockBits(sourceData);

            var widthRatio = (float)source.Width / newWidth;
            var heightRatio = (float)source.Height / newHeight;

            var resultData = result.LockBits(new Rectangle(0, 0, result.Width, result.Height), ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb);
            var resultPixels = new byte[resultData.Stride * resultData.Height];

            for (var y = 0; y < newHeight; y++)
            {
                for (var x = 0; x < newWidth; x++)
                {
                    var sourceX = (int)(x * widthRatio);
                    var sourceY = (int)(y * heightRatio);

                    var sourceIndex = sourceY * sourceData.Stride + sourceX * 4;
                    var resultIndex = y * resultData.Stride + x * 4;
                    Array.Copy(sourcePixels, sourceIndex, resultPixels, resultIndex, 4);
                }
            }

            Marshal.Copy(resultPixels, 0, resultData.Scan0, resultPixels.Length);
            result.UnlockBits(resultData);
            return result;
        }

        public static Bitmap NearestNeighbourWithOffsets(this Bitmap source, int newWidth, int newHeight, int sourceOffsetX,
            int sourceOffsetY, int offsetX, int offsetY, float widthRatio, float heightRatio)
        {
            var result = new Bitmap(newWidth, newHeight);

            var sourceData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);
            var sourcePixels = new byte[sourceData.Stride * sourceData.Height];
            Marshal.Copy(sourceData.Scan0, sourcePixels, 0, sourcePixels.Length);
            source.UnlockBits(sourceData);

            var resultData = result.LockBits(new Rectangle(0, 0, result.Width, result.Height), ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb);
            var resultPixels = new byte[resultData.Stride * resultData.Height];

            for (var y = 0; y < newHeight; y++)
            {
                for (var x = 0; x < newWidth; x++)
                {
                    var sourceX = (int)((x + offsetX) * widthRatio) - sourceOffsetX;
                    var sourceY = (int)((y + offsetY) * heightRatio) - sourceOffsetY;

                    var sourceIndex = sourceY * sourceData.Stride + sourceX * 4;
                    var resultIndex = y * resultData.Stride + x * 4;
                    Array.Copy(sourcePixels, sourceIndex, resultPixels, resultIndex, 4);
                }
            }

            Marshal.Copy(resultPixels, 0, resultData.Scan0, resultPixels.Length);
            result.UnlockBits(resultData);
            return result;
        }

        public static Bitmap Crop(this Bitmap image, int x, int y, int width, int height)
        {
            var result = new Bitmap(width, height);

            using (var graphics = Graphics.FromImage(result))
            {
                graphics.DrawImage(image, 0, 0, new Rectangle(x, y, width, height), GraphicsUnit.Pixel);
            }

            return result;
        }

        public static Bitmap Resize(this Bitmap image, int width, int height)
        {
            var resizedImage = new Bitmap(width, height);

            using (var graphics = Graphics.FromImage(resizedImage))
            {
                graphics.DrawImage(image, 0, 0, new Rectangle(0, 0, width, height), GraphicsUnit.Pixel);
            }

            return resizedImage;
        }
    }
}