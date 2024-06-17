using System.Drawing;

namespace GraphicsEditor.Tools
{
    public class FillTool : Tool
    {
        private Point? fillPoint;
        public FillTool(Canvas canvas, Bitmap display, Brush brush) : base(canvas, display, brush) { }

        public override void HandleMouseMove(MouseContainer mouseContainer)
        {
            if (mouseContainer.MouseState == MouseState.Pressed)
            {
                var widthRatio = (float)Canvas.Image.Width / display.Width;
                var heightRatio = (float)Canvas.Image.Height / display.Height;
                var x = (int)(mouseContainer.X * widthRatio);
                var y = (int)(mouseContainer.Y * heightRatio);
                fillPoint = new Point(x, y);
            }
        }

        public override void HandleMouseUp()
        {
            Apply();
        }

        public override void HandleScroll() { }

        public override void Apply()
        {
            if (fillPoint != null)
            {
                GraphicsMethods.Fill(Canvas.ActiveLayer.Image, fillPoint.Value, brush.Color);
                Canvas.Refresh();
            }
        }
    }
}