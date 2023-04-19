using System.Linq;

namespace InterNeuralNet.CoreWrapper
{
    /// <summary>
    /// 全连接层
    /// </summary>
    public class ConvolutionLayer : Layer
    {
        public Mat_Float[] kernels;
        public Mat_Float bias;
        // public float bias_scalar;
        public int padding;
        public int size;

        // 新建层时可能无法分配内存空间. 因此提供无矩阵的构造函数
        public ConvolutionLayer(int inChannel, int outChannel, int size, int padding)
        {
            this.inChannel = inChannel;
            this.outChannel = outChannel;
            this.size = size;
            this.padding = padding;
        }

        public override Mat_Shape CalcOutputShape(Mat_Shape shape)
        {
            // 如果 padding 为0, 则输出会比输入小一圈, 写反了会内存越界
            var delta = padding * 2 - (size / 2) * 2;
            return new Mat_Shape(shape.width + delta, shape.height + delta, outChannel);
        }

        public override void ApplyParams(Mat_Float[] parameters)
        {
            kernels = parameters.SkipLast(1).ToArray();
            bias = parameters.Last();
            // UnityEngine.Debug.Log($"{name}: kernels[{kernels.Length}]: {kernels[0].height}*{kernels[0].width}, bias: {bias.height}*{bias.width}");
        }

        public override void Eval(Mat_Float[] input, Mat_Float[] output)
        {
            //UnityEngine.Debug.Log($"ConvLayer \"{name}\":  Eval({input.Length} " +
            //$"{input[0].height}x{input[0].width} in, " +
            //$"{output.Length} {output[0].height}x{output[0].width} out, " +
            //$"{kernels.Length} {kernels[0].height}x{kernels[1].width} kernels, 1x{bias.height} bias, " +
            //$"{padding} pad)");
            MatrixFunc.BatchConvolutionLayer(input, kernels, ref bias, inChannel, outChannel, output, padding);
        }

        public override Mat_Shape[] GetParamShapes()
        {
            return new[] { new Mat_Shape(size, size, inChannel * outChannel), new Mat_Shape(1, outChannel, 1) };
        }
    }
}
