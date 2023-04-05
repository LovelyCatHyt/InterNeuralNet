namespace InterNeuralNet.CoreWrapper
{
    /// <summary>
    /// 全连接层
    /// </summary>
    public class ConvolutionLayer : Layer
    {
        public Mat_Float[] kernels;
        public Mat_Float bias;
        public int padding;

        public ConvolutionLayer(int inChannel, int outChannel, Mat_Float[] kernels, Mat_Float bias, int padding)
        {
            this.inChannel = inChannel;
            this.outChannel = outChannel;
            this.kernels = kernels;
            this.padding = padding;
        }

        public override Mat_Shape CalcOutputShape(Mat_Shape shape)
        {
            return new Mat_Shape(shape.width - 2 * padding, shape.height - 2 * padding, outChannel);
        }

        public override void Eval(Mat_Float[] input, Mat_Float[] output)
        {
            MatrixFunc.BatchConvolutionLayer(input, kernels, bias, inChannel, outChannel, output, padding);
        }

        public override Mat_Shape[] GetParamShapes()
        {
            return new[] { new Mat_Shape(kernels[0].width, kernels[0].height, kernels.Length), new Mat_Shape(bias.width, bias.height, 1) };
        }
    }
}
