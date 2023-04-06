using System;

namespace InterNeuralNet.CoreWrapper
{
    /// <summary>
    /// 最大池化层
    /// </summary>
    public class MaxPoolingLayer : Layer
    {
        public int size;

        public MaxPoolingLayer(int size, int channel = 1)
        {
            this.size = size;
            inChannel = outChannel = channel;
        }

        public override Mat_Shape CalcOutputShape(Mat_Shape shape)
        {
            return new Mat_Shape(shape.width / size, shape.height / size, outChannel);
        }

        public override void Eval(Mat_Float[] input, Mat_Float[] output)
        {
            var maxChannel = Math.Min(inChannel, outChannel);
            for (int i = 0; i < maxChannel; i++)
            {
                MatrixFunc.MaxPooling(ref input[i], ref output[i], size);
            }
        }

        public override Mat_Shape[] GetParamShapes()
        {
            return new Mat_Shape[0];
        }
    }
}
