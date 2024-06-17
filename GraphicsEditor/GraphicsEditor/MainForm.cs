using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using GraphicsEditor.Properties;

namespace GraphicsEditor
{
    public partial class MainForm : Form
    {
        public static event Action PictureBoxRefresh;
        public static event Action PictureBoxRedraw;
        private ToolsController toolsController = new ToolsController();
        private FramesController framesController;
        public static Cursor pencilCursor;
        public static Cursor eraserCursor;
        public static Cursor pipetCursor;
        public static Cursor rectCursor;
        private Bitmap display;
        private bool mousePressed;
        private int scale = 100;
        private int animSpeed = 1;
        private Thread animThread;
        private bool animPlaying;
        private Color[] paletteColors =
        {
            Color.Black, Color.Gray, Color.SaddleBrown, Color.Blue, Color.Green, Color.Red, Color.OrangeRed, Color.White,
            Color.Aqua,
            Color.DarkGreen, Color.BlueViolet, Color.Chartreuse, Color.DarkBlue, Color.DarkGoldenrod, Color.DarkMagenta,
            Color.Yellow,
            Color.Plum, Color.BurlyWood, Color.Teal, Color.DarkKhaki, Color.RoyalBlue, Color.CadetBlue, Color.Azure,
            Color.DarkSeaGreen,
            Color.DarkSlateGray
        };
        private readonly FieldInfo[] paletteButtons;
        private int paletteIndex;

        public MainForm()
        {
            InitializeComponent();
            pencilCursor = GetCursor(Resources.Pencil);
            eraserCursor = GetCursor(Resources.Eraser);
            pipetCursor = GetCursor(Resources.Pipet);
            rectCursor = GetCursor(Resources.rectCur);
            PictureBoxRefresh += () => { pictureBox.Refresh(); };
            PictureBoxRedraw += () => { Redraw(); };
            paletteButtons = GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                                      .Where(f => f.FieldType == typeof(Button) && f.Name.StartsWith("colorButton")).ToArray();
            Array.Sort(paletteButtons,
                (f1, f2) => int.Parse(f1.Name.Substring(11)).CompareTo(int.Parse(f2.Name.Substring(11))));
            toolsController.Brush.Color = paletteColors[0];
            brushSizeTrackBar.Value = 16;
            animSpeedTextBox.Text = "1";
            framesController = new FramesController(300, 300);
            splitContainer2.Panel1.MouseWheel += (sender, e) => { toolsController.Tool?.HandleScroll(); };

            layersGrid.AllowUserToResizeRows = false;
            layersGrid.AllowUserToOrderColumns = false;
            layersGrid.RowsDefaultCellStyle.SelectionBackColor = Color.Gray;
            framesGrid.AllowUserToResizeRows = false;
            framesGrid.AllowUserToOrderColumns = false;
            framesGrid.RowsDefaultCellStyle.SelectionBackColor = Color.Gray;

            paletteInitialize();
            Rescale();
            toolsController.Prepare(framesController.CurrentFrame, display);
            AddLayer(false);
            AddFrame(false);
        }

        public void ChangeCursor()
        {
            switch (toolsController.SelectedTool)
            {
                case ToolsController.ToolType.Pencil:
                    Cursor = pencilCursor;
                    break;
                case ToolsController.ToolType.Eraser:
                    Cursor = eraserCursor;
                    break;
                case ToolsController.ToolType.Pipet:
                    Cursor = pipetCursor;
                    break;
                case ToolsController.ToolType.SelectionRect:
                    Cursor = rectCursor;
                    break;
            }
        }

        private void splitContainer2_Panel1_Scroll(object sender, ScrollEventArgs e)
        {
            toolsController.Tool?.HandleScroll();
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

        public static void ImageRefresh() => PictureBoxRefresh.Invoke();
        public static void ImageRedraw() => PictureBoxRedraw.Invoke();

        private Cursor GetCursor(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            {
                return new Cursor(stream);
            }
        }

        private bool ConfirmDialog()
        {
            var dialogResult = MessageBox.Show("Вы уверены? Эту операцию нельзя отменить", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            return dialogResult == DialogResult.Yes;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                var result = MessageBox.Show("Вы уверены? Несохранённые данные могут быть утеряны", "Подтверждение",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                    e.Cancel = true;
                animThread?.Abort();
            }
        }
    }
}