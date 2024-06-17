using System.Drawing;

namespace GraphicsEditor
{
    public class Brush
    {
        public enum BrushShape
        {
            Square,
            Circle
        }

        public Color Color { get; set; }
        public int Size { get; set; }
        public BrushShape Shape { get; set; }

        public Brush(Color color, int size, BrushShape shape)
        {
            Color = color;
            Size = size;
            Shape = shape;
        }

        public Brush(Color color, int size)
        {
            Color = color;
            Size = size;
            Shape = BrushShape.Circle;
        }
    }
}