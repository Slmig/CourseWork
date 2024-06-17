using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GraphicsEditor.Properties;

namespace GraphicsEditor
{
    public partial class MainForm
    {
        private void addLayerButton_Click(object sender, EventArgs e)
        {
            AddLayer();
        }

        private void AddLayer(bool flag = true)
        {
            if (flag)
                foreach (var frame in framesController.Frames)
                    frame.AddLayer();
            layersGrid.Rows.Add($"Слой{layersGrid.Rows.Count + 1}", Resources.visible, Resources.arrow_down, Resources.arrow_up,
                Resources.krest);
            AdjustDataGridViewWidth(layersGrid, flag);
            foreach (var frame in framesController.Frames)
                frame.SelectLayer(layersGrid.Rows.Count - 1);
            SelectRow(layersGrid, layersGrid.Rows.Count - 1);
            Redraw();
        }

        private void AdjustDataGridViewWidth(DataGridView dgv, bool flag)
        {
            var contentWidth = dgv.Columns.Cast<DataGridViewColumn>().Sum(column => column.Width) + 3;
            if (dgv.DisplayedRowCount(false) < dgv.Rows.Count && flag)
                contentWidth += SystemInformation.VerticalScrollBarWidth;
            dgv.ClientSize = new Size(contentWidth, dgv.ClientSize.Height);
        }

        private void SelectRow(DataGridView dgv, int index)
        {
            foreach (DataGridViewRow row in dgv.Rows)
                row.Selected = false;


            dgv.Rows[index].Selected = true;
        }

        private void SwapRows(DataGridView dgv, int index1, int index2)
        {
            for (var columnIndex = 0; columnIndex < dgv.Columns.Count; columnIndex++)
                (dgv.Rows[index1].Cells[columnIndex].Value,
                    dgv.Rows[index2].Cells[columnIndex].Value) = (
                    dgv.Rows[index2].Cells[columnIndex].Value,
                    dgv.Rows[index1].Cells[columnIndex].Value);
        }

        private void layersGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (animPlaying || e.RowIndex<0) return;

            if (e.RowIndex != framesController.CurrentFrame.ActiveLayerIndex)
                foreach (var frame in framesController.Frames)
                    frame.SelectLayer(e.RowIndex);

            switch (e.ColumnIndex)
            {
                case 1:
                    if (framesController.CurrentFrame.Layers[e.RowIndex].Hidden)
                    {
                        foreach (var frame in framesController.Frames)
                            frame.Layers[e.RowIndex].Hidden = false;
                        layersGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = Resources.visible;
                    }
                    else
                    {
                        foreach (var frame in framesController.Frames)
                            frame.Layers[e.RowIndex].Hidden = true;
                        layersGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = Resources.hidden;
                    }

                    break;
                case 2:
                    if (e.RowIndex != layersGrid.Rows.Count - 1)
                    {
                        foreach (var frame in framesController.Frames)
                            frame.SwapLayers(e.RowIndex, e.RowIndex + 1);
                        SwapRows(layersGrid, e.RowIndex, e.RowIndex + 1);
                    }

                    break;
                case 3:
                    if (e.RowIndex != 0)
                    {
                        foreach (var frame in framesController.Frames)
                            frame.SwapLayers(e.RowIndex, e.RowIndex - 1);
                        SwapRows(layersGrid, e.RowIndex, e.RowIndex - 1);
                    }

                    break;
                case 4:
                    if (layersGrid.Rows.Count > 1)
                        if (ConfirmDialog())
                        {
                            HistoryController.RemoveLayerStates(framesController.CurrentFrame.ActiveLayer);
                            foreach (var frame in framesController.Frames)
                                frame.RemoveLayer(e.RowIndex);
                            layersGrid.Rows.RemoveAt(e.RowIndex);
                        }

                    break;
            }

            Redraw();
        }
    }
}