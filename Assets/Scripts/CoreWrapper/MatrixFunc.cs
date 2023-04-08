using fts;
using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace InterNeuralNet.CoreWrapper
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Mat_Float
    {
        public int width;
        public int height;
        public IntPtr ptr;

        /// <summary>
        /// 将ptr设置为NativeArray参数的指针
        /// </summary>
        /// <param name="array"></param>
        public void SetPtr(NativeArray<float> array)
        {
            unsafe
            {
                ptr = (IntPtr)array.GetUnsafePtr();
            }
        }

        /// <summary>
        /// 将ptr设置为NativeArray参数的只读指针
        /// </summary>
        /// <param name="array"></param>
        public void SetReadOnlyPtr(NativeArray<float> array)
        {
            unsafe
            {
                ptr = (IntPtr)array.GetUnsafeReadOnlyPtr();
            }
        }
    }

    /// <summary>
    /// 以矩阵作为输入输出的原生接口.
    /// </summary>
    [PluginAttr(DllName)]
    public static class MatrixFunc
    {
        const string DllName = "NeuralNetworkCore";
#if DEBUG
        // 使用NativePluginLoader, 运行时自动重载, 而不需要重启 Unity
        [PluginFunctionAttr("add")]
        public static AddDelegate Add = null;
        public delegate void AddDelegate(ref Mat_Float a, ref Mat_Float b, ref Mat_Float dst);

        [PluginFunctionAttr("add_scalar")]
        public static AddScalarDelegate AddScalar = null;
        public delegate void AddScalarDelegate(ref Mat_Float a, float scalar, ref Mat_Float dst);

        [PluginFunctionAttr("fill")]
        public static FillDelegate Fill = null;
        public delegate void FillDelegate(ref Mat_Float dst, float value);

        [PluginFunctionAttr("multiply")]
        public static MultiplyDelegate Multiply = null;
        public delegate void MultiplyDelegate(ref Mat_Float a, ref Mat_Float b, ref Mat_Float dst);

        [PluginFunctionAttr("multiply_scalar")]
        public static MultiplyScalarDelegate MultiplyScalar = null;
        public delegate void MultiplyScalarDelegate(ref Mat_Float a, float scalar, ref Mat_Float dst);

        [PluginFunctionAttr("pooling_max")]
        public static MaxPoolingDelegate MaxPooling = null;
        public delegate void MaxPoolingDelegate(ref Mat_Float src, ref Mat_Float dst, int size);

        [PluginFunctionAttr("conv_layer")]
        public static ConvolutionDelegate Convolution = null;
        public delegate void ConvolutionDelegate(ref Mat_Float _in, ref Mat_Float kernel, float bias, ref Mat_Float _out, int padding);

        [PluginFunctionAttr("batch_conv_layer")]
        public static BatchConvolutionLayerDelegate BatchConvolutionLayer = null;
        public delegate void BatchConvolutionLayerDelegate(Mat_Float[] imgArray, Mat_Float[] kernels, ref Mat_Float bias, int inChannel, int outChannel, Mat_Float[] dst, int padding);

        [PluginFunctionAttr("full_connect_layer")]
        public static FullConnectionDelegate FullConnection = null;
        /// <summary>
        /// 含偏置量的全连接层
        /// </summary>
        /// <param name="_in">输入: mx1矩阵</param>
        /// <param name="weght">权重: nxm矩阵</param>
        /// <param name="bias">偏置: nx1矩阵</param>
        /// <param name="_out">输出: nx1矩阵</param>
        /// <returns></returns>
        public delegate void FullConnectionDelegate(ref Mat_Float _in, ref Mat_Float weght, ref Mat_Float bias, ref Mat_Float _out);
#else
        // 必须重启Unity才能重载
        [DllImport(DllName, EntryPoint = "add")]
        public static extern void Add(ref Mat_Float a, ref Mat_Float b, ref Mat_Float dst);

        [DllImport(DllName, EntryPoint ="add_scalar")]
        public static extern void AddScalar(ref Mat_Float a, float scalar, ref Mat_Float dst);

        [DllImport(DllName, EntryPoint ="fill")]
        public static extern void Fill(ref Mat_Float dst, float value);

        [DllImport(DllName, EntryPoint = "multiply")]
        public static extern void Multiply(ref Mat_Float a, ref Mat_Float b, ref Mat_Float dst);

        [DllImport(DllName, EntryPoint = "multiply_scalar")]
        public static extern void MultiplyScalar(ref Mat_Float a, float scalar, ref Mat_Float dst);

        [DllImport(DllName, EntryPoint = "pooling_max")]
        public static extern void MaxPooling(ref Mat_Float src, ref Mat_Float dst, int size);

        [DllImport(DllName, EntryPoint = "conv_layer")]
        public static extern void Convolution(ref Mat_Float _in, ref Mat_Float kernel, float bias, ref Mat_Float _out, int padding);

        [DllImport(DllName, EntryPoint = "batch_conv_layer")]
        public static extern void BatchConvolutionLayer(Mat_Float[] imgArray, Mat_Float[] kernels, ref Mat_Float bias, int inChannel, int outChannel, Mat_Float[] dst, int padding);

        /// <summary>
        /// 含偏置量的全连接层
        /// </summary>
        /// <param name="_in">输入: mx1矩阵</param>
        /// <param name="weght">权重: nxm矩阵</param>
        /// <param name="bias">偏置: nx1矩阵</param>
        /// <param name="_out">输出: nx1矩阵</param>
        /// <returns></returns>
        [DllImport(DllName, EntryPoint = "full_connect_layer")]
        public static extern void FullConnection(ref Mat_Float _in, ref Mat_Float weght, ref Mat_Float bias, ref Mat_Float _out);
#endif

    }
}
