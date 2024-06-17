using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace GraphicsEditor
{
    public static class FileManager
    {
        public static Bitmap ImportImage(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                return new Bitmap(Image.FromStream(fs));
            }
        }

        public static void ExportImage(string path, Bitmap bitmap)
        {
            using (var stream = new FileStream(path, FileMode.Create))
            {
                bitmap.Save(stream, ImageFormat.Png);
            }
        }

        public static Project LoadProject(string path)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                return (Project)formatter.Deserialize(stream);
            }
        }

        public static void SaveProject(Project project)
        {
            var formatter = new BinaryFormatter();

            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "Project Files(*.PXP)|*.PXP";
                dialog.DefaultExt = "pxp";

                if (dialog.ShowDialog() == DialogResult.OK)
                    using (var stream = new FileStream(dialog.FileName, FileMode.Create))
                    {
                        formatter.Serialize(stream, project);
                    }
            }
        }
    }
}