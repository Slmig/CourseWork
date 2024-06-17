using System;
using System.Drawing;
using System.Windows.Forms;

namespace GraphicsEditor
{
    public partial class MainForm
    {
        private void openProjectMenuButton_Click(object sender, EventArgs e)
        {
            if (animPlaying) return;
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "Project Files(*.PXP)|*.PXP";

                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                var project = FileManager.LoadProject(dialog.FileName);
                HistoryController.ClearRedoStates();
                HistoryController.ClearUndoStates();
                framesController = project.FramesController;
                paletteColors = project.Palette;
                framesGrid.Rows.Clear();
                layersGrid.Rows.Clear();
                for (var i = 0; i < framesController.Frames.Count; i++)
                    AddFrame(false);
                for (var i = 0; i < framesController.CurrentFrame.Layers.Count; i++)
                    AddLayer(false);

                for (var i = 0; i < paletteButtons.Length; i++)
                {
                    var button = (Button)paletteButtons[i].GetValue(this);
                    button.BackColor = paletteColors[i];
                }

                Redraw();
                toolsController.Prepare(framesController.CurrentFrame, display);
            }
        }

        private void saveProjectMenuButton_Click(object sender, EventArgs e)
        {
            if (animPlaying) return;
            FileManager.SaveProject(new Project(framesController, paletteColors));
        }

        private void importImageMenuButton_Click(object sender, EventArgs e)
        {
            if (animPlaying) return;
            var dialog = new OpenFileDialog();
            dialog.Filter = "Image Files(*.PNG;*.JPG;*.JPEG;*.BMP)|*.PNG;*.JPG;*.JPEG;*.BMP";

            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            toolsController.Tool?.Apply();
            var img = FileManager.ImportImage(dialog.FileName);
            var buf = toolsController.Brush;
            toolsController = new ToolsController
            {
                SelectedTool = ToolsController.ToolType.SelectionRect,
                Brush = buf
            };
            toolsController.Prepare(framesController.CurrentFrame, display, img);
        }

        private void atlasExportMenuButton_Click(object sender, EventArgs e)
        {
            if (animPlaying) return;
            var bitmap = new Bitmap(framesController.Frames.Count * framesController.CurrentFrame.Width,
                framesController.CurrentFrame.Height);

            using (var graphics = Graphics.FromImage(bitmap))
            {
                for (var i = 0; i < framesController.Frames.Count; i++)
                    graphics.DrawImage(
                        framesController.Frames[i].MergeLayers(0, framesController.Frames[i].Layers.Count - 1, false),
                        i * framesController.CurrentFrame.Width, 0,
                        new Rectangle(0, 0, framesController.Frames[i].Width, framesController.Frames[i].Height),
                        GraphicsUnit.Pixel);
            }

            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "PNG Files(*.PNG)|*.PNG";
                dialog.DefaultExt = "png";

                if (dialog.ShowDialog() == DialogResult.OK)
                    FileManager.ExportImage(dialog.FileName, bitmap);
            }
        }

        private void singleExportMenuButton_Click(object sender, EventArgs e)
        {
            if (animPlaying) return;
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                for (var i = 0; i < framesController.Frames.Count; i++)
                    FileManager.ExportImage($"{dialog.SelectedPath}\\frame{i}.png",
                        framesController.Frames[i].MergeLayers(0, framesController.Frames[i].Layers.Count - 1, false));
            }
        }
    }
}