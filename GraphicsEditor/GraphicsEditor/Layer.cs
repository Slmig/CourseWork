using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization;

namespace GraphicsEditor
{
    [Serializable]
    public class Layer : ISerializable, ICloneable
    {
        public Layer(Bitmap image)
        {
            Image = image;
        }

        [NonSerialized]
        public Bitmap Image;
        [NonSerialized]
        public bool Hidden;

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Hidden", Hidden);

            using (var stream = new MemoryStream())
            {
                Image.Save(stream, ImageFormat.Png);
                var bitmapBytes = stream.ToArray();
                info.AddValue("Image", bitmapBytes);
            }
        }

        protected Layer(SerializationInfo info, StreamingContext context)
        {
            Hidden = (bool)info.GetValue("Hidden", typeof(bool));
            var bitmapBytes = (byte[])info.GetValue("Image", typeof(byte[]));
            var converter = new ImageConverter();
            var image = (Image)converter.ConvertFrom(bitmapBytes);
            Image = new Bitmap(image);
        }

        public object Clone() => new Layer((Bitmap)Image.Clone()) { Hidden = Hidden };
    }
}