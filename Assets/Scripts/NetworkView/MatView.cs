using InterNeuralNet.CoreWrapper;
using System;
using Unity.VisualScripting;
using UnityEngine;

namespace InterNeuralNet.NetworkView
{

    public class MatView
    {
        public Mat_Shape Shape { get; protected set; }

        public RenderTexture[] textures;
        public Mat_Float[] Mats { get; protected set; }
        public Mat_Float Mat => Mats[0];
        // public int MatCount { get; protected set; }

        public MatView(Mat_Shape shape)
        {
            Shape = shape;
            // 创建纹理, 指针直接赋给Mat
            for (int i = 0; i < Shape.count; i++)
            {
                // TODO: 这里很可能会出现: 无法读/无法写/无法渲染之类的问题 还需要深入研究
                textures[i] = new RenderTexture(Shape.width, Shape.height, 1, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);
                textures[i].enableRandomWrite = true;
                textures[i].Create();
                Mats[i].ptr = textures[i].GetNativeTexturePtr();
            }
        }

        public MatView(int width, int height, int matCount) : this(new Mat_Shape(width, height, matCount)) { }

        public MatView(int width, int height) : this(width, height, 1) { }

        public float ReadAt(int matIndex, int row, int col)
        {
            // 不知道咋整
            return -1;
        }

        public float ReadAt(int row, int col)
        {
            return ReadAt(0, row, col);
        }

        ~MatView()
        {
            for (int i = 0; i < Shape.count; i++)
            {
                textures[i].Release();
                Mats[i].ptr = IntPtr.Zero;
            }
        }
    }
}
