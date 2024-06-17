using System.Drawing;

namespace GraphicsEditor
{
    public abstract class Tool
    {
        public Canvas Canvas;
        protected Bitmap display;
        protected Brush brush;

        public Tool(Canvas canvas, Bitmap display, Brush brush)
        {
            Canvas = canvas;
            this.display = display;
            this.brush = brush;
        }

        public abstract void HandleMouseMove(MouseContainer mouseContainer);
        public abstract void HandleMouseUp();
        public abstract void HandleScroll();
        public abstract void Apply();

        public virtual void Prepare(Bitmap display)
        {
            this.display = display;
        }
    }
}