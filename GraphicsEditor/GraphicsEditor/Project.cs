using System;
using System.Drawing;

namespace GraphicsEditor
{
    [Serializable]
    public class Project
    {
        public FramesController FramesController;
        public Color[] Palette;

        public Project(FramesController framesController, Color[] palette)
        {
            FramesController = framesController;
            Palette = palette;
        }
    }
}