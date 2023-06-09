using fts;
using System;
using System.IO;
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

        public void ReadFromFile(FileStream f)
        {
            var len = width * height;
            unsafe
            {
                Span<byte> buffer = stackalloc byte[4];
                for (int i = 0; i < len; i++)
                {
                    f.Read(buffer);
                    *(float*)(ptr + sizeof(float) * i) = BitConverter.ToSingle(buffer);
                }
            }
        }

        public void Fill(float value) => MatrixFunc.Fill(ref this, value);

        public float Min => MatrixFunc.Min(ref this);
        public float Max => MatrixFunc.Max(ref this);

        public int ArgMinInt() => MatrixFunc.ArgMin(ptr, width * height);
        public int ArgMaxInt() => MatrixFunc.ArgMax(ptr, width * height);
    }

    /// <summary>
    /// 以矩阵作为输入输出的原生接口.
    /// </summary>
    [PluginAttr(DllName)]
    public static class MatrixFunc
    {
        const string DllName = "NeuralNetworkCore";

        public delegate void Logger(string message);

#if DEBUG
        // 使用NativePluginLoader, 运行时自动重载, 而不需要重启 Unity

        [PluginFunctionAttr("remove_logger")]
        public static RemoveLoggerDelegate RemoveLogger = null;
        public delegate void RemoveLoggerDelegate();

        [PluginFunctionAttr("set_logger")]
        public static SetLoggerDelegate SetLogger = null;
        public delegate void SetLoggerDelegate(Logger logger);

        [PluginFunctionAttr("arg_max")]
        public static ArgMinMaxDelegate ArgMax = null;
        [PluginFunctionAttr("arg_min")]
        public static ArgMinMaxDelegate ArgMin = null;
        public delegate int ArgMinMaxDelegate(IntPtr ptr, int length);

        [PluginFunctionAttr("min_element")]
        public static MinMaxDelegate Min = null;
        [PluginFunctionAttr("max_element")]
        public static MinMaxDelegate Max = null;
        public delegate float MinMaxDelegate(ref Mat_Float mat);

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

        [PluginFunctionAttr("full_connect_layer_arr")]
        public static FullConnectionArrDelegate FullConnectionArr = null;
        /// <summary>
        /// 含偏置量的全连接层, 但输入是数组
        /// </summary>
        /// <param name="_in">输入: 可以组成mx1大小的相同大小的矩阵数组</param>
        /// <param name="weght">权重: nxm矩阵</param>
        /// <param name="bias">偏置: nx1矩阵</param>
        /// <param name="_out">输出: nx1矩阵</param>
        /// <returns></returns>
        public delegate void FullConnectionArrDelegate(Mat_Float[] inArr, ref Mat_Float weght, ref Mat_Float bias, ref Mat_Float _out);
#else
        // 必须重启Unity才能重载

        [DllImport(DllName, EntryPoint ="arg_max")]
        public static extern int ArgMax(IntPtr ptr, int length);
        [DllImport(DllName, EntryPoint = "arg_min")]
        public static extern int ArgMin(IntPtr ptr, int length);

        [DllImport(DllName, EntryPoint ="max_element")]
        public static extern float Max(ref Mat_Float mat);
        [DllImport(DllName, EntryPoint = "min_element")]
        public static extern float Min(ref Mat_Float mat);

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
        
        /// <summary>
        /// 含偏置量的全连接层, 但输入是数组
        /// </summary>
        /// <param name="_in">输入: 可以组成mx1大小的相同大小的矩阵数组</param>
        /// <param name="weght">权重: nxm矩阵</param>
        /// <param name="bias">偏置: nx1矩阵</param>
        /// <param name="_out">输出: nx1矩阵</param>
        /// <returns></returns>
        [DllImport(DllName, EntryPoint = "full_connect_layer_arr")]
        public static extern void FullConnectionArr(Mat_Float[] inArr, ref Mat_Float weght, ref Mat_Float bias, ref Mat_Float _out);
#endif

    }
}
