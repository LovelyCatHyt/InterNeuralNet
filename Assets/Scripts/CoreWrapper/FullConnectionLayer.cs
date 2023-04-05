namespace InterNeuralNet.CoreWrapper
{
    public class FullConnectionLayer : Layer
    {
        public Mat_Float weight;
        public Mat_Float bias;

        public FullConnectionLayer(Mat_Float weight, Mat_Float bias)
        {
            this.weight = weight;
            this.bias = bias;
            inChannel = outChannel = 1;
        }

        public override Mat_Shape CalcOutputShape(Mat_Shape shape)
        {
            return new Mat_Shape(1, bias.height, 1);
        }

        public override void Eval(Mat_Float[] input, Mat_Float[] output)
        {
            MatrixFunc.FullConnection(input[0], weight, bias, output[0]);
        }

        public override Mat_Shape[] GetParamShapes()
        {
            return new[] { new Mat_Shape(weight.width, weight.height, 1), new Mat_Shape(bias.width, bias.height, 1) };
        }
    }
}
