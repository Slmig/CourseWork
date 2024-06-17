using System;

namespace GraphicsEditor
{
    public partial class MainForm
    {
        private void undoButton_Click(object sender, EventArgs e)
        {
            if (animPlaying) return;
            if (HistoryController.UndoAvailable)
            {
                toolsController.Tool?.Apply();
                var layer = HistoryController.GetNextUndoLayer();
                framesController.SetPointer(layer);
                SelectRow(framesGrid, framesController.CurrentFrameIndex);
                SelectRow(layersGrid, framesController.CurrentFrame.ActiveLayerIndex);
                HistoryController.PushRedoState(framesController.CurrentFrame.ActiveLayer);
                HistoryController.Undo();
                Redraw();
            }
        }

        private void redoButton_Click(object sender, EventArgs e)
        {
            if (animPlaying) return;
            if (HistoryController.RedoAvailable)
            {
                var layer = HistoryController.GetNextRedoLayer();
                framesController.SetPointer(layer);
                SelectRow(framesGrid, framesController.CurrentFrameIndex);
                SelectRow(layersGrid, framesController.CurrentFrame.ActiveLayerIndex);
                HistoryController.PushUndoState(framesController.CurrentFrame.ActiveLayer);
                HistoryController.Redo();
                Redraw();
            }
        }
    }
}