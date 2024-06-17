using System.Collections.Generic;
using System.Drawing;

namespace GraphicsEditor
{
    public class PencilTool : Tool
    {
        private readonly HashSet<Point> points = new HashSet<Point>();
        public PencilTool(Canvas canvas, Bitmap display, Brush brush) : base(canvas, display, brush) { }

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

            using (var graphics = Graphics.FromImage(appliedImage))
            {
                graphics.DrawImage(Canvas.Foreground, 0, 0, new Rectangle(x, y, brush.Size, brush.Size),
                    GraphicsUnit.Pixel);
            }

            var scaledAppliedImage =
                appliedImage.NearestNeighbourWithOffsets(scaledWidth, scaledHeight, x, y, scaledX, scaledY, widthRatio,
                    heightRatio);

            if (mouseContainer.MouseState == MouseState.Released)
            {
                var saveImage = new Bitmap(scaledWidth, scaledHeight);

                using (var graphics = Graphics.FromImage(saveImage))
                {
                    graphics.DrawImage(display, 0, 0, new Rectangle(scaledX, scaledY, scaledWidth, scaledHeight),
                        GraphicsUnit.Pixel);
                }

                using (var graphics = Graphics.FromImage(display))
                {
                    graphics.DrawImage(scaledAppliedImage, scaledX, scaledY, new Rectangle(0, 0, scaledWidth, scaledHeight),
                        GraphicsUnit.Pixel);
                    MainForm.ImageRefresh();
                    graphics.DrawImage(saveImage, scaledX, scaledY, new Rectangle(0, 0, scaledWidth, scaledHeight),
                        GraphicsUnit.Pixel);
                }
            }
            else if (mouseContainer.MouseState == MouseState.Pressed)
            {
                using (var graphics = Graphics.FromImage(display))
                {
                    graphics.DrawImage(scaledAppliedImage, scaledX, scaledY, new Rectangle(0, 0, scaledWidth, scaledHeight),
                        GraphicsUnit.Pixel);
                }

                MainForm.ImageRefresh();
                points.Add(new Point(x, y));
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

                using (var graphics = Graphics.FromImage(Canvas.ActiveLayer.Image))
                {
                    foreach (var point in points)
                        graphics.DrawImage(appliedImage, point.X, point.Y, new Rectangle(0, 0, brush.Size, brush.Size),
                            GraphicsUnit.Pixel);
                }

                points.Clear();
            }
        }
    }
}