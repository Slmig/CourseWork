using System;
using System.Drawing;
using System.Windows.Forms;

namespace GraphicsEditor
{
    public partial class MainForm
    {
        public void Redraw()
        {
            framesController.CurrentFrame.Refresh();
            Rescale();
        }

        public void CenterCanvas()
        {
            var scrollXRatio = (float)splitContainer2.Panel1.HorizontalScroll.Value /
                               splitContainer2.Panel1.HorizontalScroll.Maximum;
            var scrollYRatio = (float)splitContainer2.Panel1.VerticalScroll.Value /
                               splitContainer2.Panel1.VerticalScroll.Maximum;
            splitContainer2.Panel1.VerticalScroll.Value = 0;
            splitContainer2.Panel1.HorizontalScroll.Value = 0;
            int x, y;
            if (splitContainer2.Panel1.Width > pictureBox.Width)
                x = (splitContainer2.Panel1.Width - pictureBox.Width) / 2;
            else x = 0;
            if (splitContainer2.Panel1.Height > pictureBox.Height)
                y = (splitContainer2.Panel1.Height - pictureBox.Height) / 2;
            else y = 0;
            pictureBox.Location = new Point(x, y);
            brushSizeTextBox.Select();
            splitContainer2.Panel1.HorizontalScroll.Value =
                (int)(scrollXRatio * splitContainer2.Panel1.HorizontalScroll.Maximum);
            splitContainer2.Panel1.VerticalScroll.Value =
                (int)(scrollYRatio * splitContainer2.Panel1.VerticalScroll.Maximum);
            splitContainer2.PerformLayout();
        }

        private void pictureBox_Resize(object sender, EventArgs e) => CenterCanvas();

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == 0 || animPlaying) return;
            mousePressed = false;

            if (framesController.CurrentFrame.Layers[framesController.CurrentFrame.ActiveLayerIndex].Hidden)
                return;

            if (toolsController.SelectedTool != ToolsController.ToolType.SelectionRect &&
                toolsController.SelectedTool != ToolsController.ToolType.Pipet)
            {
                HistoryController.ClearRedoStates();
                HistoryController.PushUndoState(framesController.CurrentFrame.ActiveLayer);
            }

            toolsController.Tool?.HandleMouseUp();
            if (toolsController.SelectedTool == ToolsController.ToolType.Pipet &&
                paletteColors[paletteIndex] != toolsController.Brush.Color)
                updatePaletteColor(toolsController.Brush.Color);

            if (toolsController.SelectedTool != ToolsController.ToolType.SelectionRect &&
                toolsController.SelectedTool != ToolsController.ToolType.Pipet) Redraw();
        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == 0 || animPlaying) return;

            mousePressed = true;
            if (!framesController.CurrentFrame.Layers[framesController.CurrentFrame.ActiveLayerIndex].Hidden)
                toolsController.Tool?.HandleMouseMove(new MouseContainer(e.X, e.Y,
                    mousePressed ? MouseState.Pressed : MouseState.Released));
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (animPlaying) return;

            if (!framesController.CurrentFrame.Layers[framesController.CurrentFrame.ActiveLayerIndex].Hidden)
                toolsController.Tool?.HandleMouseMove(new MouseContainer(e.X, e.Y,
                    mousePressed ? MouseState.Pressed : MouseState.Released));
        }

        private void pictureBox_MouseEnter(object sender, EventArgs e)
        {
            if (animPlaying) return;
            toolsController.Prepare(framesController.CurrentFrame, display);
            ChangeCursor();
        }

        private void pictureBox_MouseLeave(object sender, EventArgs e)
        {
            if (animPlaying) return;

            Cursor = Cursors.Default;
            if (toolsController.SelectedTool != ToolsController.ToolType.SelectionRect &&
                toolsController.SelectedTool != ToolsController.ToolType.Pipet) pictureBox.Refresh();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            CenterCanvas();
            toolsController.Tool?.HandleScroll();
        }
    }
}