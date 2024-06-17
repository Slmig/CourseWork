namespace GraphicsEditor
{
    partial class CanvasResizeDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.widthTextBox = new System.Windows.Forms.TextBox();
            this.heightTextBox = new System.Windows.Forms.TextBox();
            this.confirmButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.widthLabel = new System.Windows.Forms.Label();
            this.heightLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // widthTextBox
            // 
            this.widthTextBox.Location = new System.Drawing.Point(120, 27);
            this.widthTextBox.Name = "widthTextBox";
            this.widthTextBox.Size = new System.Drawing.Size(100, 20);
            this.widthTextBox.TabIndex = 0;
            this.widthTextBox.TextChanged += new System.EventHandler(this.widthTextBox_TextChanged);
            this.widthTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.widthTextBox_KeyPress);
            this.widthTextBox.Leave += new System.EventHandler(this.widthTextBox_Leave);
            // 
            // heightTextBox
            // 
            this.heightTextBox.Location = new System.Drawing.Point(120, 66);
            this.heightTextBox.Name = "heightTextBox";
            this.heightTextBox.Size = new System.Drawing.Size(100, 20);
            this.heightTextBox.TabIndex = 1;
            this.heightTextBox.TextChanged += new System.EventHandler(this.heightTextBox_TextChanged);
            this.heightTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.heightTextBox_KeyPress);
            this.heightTextBox.Leave += new System.EventHandler(this.heightTextBox_Leave);
            // 
            // confirmButton
            // 
            this.confirmButton.Location = new System.Drawing.Point(71, 107);
            this.confirmButton.Name = "confirmButton";
            this.confirmButton.Size = new System.Drawing.Size(70, 23);
            this.confirmButton.TabIndex = 2;
            this.confirmButton.Text = "Ок";
            this.confirmButton.UseVisualStyleBackColor = true;
            this.confirmButton.Click += new System.EventHandler(this.confirmButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(150, 107);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(70, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Отмена";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // widthLabel
            // 
            this.widthLabel.AutoSize = true;
            this.widthLabel.Location = new System.Drawing.Point(68, 30);
            this.widthLabel.Name = "widthLabel";
            this.widthLabel.Size = new System.Drawing.Size(46, 13);
            this.widthLabel.TabIndex = 4;
            this.widthLabel.Text = "Ширина";
            // 
            // heightLabel
            // 
            this.heightLabel.AutoSize = true;
            this.heightLabel.Location = new System.Drawing.Point(68, 69);
            this.heightLabel.Name = "heightLabel";
            this.heightLabel.Size = new System.Drawing.Size(45, 13);
            this.heightLabel.TabIndex = 5;
            this.heightLabel.Text = "Высота";
            // 
            // CanvasResizeDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(289, 156);
            this.Controls.Add(this.heightLabel);
            this.Controls.Add(this.widthLabel);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.confirmButton);
            this.Controls.Add(this.heightTextBox);
            this.Controls.Add(this.widthTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "CanvasResizeDialog";
            this.Text = "Изменение размера";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox widthTextBox;
        private System.Windows.Forms.TextBox heightTextBox;
        private System.Windows.Forms.Button confirmButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label widthLabel;
        private System.Windows.Forms.Label heightLabel;
    }
}