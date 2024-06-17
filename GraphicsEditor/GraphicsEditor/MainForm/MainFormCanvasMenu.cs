using System;
using System.Windows.Forms;

namespace GraphicsEditor
{
    public partial class MainForm
    {
        private void resizeCanvasMenuButton_Click(object sender, EventArgs e)
        {
            if (animPlaying) return;
            using (var dialog = new CanvasResizeDialog(framesController.CurrentFrame.Width, framesController.CurrentFrame.Height))
            {
                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                foreach (var frame in framesController.Frames)
                    frame.Resize(dialog.Width, dialog.Height);
                Redraw();
                HistoryController.ClearUndoStates();
                HistoryController.ClearRedoStates();
            }
        }

        private void turnRightCanvasMenuButton_Click(object sender, EventArgs e)
        {
            if (animPlaying) return;
            foreach (var frame in framesController.Frames)
                frame.TurnRight();
            if (ConfirmDialog())
            {
                HistoryController.ClearUndoStates();
                HistoryController.ClearRedoStates();
            }

            Redraw();
        }

        private void turnLeftCanvasMenuButton_Click(object sender, EventArgs e)
        {
            if (animPlaying) return;
            foreach (var frame in framesController.Frames)
                frame.TurnLeft();
            if (ConfirmDialog())
            {
                HistoryController.ClearUndoStates();
                HistoryController.ClearRedoStates();
            }

            Redraw();
        }
    }
}