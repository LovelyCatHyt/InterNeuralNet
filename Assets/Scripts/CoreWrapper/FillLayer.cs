namespace InterNeuralNet.CoreWrapper
{
    public class FillLayer : Layer
    {
        public float scalar;

        public FillLayer(float scalar)
        {
            this.scalar = scalar;
        }

        public FillLayer(float scalar, int channel)
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
            for (int i = 0; i < output.Length; i++)
            {
                MatrixFunc.Fill(ref output[i], scalar);
            }
        }

        public override Mat_Shape[] GetParamShapes()
        {
            return new Mat_Shape[0];
        }
    }
}
