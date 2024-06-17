using System;

namespace GraphicsEditor
{
    public partial class MainForm
    {
        public void ScaleUp()
        {
            if (scale < 100) scale += 10;
            else if (scale < 300) scale += 200;
            Rescale();
        }

        public void ScaleDown()
        {
            if (scale > 100) scale -= 100;
            else if (scale > 30) scale -= 10;
            Rescale();
        }

        public void Rescale()
        {
            display?.Dispose();
            var width = (int)(framesController.CurrentFrame.Width * (float)scale / 100);
            var height = (int)(framesController.CurrentFrame.Height * (float)scale / 100);
            var image = framesController.CurrentFrame.Image.NearestNeighbour(width, height);
            display = image;
            pictureBox.Image = display;
            pictureBox.Width = display.Width;
            pictureBox.Height = display.Height;
            toolsController.Prepare(framesController.CurrentFrame, display);
        }

        private void upScaleButton_Click(object sender, EventArgs e) => ScaleUp();
        private void downScaleButton_Click(object sender, EventArgs e) => ScaleDown();
    }
}