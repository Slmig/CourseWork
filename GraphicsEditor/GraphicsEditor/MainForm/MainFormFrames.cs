using System;
using System.Windows.Forms;
using GraphicsEditor.Properties;

namespace GraphicsEditor
{
    public partial class MainForm
    {
        private void AddFrame(bool flag = true)
        {
            if (flag) framesController.AddFrame();
            framesGrid.Rows.Add($"Кадр{framesGrid.Rows.Count + 1}", Resources.arrow_down, Resources.arrow_up, Resources.krest);
            AdjustDataGridViewWidth(framesGrid, flag);
            framesController.CurrentFrameIndex = framesGrid.Rows.Count - 1;
            SelectRow(framesGrid, framesGrid.Rows.Count - 1);
            Redraw();
        }

        private void framesGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (animPlaying || e.RowIndex < 0) return;

            if (e.RowIndex != framesController.CurrentFrameIndex)
                framesController.CurrentFrameIndex = e.RowIndex;

            switch (e.ColumnIndex)
            {
                case 1:
                    if (e.RowIndex != framesGrid.Rows.Count - 1)
                    {
                        framesController.SwapFrames(e.RowIndex, e.RowIndex + 1);
                        SwapRows(framesGrid, e.RowIndex, e.RowIndex + 1);
                    }

                    break;
                case 2:
                    if (e.RowIndex != 0)
                    {
                        framesController.SwapFrames(e.RowIndex, e.RowIndex - 1);
                        SwapRows(framesGrid, e.RowIndex, e.RowIndex - 1);
                    }

                    break;
                case 3:
                    if (framesGrid.Rows.Count > 1)
                        if (ConfirmDialog())
                        {
                            foreach (var layer in framesController.CurrentFrame.Layers)
                                HistoryController.RemoveLayerStates(layer);

                            framesController.RemoveFrame(e.RowIndex);
                            framesGrid.Rows.RemoveAt(e.RowIndex);
                        }

                    break;
            }

            Redraw();
        }

        private void addFrameButton_Click(object sender, EventArgs e)
        {
            AddFrame();
        }
    }
}