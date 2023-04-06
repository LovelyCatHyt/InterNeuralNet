using InterNeuralNet.CoreWrapper;
using System;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

namespace InterNeuralNet.NetworkView
{

    public class MatView
    {
        public Mat_Shape Shape { get; protected set; }
        public Texture2D[] textures;
        public Mat_Float[] Mats { get; protected set; }
        public Mat_Float Mat => Mats[0];
        // public int MatCount { get; protected set; }

        public MatView(Mat_Shape shape)
        {
            Shape = shape;
            textures = new Texture2D[shape.count];
            // 创建纹理, 指针直接赋给Mat
            for (int i = 0; i < Shape.count; i++)
            {
                // TODO: 这里很可能会出现: 无法读/无法写/无法渲染之类的问题 还需要深入研究
                Texture2D tex = new Texture2D(shape.width, shape.height, UnityEngine.Experimental.Rendering.GraphicsFormat.R32_SFloat, UnityEngine.Experimental.Rendering.TextureCreationFlags.None);
                tex.filterMode = FilterMode.Point;
                textures[i] = tex;
            }
            // SetMatPtr();
        }

        public MatView(int width, int height, int matCount) : this(new Mat_Shape(width, height, matCount)) { }

        public MatView(int width, int height) : this(width, height, 1) { }

        /// <summary>
        /// 指示 <see cref="MatView"/> 开始写入纹理数据. 该方法调用后可以访问 <see cref="Mats"/> 中的数据.
        /// </summary>
        public void WriteBegin()
        {
            SetMatPtr();
        }

        /// <summary>
        /// 指示 <see cref="MatView"/> 数据已写入完成, 可以渲染. 该方法调用后, <see cref="Mats"/> 中的指针可能失效.
        /// </summary>
        public void WriteEnd()
        {
            Apply();
        }

        /// <summary>
        /// 将 <see cref="Texture2D.GetPixelData"/> 通过 CPU 修改的内容上传到 GPU
        /// </summary>
        protected void Apply()
        {
            for (int i = 0; i < Shape.count; i++) textures[i].Apply(false, false);
        }

        protected virtual void SetMatPtr()
        {
            for (int i = 0; i < Shape.count; i++) Mats[i].SetPtr(textures[i].GetPixelData<float>(0));
        }

        public float ReadAt(int matIndex, int row, int col)
        {
            return textures[matIndex].GetPixel(col, row).r;
        }

        public float ReadAt(int row, int col)
        {
            return ReadAt(0, row, col);
        }

        ~MatView()
        {
            for (int i = 0; i < Shape.count; i++)
            {
                UnityEngine.Object.Destroy(textures[i]);
                Mats[i].ptr = IntPtr.Zero;
            }
        }
    }
}
