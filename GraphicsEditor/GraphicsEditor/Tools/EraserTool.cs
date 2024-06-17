using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace GraphicsEditor
{
    public class EraserTool : Tool
    {
        private readonly HashSet<Point> points = new HashSet<Point>();
        public EraserTool(Canvas canvas, Bitmap display, Brush brush) : base(canvas, display, brush) { }

        public override void HandleMouseMove(MouseContainer mouseContainer)
        {
            var appliedImage = new Bitmap(brush.Size, brush.Size);

            if (brush.Shape == Brush.BrushShape.Circle) GraphicsMethods.DrawCircle(appliedImage, 0, 0, brush.Size, brush.Color);
            else
                using (var g = Graphics.FromImage(appliedImage))
                {
                    using (var b = new SolidBrush(brush.Color))
                    {
                        g.FillRectangle(b, 0, 0, brush.Size, brush.Size);
                    }
                }

            var widthRatio = (float)Canvas.Image.Width / display.Width;
            var heightRatio = (float)Canvas.Image.Height / display.Height;
            var x = (int)(mouseContainer.X * widthRatio) - brush.Size / 2;
            var y = (int)(mouseContainer.Y * heightRatio) - brush.Size / 2;
            int scaledX = 0, scaledY = 0, scaledWidth = 0, scaledHeight = 0;
            GraphicsMethods.SetRectParams(x, y, brush.Size, brush.Size, widthRatio, heightRatio, ref scaledX, ref scaledY,
                ref scaledWidth, ref scaledHeight);
            if (scaledWidth <= 0 || scaledHeight <= 0) return;

            var scaledAppliedImage =
                appliedImage.NearestNeighbourWithOffsets(scaledWidth, scaledHeight, x, y, scaledX, scaledY, widthRatio,
                    heightRatio);
            var scaledAppliedImageContour = GraphicsMethods.GetEdgeContour(scaledAppliedImage, display, scaledX, scaledY);

            if (mouseContainer.MouseState == MouseState.Pressed)
            {
                var scaledBackground = Canvas.Background.Crop(x, y, brush.Size, brush.Size)
                                             .NearestNeighbour(scaledWidth, scaledHeight);
                var scaledForeground = Canvas.Foreground.Crop(x, y, brush.Size, brush.Size)
                                             .NearestNeighbour(scaledWidth, scaledHeight);
                GraphicsMethods.Erase(display, scaledBackground, scaledAppliedImage, scaledX, scaledY);

                using (var graphics = Graphics.FromImage(display))
                {
                    graphics.DrawImage(scaledForeground, scaledX, scaledY, new Rectangle(0, 0, scaledWidth, scaledHeight),
                        GraphicsUnit.Pixel);
                }

                MainForm.ImageRefresh();
                points.Add(new Point(x, y));
            }

            var saveImage = new Bitmap(scaledWidth, scaledHeight);

            using (var graphics = Graphics.FromImage(saveImage))
            {
                graphics.DrawImage(display, 0, 0, new Rectangle(scaledX, scaledY, scaledWidth, scaledHeight),
                    GraphicsUnit.Pixel);
            }

            using (var graphics = Graphics.FromImage(display))
            {
                graphics.DrawImage(scaledAppliedImageContour, scaledX, scaledY, new Rectangle(0, 0, scaledWidth, scaledHeight),
                    GraphicsUnit.Pixel);
                MainForm.ImageRefresh();
                graphics.DrawImage(saveImage, scaledX, scaledY, new Rectangle(0, 0, scaledWidth, scaledHeight),
                    GraphicsUnit.Pixel);
            }
        }

        public override void HandleMouseUp()
        {
            Apply();
        }

        public override void HandleScroll() { }

        public override void Apply()
        {
            if (points.Count > 0)
            {
                var appliedImage = new Bitmap(brush.Size, brush.Size);

                if (brush.Shape == Brush.BrushShape.Circle)
                    GraphicsMethods.DrawCircle(appliedImage, 0, 0, brush.Size, brush.Color);
                else
                    using (var g = Graphics.FromImage(appliedImage))
                    {
                        using (System.Drawing.Brush b = new SolidBrush(brush.Color))
                        {
                            g.FillRectangle(b, 0, 0, brush.Size, brush.Size);
                        }
                    }

                var appliedImageData = appliedImage.LockBits(new Rectangle(0, 0, appliedImage.Width, appliedImage.Height),
                    ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                var appliedImagePixels = new byte[appliedImageData.Stride * appliedImageData.Height];
                Marshal.Copy(appliedImageData.Scan0, appliedImagePixels, 0, appliedImagePixels.Length);

                var imageData = Canvas.ActiveLayer.Image.LockBits(
                    new Rectangle(0, 0, Canvas.ActiveLayer.Image.Width, Canvas.ActiveLayer.Image.Height),
                    ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                var imagePixels = new byte[imageData.Stride * imageData.Height];
                Marshal.Copy(imageData.Scan0, imagePixels, 0, imagePixels.Length);

                foreach (var point in points)
                    for (var x = 0; x < appliedImage.Width; x++)
                    for (var y = 0; y < appliedImage.Height; y++)
                        if (point.X + x >= 0 &&
                            point.X + x < Canvas.ActiveLayer.Image.Width &&
                            point.Y + y >= 0 &&
                            point.Y + y < Canvas.ActiveLayer.Image.Height)
                            if (appliedImagePixels[appliedImageData.Stride * y + 4 * x + 3] != 0)
                            {
                                var index = (point.X + x) * 4 + (point.Y + y) * imageData.Stride;
                                for (var d = 0; d < 4; d++) imagePixels[index + d] = 0;
                            }

                points.Clear();
                Marshal.Copy(imagePixels, 0, imageData.Scan0, imagePixels.Length);
                Canvas.ActiveLayer.Image.UnlockBits(imageData);
                appliedImage.UnlockBits(appliedImageData);
            }
        }
    }
}