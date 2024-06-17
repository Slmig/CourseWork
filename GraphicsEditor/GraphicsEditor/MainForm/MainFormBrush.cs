using System;
using System.Windows.Forms;

namespace GraphicsEditor
{
    public partial class MainForm
    {
        private void brushSizeTrackBar_ValueChanged(object sender, EventArgs e)
        {
            toolsController.Brush.Size = brushSizeTrackBar.Value;
            brushSizeTextBox.Text = brushSizeTrackBar.Value.ToString();
        }

        private void brushSizeTextBox_TextChanged(object sender, EventArgs e)
        {
            var value = getValueAndLimit((TextBox)sender, 100);
            brushSizeTrackBar.Value = value == 0 ? 1 : value;
        }

        private void brushSizeTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back) e.Handled = true;
        }

        private void brushSizeTextBox_Leave(object sender, EventArgs e)
        {
            if (brushSizeTextBox.Text == "")
            {
                brushSizeTextBox.Text = "1";
                brushSizeTrackBar.Value = 1;
            }
        }

        private void cirleBrushButton_Click(object sender, EventArgs e)
        {
            toolsController.Brush.Shape = Brush.BrushShape.Circle;
        }

        private void squareBrushButton_Click(object sender, EventArgs e)
        {
            toolsController.Brush.Shape = Brush.BrushShape.Square;
        }
    }
}