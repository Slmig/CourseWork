using System.Collections.Generic;
using System.Drawing;

namespace GraphicsEditor
{
    public static class HistoryController
    {
        private class State
        {
            public readonly Layer Layer;
            public readonly Bitmap Image;

            public State(Layer layer, Bitmap image)
            {
                Layer = layer;
                Image = image;
            }
        }

        public static bool UndoAvailable => undoStates.Count != 0;
        public static bool RedoAvailable => redoStates.Count != 0;
        private static readonly Stack<State> undoStates = new Stack<State>();
        private static readonly Stack<State> redoStates = new Stack<State>();

        public static Layer GetNextUndoLayer()
        {
            var state = undoStates.Pop();
            var layer = state.Layer;
            undoStates.Push(state);
            return layer;
        }

        public static Layer GetNextRedoLayer()
        {
            var state = redoStates.Pop();
            var layer = state.Layer;
            redoStates.Push(state);
            return layer;
        }

        public static void Undo()
        {
            var state = undoStates.Pop();
            state.Layer.Image = state.Image;
        }

        public static void Redo()
        {
            var state = redoStates.Pop();
            state.Layer.Image = state.Image;
        }

        public static void PushUndoState(Layer layer)
        {
            if (undoStates.Count > 20)
            {
                var tempStack = new Stack<State>();
                while (undoStates.Count > 1) tempStack.Push(undoStates.Pop());
                undoStates.Pop();
                while (tempStack.Count > 0) undoStates.Push(tempStack.Pop());
            }

            undoStates.Push(new State(layer, (Bitmap)layer.Image.Clone()));
        }

        public static void PushRedoState(Layer layer)
        {
            redoStates.Push(new State(layer, (Bitmap)layer.Image.Clone()));
        }

        public static void ClearUndoStates()
        {
            undoStates.Clear();
        }

        public static void ClearRedoStates()
        {
            redoStates.Clear();
        }

        public static void RemoveLayerStates(Layer layer)
        {
            RemoveFromStack(undoStates, layer);
            RemoveFromStack(redoStates, layer);
        }

        private static void RemoveFromStack(Stack<State> stack, Layer layer)
        {
            var buf = new Stack<State>();

            while (stack.Count > 0)
            {
                var undoState = stack.Pop();
                if (undoState.Layer == layer)
                    continue;

                buf.Push(undoState);
            }

            while (buf.Count > 0)
                stack.Push(buf.Pop());
        }
    }
}