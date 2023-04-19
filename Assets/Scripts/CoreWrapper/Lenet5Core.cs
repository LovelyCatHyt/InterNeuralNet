using System;
using System.IO;
using System.Runtime.InteropServices;
using fts;
using UnityEngine;

namespace InterNeuralNet.CoreWrapper
{
    [PluginAttr(DllName)]
    public static class Lenet5_CoreFunc
    {
        const string DllName = "NeuralNetworkCore";

        [PluginFunctionAttr("create_lenet_core")]
        public static CreateLenetDelegate CreateLenet = null;
        public delegate IntPtr CreateLenetDelegate(string path);

        [PluginFunctionAttr("eval_core")]
        public static EvalDelegate Eval = null;
        public delegate int EvalDelegate(IntPtr lenetPtr, ref Mat_Float img);

        [PluginFunctionAttr("release_core")]
        public static ReleaseLenetDelegate ReleaseLenet = null;
        public delegate void ReleaseLenetDelegate(IntPtr lenetPtr);
    }

    public class Lenet5Core
    {
        public IntPtr lenetPtr;
        public Lenet5Core(string path)
        {
            var combined = Path.Combine(Application.dataPath, path);
            lenetPtr = Lenet5_CoreFunc.CreateLenet(Path.GetFullPath(combined));
        }

        public int Eval(ref Mat_Float mat) => Lenet5_CoreFunc.Eval(lenetPtr, ref mat);
        public int Eval(Mat_Float mat) => Lenet5_CoreFunc.Eval(lenetPtr, ref mat);

        public void Release()
        {
            Lenet5_CoreFunc.ReleaseLenet(lenetPtr);
            lenetPtr = IntPtr.Zero;
        }

        ~Lenet5Core()
        {
            if (lenetPtr != IntPtr.Zero) Lenet5_CoreFunc.ReleaseLenet(lenetPtr);
        }
    }
}
