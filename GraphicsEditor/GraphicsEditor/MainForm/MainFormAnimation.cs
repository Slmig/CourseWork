using System;
using System.Threading;
using System.Windows.Forms;

namespace GraphicsEditor
{
    public partial class MainForm
    {
        private void startAnimButton_Click(object sender, EventArgs e)
        {
            if (animPlaying || framesController.Frames.Count < 2) return;

            animPlaying = true;
            var delay = (int)(1000 / (float)animSpeed);
            animThread = new Thread(() =>
            {
                while (true)
                    for (var i = 0; i < framesGrid.Rows.Count; i++)
                    {
                        framesController.CurrentFrameIndex = i;
                        Invoke((MethodInvoker)delegate { Redraw(); });
                        Thread.Sleep(delay);
                    }
            });
            animThread.Start();
        }

        private void animSpeedTextBox_TextChanged(object sender, EventArgs e)
        {
            var value = getValueAndLimit((TextBox)sender, 60);
            animSpeed = value == 0 ? 1 : value;
        }

        private void animSpeedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back) e.Handled = true;
        }

        private void animSpeedTextBox_Leave(object sender, EventArgs e)
        {
            if (animSpeedTextBox.Text == "")
                animSpeedTextBox.Text = "1";
        }

        private void stopAnimButton_Click(object sender, EventArgs e)
        {
            animThread?.Abort();
            animPlaying = false;
            framesController.CurrentFrameIndex = framesGrid.SelectedRows[0].Index;
            foreach (var frame in framesController.Frames)
                frame.SelectLayer(layersGrid.SelectedRows[0].Index);
            Redraw();
        }
    }
}