using System;
using System.Collections.Generic;
using System.Drawing;

namespace GraphicsEditor
{
    [Serializable]
    public class Canvas : ICloneable
    {
        [NonSerialized]
        public Bitmap Image;
        [NonSerialized]
        public Bitmap Background;
        [NonSerialized]
        public Bitmap Foreground;
        public Layer ActiveLayer => Layers[ActiveLayerIndex];
        public int ActiveLayerIndex
        {
            get => activeLayerIndex;
            set
            {
                if (value >= 0 && value < Layers.Count)
                    activeLayerIndex = value;
            }
        }
        public int Width => Image.Width;
        public int Height => Image.Height;
        public readonly List<Layer> Layers = new List<Layer>();
        private Bitmap tilemap;
        private int activeLayerIndex;

        public Canvas() { }

        public Canvas(int width, int height)
        {
            Layers.Add(new Layer(new Bitmap(width, height)));
            initialize();
        }

        public void Refresh()
        {
            Image?.Dispose();
            Image = MergeLayers(0, Layers.Count - 1, true);
            Background = MergeLayers(0, ActiveLayerIndex - 1, true);
            Foreground = MergeLayers(ActiveLayerIndex + 1, Layers.Count - 1, false);
        }

        public void SelectLayer(int index)
        {
            ActiveLayerIndex = index;
            Refresh();
        }

        public void AddLayer()
        {
            Layers.Add(new Layer(new Bitmap(Width, Height)));
            SelectLayer(Layers.Count - 1);
        }

        public void SelectLayer(Layer layer)
        {
            SelectLayer(Layers.FindIndex(l => l == layer));
        }

        public void RemoveLayer(int index)
        {
            Layers.RemoveAt(index);
            if (Layers.Count == ActiveLayerIndex)
                ActiveLayerIndex = Layers.Count - 1;
            Refresh();
        }

        public void SwapLayers(int index1, int index2)
        {
            (Layers[index1], Layers[index2]) = (Layers[index2], Layers[index1]);
            Refresh();
        }

        public bool Contains(Layer layer) =>
            Layers.Contains(layer);

        public void Resize(int width, int height)
        {
            foreach (var layer in Layers)
                layer.Image = layer.Image.Resize(width, height);

            Refresh();
        }

        public void TurnRight()
        {
            foreach (var layer in Layers)
            {
                layer.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                layer.Image = (Bitmap)layer.Image.Clone();
            }

            Refresh();
        }

        public void TurnLeft()
        {
            foreach (var layer in Layers)
            {
                layer.Image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                layer.Image = (Bitmap)layer.Image.Clone();
            }

            Refresh();
        }

        private void initialize()
        {
            ActiveLayerIndex = 0;
            generateTilemap();
            Refresh();
        }

        public Bitmap MergeLayers(int index1, int index2, bool needTilemap)
        {
            Bitmap bitmap;

            if (needTilemap)
            {
                if (tilemap.Width != ActiveLayer.Image.Width || tilemap.Height != ActiveLayer.Image.Height)
                    generateTilemap();
                bitmap = (Bitmap)tilemap.Clone();
            }
            else bitmap = new Bitmap(ActiveLayer.Image.Width, ActiveLayer.Image.Height);

            using (var graphics = Graphics.FromImage(bitmap))
            {
                for (var i = index1; i <= index2; i++)
                    if (!Layers[i].Hidden)
                        graphics.DrawImage(Layers[i].Image, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
            }

            return bitmap;
        }

        private void generateTilemap()
        {
            var bmp = new Bitmap(ActiveLayer.Image.Width, ActiveLayer.Image.Height);
            var tileSize = 48;
            var color1 = Color.FromArgb(190, 190, 190);
            var color2 = Color.FromArgb(127, 127, 127);

            using (var g = Graphics.FromImage(bmp))
            {
                g.FillRectangle(new SolidBrush(color1), 0, 0, ActiveLayer.Image.Width, ActiveLayer.Image.Height);
                for (var y = 0; y < ActiveLayer.Image.Height; y += tileSize)
                for (var x = y / tileSize % 2 * tileSize; x < ActiveLayer.Image.Width; x += tileSize * 2)
                    g.FillRectangle(new SolidBrush(color2), x, y, tileSize, tileSize);
            }

            tilemap = bmp;
        }

        public object Clone()
        {
            var clone = new Canvas();
            foreach (var layer in Layers)
                clone.Layers.Add((Layer)layer.Clone());
            clone.initialize();
            return clone;
        }
    }
}