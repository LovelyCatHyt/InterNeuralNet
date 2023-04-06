using InterNeuralNet.CoreWrapper;
using UnityEngine;

namespace InterNeuralNet.NetworkView
{
    public class MatWritableView : MatView
    {
        public MatWritableView(Mat_Shape shape) : base(shape) { }

        public MatWritableView(int width, int height, int matCount) : this(new Mat_Shape(width, height, matCount)) { }

        public MatWritableView(int width, int height) : this(width, height, 1) { }

        public void WriteAt(int matIndex, int row, int col, float value)
        {
            // TODO
        }

        public void WriteAt(int row, int col, float value)
        {
            WriteAt(0, row, col, value);
        }

        public void BindTo(Layer layer)
        {
            // TODO
        }
    }
}
