using System;
using System.Drawing;
using System.Windows.Forms;

namespace GraphicsEditor
{
    public partial class MainForm
    {
        private void paletteInitialize()
        {
            for (var i = 0; i < paletteButtons.Length; i++)
            {
                var button = (Button)paletteButtons[i].GetValue(this);
                button.BackColor = paletteColors[i];
                button.Click += (sender, args) =>
                {
                    var btn = (Button)sender;
                    var index = int.Parse(btn.Name.Substring(11)) - 1;
                    paletteIndex = index;
                    toolsController.Brush.Color = paletteColors[index];

                    foreach (var paletteButton in paletteButtons)
                        ((Button)paletteButton.GetValue(this)).Invalidate();
                };
                button.Paint += (sender, args) =>
                {
                    var btn = (Button)sender;
                    var index = int.Parse(btn.Name.Substring(11)) - 1;

                    if (index == paletteIndex)
                    {
                        var pen1 = new Pen(Color.Black, 3);
                        var pen2 = new Pen(Color.White, 1);
                        var borderRect1 = new Rectangle(0, 0, btn.ClientRectangle.Width - 1, btn.ClientRectangle.Height - 1);
                        var borderRect2 = new Rectangle(2, 2, btn.ClientRectangle.Width - 6, btn.ClientRectangle.Height - 6);
                        args.Graphics.DrawRectangle(pen1, borderRect1);
                        args.Graphics.DrawRectangle(pen2, borderRect2);
                        pen1.Dispose();
                        pen2.Dispose();
                    }
                };
            }
        }

        private void colorPickButton_Click(object sender, EventArgs e)
        {
            using (var colorDialog = new ColorDialog())
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                    updatePaletteColor(colorDialog.Color);
            }
        }

        private void updatePaletteColor(Color color)
        {
            paletteColors[paletteIndex] = color;
            ((Button)paletteButtons[paletteIndex].GetValue(this)).BackColor = color;
            toolsController.Brush.Color = color;
        }
    }
}