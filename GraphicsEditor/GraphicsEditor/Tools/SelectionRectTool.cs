using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace GraphicsEditor
{
    public class SelectionRectTool : Tool
    {
        private enum AreaType
        {
            Move,
            StretchLeft,
            StretchRight,
            StretchUp,
            StretchDown,
            None
        }

        private Bitmap image;
        private bool selected;
        private List<Point> points = new List<Point>();
        private Point triggerPoint;
        private AreaType areaType = AreaType.None;
        private const int deltaSide = 2;

        public SelectionRectTool(Canvas canvas, Bitmap display, Brush brush, Bitmap image = null)
            : base(canvas, display, brush)
        {
            if (image != null)
            {
                HistoryController.ClearRedoStates();
                HistoryController.PushUndoState(Canvas.ActiveLayer);
                this.image = image;
                points.Add(new Point(0, 0));
                int width, height;
                if (image.Width <= canvas.Width && image.Height <= canvas.Height)
                {
                    width = canvas.Width;
                    height = canvas.Height;
                }
                else if ((float)image.Width / canvas.Width >= (float)image.Height / canvas.Height)
                {
                    var buf = image.Width;
                    width = canvas.Width;
                    height = (int)((float)width / buf * image.Height);
                }
                else
                {
                    var buf = image.Height;
                    height = canvas.Height;
                    width = (int)((float)height / buf * image.Width);
                }

                points.Add(new Point(width, height));
                selected = true;
                insertImage(width, height, (float)Canvas.Image.Width / display.Width,
                    (float)Canvas.Image.Height / display.Height);
            }
        }

        public override void HandleMouseMove(MouseContainer mouseContainer)
        {
            var widthRatio = (float)Canvas.Image.Width / display.Width;
            var heightRatio = (float)Canvas.Image.Height / display.Height;
            var x = (int)(mouseContainer.X * widthRatio);
            var y = (int)(mouseContainer.Y * heightRatio);
            if (x < 0) x = 0;
            else if (x >= Canvas.Width) x = Canvas.Width - 1;
            if (y < 0) y = 0;
            else if (y >= Canvas.Height) y = Canvas.Height - 1;

            if (!selected)
            {
                if (mouseContainer.MouseState == MouseState.Pressed)
                {
                    if (points.Count == 0)
                        points.Add(new Point(x, y));
                    else
                    {
                        if (points.Count == 2) points.RemoveAt(1);
                        if (points[0].X != x && points[0].Y != y) points.Add(new Point(x, y));
                        if (points.Count != 2) return;

                        var width = Math.Abs(points[0].X - points[1].X) + 1;
                        var height = Math.Abs(points[0].Y - points[1].Y) + 1;
                        int scaledX = 0, scaledY = 0, scaledWidth = 0, scaledHeight = 0;
                        GraphicsMethods.SetRectParams(Math.Min(points[0].X, points[1].X),
                            Math.Min(points[0].Y, points[1].Y),
                            width, height, widthRatio, heightRatio, ref scaledX, ref scaledY, ref scaledWidth,
                            ref scaledHeight);
                        var appliedImage = new Bitmap(scaledWidth, scaledHeight);

                        using (var graphics = Graphics.FromImage(appliedImage))
                        {
                            using (var brush = new SolidBrush(Color.Black))
                            {
                                graphics.FillRectangle(brush, 0, 0, scaledWidth, scaledHeight);
                            }
                        }

                        appliedImage = GraphicsMethods.GetEdgeContour(appliedImage, display, scaledX, scaledY);
                        var saveImage = new Bitmap(scaledWidth, scaledHeight);

                        using (var graphics = Graphics.FromImage(saveImage))
                        {
                            graphics.DrawImage(display, 0, 0,
                                new Rectangle(scaledX, scaledY, scaledWidth, scaledHeight),
                                GraphicsUnit.Pixel);
                        }

                        using (var graphics = Graphics.FromImage(display))
                        {
                            graphics.DrawImage(appliedImage, scaledX, scaledY,
                                new Rectangle(0, 0, scaledWidth, scaledHeight),
                                GraphicsUnit.Pixel);
                            MainForm.ImageRefresh();
                            graphics.DrawImage(saveImage, scaledX, scaledY,
                                new Rectangle(0, 0, scaledWidth, scaledHeight),
                                GraphicsUnit.Pixel);
                        }
                    }
                }
                else if (points.Count == 2)
                {
                    var newPoints = new Point[2];
                    newPoints[0].X = points[0].X <= points[1].X ? points[0].X : points[1].X;
                    newPoints[0].Y = points[0].Y <= points[1].Y ? points[0].Y : points[1].Y;
                    newPoints[1].X = points[0].X >= points[1].X ? points[0].X : points[1].X;
                    newPoints[1].Y = points[0].Y >= points[1].Y ? points[0].Y : points[1].Y;
                    points = newPoints.ToList();
                    var width = points[1].X - points[0].X + 1;
                    var height = points[1].Y - points[0].Y + 1;
                    image = Canvas.ActiveLayer.Image.Crop(points[0].X, points[0].Y, width, height);
                    HistoryController.ClearRedoStates();
                    HistoryController.PushUndoState(Canvas.ActiveLayer);
                    eraseRect(points[0].X, points[0].Y, width, height);
                    MainForm.ImageRedraw();
                    insertImage(image.Width, image.Height, widthRatio, heightRatio);
                    selected = true;
                }
            }
            else
            {
                if (mouseContainer.MouseState == MouseState.Released)
                {
                    var deltas = new[]
                    {
                        Math.Abs(points[0].X - x), Math.Abs(points[0].Y - y), Math.Abs(points[1].X - x),
                        Math.Abs(points[1].Y - y)
                    };
                    var index = Array.IndexOf(deltas, deltas.Min());

                    if (deltas.Min() < deltaSide)
                        switch (index)
                        {
                            case 0:
                                areaType = AreaType.StretchLeft;
                                break;
                            case 1:
                                areaType = AreaType.StretchUp;
                                break;
                            case 2:
                                areaType = AreaType.StretchRight;
                                break;
                            case 3:
                                areaType = AreaType.StretchDown;
                                break;
                        }
                    else if (x >= points[0].X && x <= points[1].X && y >= points[0].Y && y < points[1].Y)
                        areaType = AreaType.Move;
                    else areaType = AreaType.None;

                    triggerPoint = new Point(x, y);
                }
                else if (areaType != AreaType.None)
                {
                    if (areaType == AreaType.Move)
                        for (var i = 0; i <= 1; i++)
                            points[i] = new Point(points[i].X + x - triggerPoint.X, points[i].Y + y - triggerPoint.Y);
                    else if (areaType == AreaType.StretchLeft)
                    {
                        var newX = points[0].X + x - triggerPoint.X;
                        if (newX < 0 || points[1].X - newX + 1 <= 1) return;

                        points[0] = new Point(newX, points[0].Y);
                    }
                    else if (areaType == AreaType.StretchRight)
                    {
                        var newX = points[1].X + x - triggerPoint.X;
                        if (newX >= Canvas.Width || newX - points[0].X + 1 <= 1) return;

                        points[1] = new Point(newX, points[1].Y);
                    }
                    else if (areaType == AreaType.StretchUp)
                    {
                        var newY = points[0].Y + y - triggerPoint.Y;
                        if (newY < 0 || points[1].Y - newY + 1 <= 1) return;

                        points[0] = new Point(points[0].X, newY);
                    }
                    else if (areaType == AreaType.StretchDown)
                    {
                        var newY = points[1].Y + y - triggerPoint.Y;
                        if (newY >= Canvas.Height || newY - points[0].Y + 1 <= 1) return;

                        points[1] = new Point(points[1].X, newY);
                    }

                    insertImage(points[1].X - points[0].X + 1, points[1].Y - points[0].Y + 1, widthRatio, heightRatio);
                    triggerPoint = new Point(x, y);
                }
                else Apply();
            }

            changeCursor();
        }

        public override void HandleMouseUp()
        {
            if (selected && areaType == AreaType.None) Apply();
        }

        public override void HandleScroll()
        {
            if (selected)
            {
                var width = points[1].X - points[0].X + 1;
                var height = points[1].Y - points[0].Y + 1;
                var widthRatio = (float)Canvas.Image.Width / display.Width;
                var heightRatio = (float)Canvas.Image.Height / display.Height;
                insertImage(width, height, widthRatio, heightRatio);
            }
        }

        public override void Apply()
        {
            if (selected)
            {
                var width = points[1].X - points[0].X + 1;
                var height = points[1].Y - points[0].Y + 1;
                var stretchedImage = image.NearestNeighbour(width, height);

                using (var graphics = Graphics.FromImage(Canvas.ActiveLayer.Image))
                {
                    graphics.DrawImage(stretchedImage, points[0].X, points[0].Y, new Rectangle(0, 0, width, height),
                        GraphicsUnit.Pixel);
                }

                selected = false;
                points.Clear();
                areaType = AreaType.None;
                MainForm.ImageRedraw();
            }
        }

        private void insertImage(int width, int height, float widthRatio, float heightRatio)
        {
            Bitmap scaledImage;
            if (image.Width == width && image.Height == height) scaledImage = (Bitmap)image.Clone();
            else scaledImage = image.NearestNeighbour(width, height);
            using (var graphics = Graphics.FromImage(scaledImage))
            {
                graphics.DrawImage(Canvas.Foreground, 0, 0, new Rectangle(points[0].X, points[0].Y, width, height),
                    GraphicsUnit.Pixel);
            }

            int scaledX = 0, scaledY = 0, scaledWidth = 0, scaledHeight = 0;
            GraphicsMethods.SetRectParams(points[0].X, points[0].Y, width, height, widthRatio, heightRatio, ref scaledX,
                ref scaledY, ref scaledWidth, ref scaledHeight);
            scaledImage = scaledImage.NearestNeighbour(scaledWidth, scaledHeight);
            using (var graphics = Graphics.FromImage(scaledImage))
            {
                using (var brush = new Pen(Color.White, 1))
                {
                    graphics.DrawRectangle(brush, 0, 0, scaledImage.Width - 1, scaledImage.Height - 1);
                }

                using (var brush = new Pen(Color.Black, 1))
                {
                    float[] pattern = { 4.0f, 2.0f };
                    brush.DashPattern = pattern;
                    graphics.DrawRectangle(brush, 0, 0, scaledImage.Width - 1, scaledImage.Height - 1);
                }
            }

            var saveImage = new Bitmap(scaledWidth, scaledHeight);

            using (var graphics = Graphics.FromImage(saveImage))
            {
                graphics.DrawImage(display, 0, 0, new Rectangle(scaledX, scaledY, scaledWidth, scaledHeight),
                    GraphicsUnit.Pixel);
            }

            using (var graphics = Graphics.FromImage(display))
            {
                graphics.DrawImage(scaledImage, scaledX, scaledY, new Rectangle(0, 0, scaledWidth, scaledHeight),
                    GraphicsUnit.Pixel);
                MainForm.ImageRefresh();
                graphics.DrawImage(saveImage, scaledX, scaledY, new Rectangle(0, 0, scaledWidth, scaledHeight),
                    GraphicsUnit.Pixel);
            }
        }

        private void eraseRect(int x, int y, int width, int height)
        {
            var bg = new Bitmap(width, height);
            var mask = new Bitmap(width, height);

            using (var graphics = Graphics.FromImage(mask))
            {
                using (var brush = new SolidBrush(Color.Black))
                {
                    graphics.FillRectangle(brush, new Rectangle(0, 0, width, height));
                }
            }

            GraphicsMethods.Erase(Canvas.ActiveLayer.Image, bg, mask, x, y);
        }

        private void changeCursor()
        {
            switch (areaType)
            {
                case AreaType.StretchLeft:
                case AreaType.StretchRight:
                    Cursor.Current = Cursors.SizeWE;
                    break;
                case AreaType.StretchUp:
                case AreaType.StretchDown:
                    Cursor.Current = Cursors.SizeNS;
                    break;
                case AreaType.Move:
                    Cursor.Current = Cursors.SizeAll;
                    break;
                default:
                    Cursor.Current = MainForm.rectCursor;
                    break;
            }
        }

        public override void Prepare(Bitmap display)
        {
            base.Prepare(display);

            if (selected)
            {
                var widthRatio = (float)Canvas.Image.Width / display.Width;
                var heightRatio = (float)Canvas.Image.Height / display.Height;
                insertImage(points[1].X - points[0].X + 1, points[1].Y - points[0].Y + 1, widthRatio, heightRatio);
            }
        }
    }
}