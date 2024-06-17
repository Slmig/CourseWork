using System.Drawing;
using GraphicsEditor.Tools;

namespace GraphicsEditor
{
    public class ToolsController
    {
        public enum ToolType
        {
            Pencil,
            Eraser,
            Fill,
            Pipet,
            SelectionRect
        }

        public ToolType SelectedTool { get; set; }
        public Brush Brush { get; set; }
        public Tool Tool { get; protected set; }

        public ToolsController()
        {
            Brush = new Brush(Color.Black, 16);
            SelectedTool = ToolType.Pencil;
        }

        public void Prepare(Canvas canvas, Bitmap display, object arg = null)
        {
            switch (SelectedTool)
            {
                case ToolType.Pencil:
                    if (!(Tool is PencilTool) || Tool.Canvas != canvas) Tool = new PencilTool(canvas, display, Brush);
                    else Tool.Prepare(display);
                    break;
                case ToolType.Eraser:
                    if (!(Tool is EraserTool) || Tool.Canvas != canvas) Tool = new EraserTool(canvas, display, Brush);
                    else Tool.Prepare(display);
                    break;
                case ToolType.Fill:
                    if (!(Tool is FillTool) || Tool.Canvas != canvas) Tool = new FillTool(canvas, display, Brush);
                    else Tool.Prepare(display);
                    break;
                case ToolType.Pipet:
                    if (!(Tool is PipetTool) || Tool.Canvas != canvas) Tool = new PipetTool(canvas, display, Brush);
                    else Tool.Prepare(display);
                    break;
                case ToolType.SelectionRect:
                    if (!(Tool is SelectionRectTool) || Tool.Canvas != canvas)
                        Tool = new SelectionRectTool(canvas, display, Brush, (Bitmap)arg);
                    else Tool.Prepare(display);
                    break;
            }
        }
    }
}