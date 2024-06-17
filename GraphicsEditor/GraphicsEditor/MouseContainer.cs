namespace GraphicsEditor
{
    public enum MouseState
    {
        Pressed,
        Released
    }

    public class MouseContainer
    {
        public MouseState MouseState;
        public int X;
        public int Y;

        public MouseContainer(int X, int Y, MouseState mouseState)
        {
            this.X = X;
            this.Y = Y;
            MouseState = mouseState;
        }
    }
}