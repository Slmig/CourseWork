using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GraphicsEditor
{
    [Serializable]
    public class FramesController : ISerializable
    {
        public Canvas CurrentFrame => Frames[CurrentFrameIndex];
        public int CurrentFrameIndex
        {
            get => currentFrameIndex;
            set
            {
                if (value >= 0 && value < Frames.Count)
                    currentFrameIndex = value;
            }
        }
        public List<Canvas> Frames = new List<Canvas>();
        private int currentFrameIndex;

        public FramesController(int width, int height)
        {
            Frames.Add(new Canvas(width, height));
        }

        public void AddFrame()
        {
            Frames.Add((Canvas)Frames[Frames.Count - 1].Clone());
            Frames[Frames.Count - 1].SelectLayer(CurrentFrame.ActiveLayerIndex);
        }

        public void RemoveFrame(int index)
        {
            Frames.RemoveAt(index);
            if (index >= Frames.Count)
                currentFrameIndex = Frames.Count - 1;
        }

        public void SwapFrames(int index1, int index2)
        {
            (Frames[index1], Frames[index2]) = (Frames[index2], Frames[index1]);
        }

        public void SetPointer(Layer layer)
        {
            for (var i = 0; i < Frames.Count; i++)
                if (Frames[i].Contains(layer))
                {
                    Frames[i].SelectLayer(layer);

                    foreach (var frame in Frames)
                        frame.SelectLayer(Frames[i].ActiveLayerIndex);

                    CurrentFrameIndex = i;
                    return;
                }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Frames", Frames);
        }

        protected FramesController(SerializationInfo info, StreamingContext context)
        {
            Frames = (List<Canvas>)info.GetValue("Frames", typeof(List<Canvas>));
        }
    }
}