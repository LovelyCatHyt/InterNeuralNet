using UnityEngine;
using System.IO;
using InterNeuralNet.CoreWrapper;

namespace InterNeuralNet.NetworkView.NetworkDefine
{
    public class LeNet5
    {
        public Net net;

        public LeNet5(string filePath)
        {
            net = CreateFromFile(filePath);
        }

        /// <summary>
        /// 从文件中读取参数, 构建 LeNet5 模型
        /// </summary>
        /// <param name="filePath">以 Assets/ (Editor内) 或者 $ProjectName$_Data/ (程序包) 为起点的路径</param>
        /// <returns></returns>
        public static Net CreateFromFile(string filePath)
        {
            var net = new Net();
            net
                .Append(new ConvolutionLayer(1, 6, 5, 2), "c1")
                .Append(new MaxPoolingLayer(2), "pa1")
                .Append(new ConvolutionLayer(6, 16, 5, 0), "c2")
                .Append(new MaxPoolingLayer(2), "pa2")
                .Append(new FullConnectionLayer(16 * 5 * 5, 120), "fc1")
                .Append(new FullConnectionLayer(120, 84), "fc2")
                .Append(new FullConnectionLayer(84, 10), "fc3");

            net.BuildNetworkView(28, 28);
            try
            {
                var combined = Path.Combine(Application.dataPath, filePath);
                using (FileStream file = new FileStream(Path.GetFullPath(combined), FileMode.Open))
                {
                    // 跳过文件头
                    var buffer = new byte[sizeof(int)];
                    file.Read(buffer, 0, 4);
                    file.Seek(System.BitConverter.ToInt32(buffer), SeekOrigin.Current);
                    net.LoadFromFile(file);
                    file.Close();
                }
            }
            catch (FileNotFoundException)
            {
                Debug.LogError($"Network parameter file \"{filePath}\" is not found!");
                return net;
            }
            return net;
        }
    }

}
