namespace InterNeuralNet.CoreWrapper
{
    public class AddScalarLayer : Layer
    {
        public float scalar;

        public AddScalarLayer(float scalar)
        {
            this.scalar = scalar;
        }

        public AddScalarLayer(float scalar, int channel)
        {
            this.scalar = scalar;
            inChannel = outChannel = channel;
        }

        public override Mat_Shape CalcOutputShape(Mat_Shape shape)
        {
            return shape;
        }

        public override void Eval(Mat_Float[] input, Mat_Float[] output)
        {
            for (int i = 0; i < inChannel; i++)
            {
                MatrixFunc.AddScalar(ref input[i], scalar, ref output[i]);
            }
        }

        public override Mat_Shape[] GetParamShapes()
        {
            return new Mat_Shape[0];
        }
    }
}
