using System;
using System.Windows.Forms;

namespace GraphicsEditor
{
    public partial class CanvasResizeDialog : Form
    {
        public int Width;
        public int Height;

        public CanvasResizeDialog(int width, int height)
        {
            InitializeComponent();
            Width = width;
            Height = height;
            widthTextBox.Text = width.ToString();
            heightTextBox.Text = height.ToString();
        }

        private void widthTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back) e.Handled = true;
        }

        private void widthTextBox_TextChanged(object sender, EventArgs e)
        {
            var value = getValueAndLimit((TextBox)sender, 5000);
            Width = value == 0 ? 1 : value;
        }

        private void widthTextBox_Leave(object sender, EventArgs e)
        {
            var textBox = (TextBox)sender;
            if (textBox.Text == "")
                textBox.Text = "1";
        }

        private int getValueAndLimit(TextBox textBox, int maxValue)
        {
            int value;

            if (int.TryParse(textBox.Text, out value))
            {
                if (value > maxValue)
                {
                    value = maxValue;
                    textBox.Text = value.ToString();
                    textBox.Select(textBox.Text.Length, 0);
                }
                else if (value == 0)
                {
                    value = 1;
                    textBox.Text = value.ToString();
                    textBox.Select(textBox.Text.Length, 0);
                }
            }

            return value;
        }

        private void heightTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back) e.Handled = true;
        }

        private void heightTextBox_TextChanged(object sender, EventArgs e)
        {
            var value = getValueAndLimit((TextBox)sender, 5000);
            Height = value == 0 ? 1 : value;
        }

        private void heightTextBox_Leave(object sender, EventArgs e)
        {
            var textBox = (TextBox)sender;
            if (textBox.Text == "")
                textBox.Text = "1";
        }

        private void confirmButton_Click(object sender, EventArgs e)
        {
            if (ConfirmDialog())
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private bool ConfirmDialog()
        {
            var dialogResult = MessageBox.Show("Вы уверены? Эту операцию нельзя отменить", "Подтверждение",
                MessageBoxButtons.YesNo);
            return dialogResult == DialogResult.Yes;
        }
    }
}