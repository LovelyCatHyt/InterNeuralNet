using InterNeuralNet.CoreWrapper;
using UnityEngine;

namespace InterNeuralNet.NetworkView
{
    public class MatWritableView : MatView
    {
        /// <summary>
        /// 使用输入纹理的R通道创建一个视图
        /// </summary>
        /// <param name="src"></param>
        public MatWritableView(Texture2D src) : this(new Mat_Shape(src.width, src.height, 1))
        {
            textures[0].SetPixels(src.GetPixels());
            textures[0].Apply();
        }

        public MatWritableView(Mat_Shape shape) : base(shape) { }

        public MatWritableView(int width, int height, int matCount) : this(new Mat_Shape(width, height, matCount)) { }

        public MatWritableView(int width, int height) : this(width, height, 1) { }

        public void WriteAt(int matIndex, int row, int col, float value)
        {
            textures[matIndex].SetPixel(col, row, new Color(value, 0, 0));
        }

        public void WriteAt(int row, int col, float value)
        {
            WriteAt(0, row, col, value);
        }
    }
}
