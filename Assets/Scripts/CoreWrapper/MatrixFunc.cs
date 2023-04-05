using fts;
using System;
using System.Runtime.InteropServices;
using Unity.VisualScripting;

namespace InterNeuralNet.CoreWrapper
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Mat_Float
    {
        public int width;
        public int height;
        public IntPtr ptr;
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

        [PluginFunctionAttr("multiply")]
        public static MulticastDelegate Multiply = null;
        public delegate void MultiplyDelegate(Mat_Float a, Mat_Float b, Mat_Float dst);

        [PluginFunctionAttr("pooling_max")]
        public static MaxPoolingDelegate MaxPooling = null;
        public delegate void MaxPoolingDelegate(Mat_Float src, Mat_Float dst, int size);

        [PluginFunctionAttr("conv_layer")]
        public static ConvolutionDelegate Convolution = null;
        public delegate void ConvolutionDelegate(Mat_Float _in, Mat_Float kernel, float bias, Mat_Float _out, int padding);

        [PluginFunctionAttr("batch_conv_layer")]
        public static BatchConvolutionLayerDelegate BatchConvolutionLayer = null;
        public delegate void BatchConvolutionLayerDelegate(Mat_Float[] imgArray, Mat_Float[] kernels, Mat_Float bias, int inChannel, int outChannel, Mat_Float[] dst, int padding);

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
        public delegate void FullConnectionDelegate(Mat_Float _in, Mat_Float weght, Mat_Float bias, Mat_Float _out);
#else
        // 必须重启Unity才能重载
        [DllImport(DllName, EntryPoint = "multiply")]
        public static extern void Multiply(Mat_Float a, Mat_Float b, Mat_Float dst);

        [DllImport(DllName, EntryPoint = "pooling_max")]
        public static extern void MaxPooling(Mat_Float src, Mat_Float dst, int size);

        [DllImport(DllName, EntryPoint = "conv_layer")]
        public static extern void Convolution(Mat_Float _in, Mat_Float kernel, float bias, Mat_Float _out, int padding);

        [DllImport(DllName, EntryPoint = "batch_conv_layer")]
        public static extern void BatchConvolutionLayer(Mat_Float[] imgArray, Mat_Float[] kernels, Mat_Float bias, int inChannel, int outChannel, Mat_Float[] dst, int padding);

        /// <summary>
        /// 含偏置量的全连接层
        /// </summary>
        /// <param name="_in">输入: mx1矩阵</param>
        /// <param name="weght">权重: nxm矩阵</param>
        /// <param name="bias">偏置: nx1矩阵</param>
        /// <param name="_out">输出: nx1矩阵</param>
        /// <returns></returns>
        [DllImport(DllName, EntryPoint = "full_connect_layer")]
        public static extern void FullConnection(Mat_Float _in, Mat_Float weght, Mat_Float bias, Mat_Float _out);
#endif


    }
}
