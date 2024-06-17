using System.Drawing;

namespace GraphicsEditor
{
    public class PipetTool : Tool
    {
        private Color colorBuf;
        public PipetTool(Canvas canvas, Bitmap display, Brush brush) : base(canvas, display, brush) { }

        public override void HandleMouseMove(MouseContainer mouseContainer)
        {
            if (mouseContainer.MouseState == MouseState.Pressed)
                if (mouseContainer.X >= 0 &&
                    mouseContainer.X < display.Width &&
                    mouseContainer.Y >= 0 &&
                    mouseContainer.Y < display.Height)
                {
                    var color = display.GetPixel(mouseContainer.X, mouseContainer.Y);
                    if (color.A > 0) colorBuf = color;
                }
        }

        public override void HandleMouseUp()
        {
            Apply();
        }

        public override void HandleScroll() { }

        public override void Apply()
        {
            brush.Color = colorBuf;
        }
    }
}