namespace InterNeuralNet.CoreWrapper
{
    public class FullConnectionLayer : Layer
    {
        public int inNode;
        public int outNode;
        public Mat_Float weight;
        public Mat_Float bias;

        public FullConnectionLayer(int inNode, int outNode)
        {
            this.inNode = inNode;
            this.outNode = outNode;
            weight.width = inNode;
            bias.height = weight.height = outNode;
            bias.width = 1;
        }

        public FullConnectionLayer(Mat_Float weight, Mat_Float bias)
        {
            this.weight = weight;
            this.bias = bias;
            inChannel = outChannel = 1;
            inNode = weight.width;
            outNode = weight.height;
        }

        public override void ApplyParams(Mat_Float[] parameters)
        {
            weight = parameters[0];
            bias = parameters[1];
            // UnityEngine.Debug.Log($"{name}: weight: {weight.height}*{weight.width}, bias: {bias.height}*{bias.width}");
        }

        public override Mat_Shape CalcOutputShape(Mat_Shape shape)
        {
            return new Mat_Shape(1, outNode, 1);
        }

        public override void Eval(Mat_Float[] input, Mat_Float[] output)
        {
            if (input.Length > 1)
            {
                MatrixFunc.FullConnectionArr(input, ref weight, ref bias, ref output[0]);
            }
            else
            {
                MatrixFunc.FullConnection(ref input[0], ref weight, ref bias, ref output[0]);
            }
        }

        public override Mat_Shape[] GetParamShapes()
        {
            return new[] { new Mat_Shape(weight.width, weight.height, 1), new Mat_Shape(bias.width, bias.height, 1) };
        }
    }
}
