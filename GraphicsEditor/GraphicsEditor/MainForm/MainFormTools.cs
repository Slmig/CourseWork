using System;
using System.Threading;
using System.Threading.Tasks;

namespace GraphicsEditor
{
    public partial class MainForm
    {
        private void pencilButton_Click(object sender, EventArgs e)
        {
            if (toolsController.SelectedTool == ToolsController.ToolType.SelectionRect)
                toolsController.Tool?.Apply();
            toolsController.SelectedTool = ToolsController.ToolType.Pencil;
        }

        private void eraserButton_Click(object sender, EventArgs e)
        {
            if (toolsController.SelectedTool == ToolsController.ToolType.SelectionRect)
                toolsController.Tool?.Apply();
            toolsController.SelectedTool = ToolsController.ToolType.Eraser;
        }

        private void fillButton_Click(object sender, EventArgs e)
        {
            if (toolsController.SelectedTool == ToolsController.ToolType.SelectionRect)
                toolsController.Tool?.Apply();
            toolsController.SelectedTool = ToolsController.ToolType.Fill;
        }

        private void pipetButton_Click(object sender, EventArgs e)
        {
            if (toolsController.SelectedTool == ToolsController.ToolType.SelectionRect)
                toolsController.Tool?.Apply();
            toolsController.SelectedTool = ToolsController.ToolType.Pipet;
        }

        private void rectToolButton_Click(object sender, EventArgs e)
        {
            if (toolsController.SelectedTool == ToolsController.ToolType.SelectionRect)
                toolsController.Tool?.Apply();
            toolsController.SelectedTool = ToolsController.ToolType.SelectionRect;
        }
    }
}