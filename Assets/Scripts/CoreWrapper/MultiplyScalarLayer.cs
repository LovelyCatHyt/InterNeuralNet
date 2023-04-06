namespace InterNeuralNet.CoreWrapper
{
    public class MultiplyScalarLayer : Layer
    {
        public float scalar;

        public MultiplyScalarLayer(float scalar)
        {
            this.scalar = scalar;
        }

        public MultiplyScalarLayer(float scalar, int channel)
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
            for (int i = 0; i < input.Length; i++)
            {
                MatrixFunc.MultiplyScalar(ref input[i], scalar, ref output[i]);
            }
        }

        public override Mat_Shape[] GetParamShapes()
        {
            return new Mat_Shape[0];
        }
    }
}
