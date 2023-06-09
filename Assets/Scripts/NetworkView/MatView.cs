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
        /// <summary>
        /// 是否存在未更新的数据
        /// </summary>
        public bool IsDirty { get; protected set; } = false;
        /// <summary>
        /// 当前矩阵中的指针是否可用
        /// </summary>
        public bool IsPtrAvailable { get; protected set; } = false;

        public MatView(Mat_Shape shape)
        {
            Shape = shape;
            textures = new Texture2D[shape.count];
            Mats = new Mat_Float[shape.count];
            // 创建纹理, 指针直接赋给Mat
            for (int i = 0; i < Shape.count; i++)
            {
                Texture2D tex = new Texture2D(shape.width, shape.height, UnityEngine.Experimental.Rendering.GraphicsFormat.R32_SFloat, UnityEngine.Experimental.Rendering.TextureCreationFlags.None);
                tex.filterMode = FilterMode.Point;
                textures[i] = tex;
                Mats[i].width = shape.width;
                Mats[i].height = shape.height;
            }
            // SetMatPtr();
        }

        public MatView(int width, int height, int matCount) : this(new Mat_Shape(width, height, matCount)) { }

        public MatView(int width, int height) : this(width, height, 1) { }

        /// <summary>
        /// 指示 <see cref="MatView"/> 开始写入纹理数据. 该方法调用后可以访问 <see cref="Mats"/> 中的数据.
        /// </summary>
        public void EnableAccess()
        {
            if (!IsPtrAvailable) SetMatPtr();
            IsDirty = true;
            IsPtrAvailable = true;
        }

        /// <summary>
        /// 指示 <see cref="MatView"/> 数据已写入完成, 可以渲染. 该方法调用后, <see cref="Mats"/> 中的指针可能失效.
        /// <para>多次连续调用仅触发一次纹理更新.</para>
        /// </summary>
        public void UpdateTexture()
        {
            if (IsDirty) Apply();
            IsDirty = false;
            IsPtrAvailable = false;
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

        public override string ToString() => Shape.ToString();

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
