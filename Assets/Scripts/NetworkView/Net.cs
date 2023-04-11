using InterNeuralNet.CoreWrapper;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InterNeuralNet.NetworkView
{
    public class Net
    {
        /// <summary>
        /// 输入视图
        /// </summary>
        public MatWritableView InputView { get; private set; }
        /// <summary>
        /// 所有过程输出的矩阵视图
        /// </summary>
        public MatView[] OutputViews { get; private set; }

        /// <summary>
        /// 所有视图
        /// </summary>
        private MatView[] _views;
        /// <summary>
        /// 所有可编辑参数的矩阵视图
        /// 由于参数可能为单个标量, 也可能有多个矩阵, 因此这里是个二维数组
        /// </summary>
        private MatWritableView[][] _paramViews;
        private Dictionary<string, MatWritableView[]> _paramViewsDict;
        private Layer _startLayer = null;
        private Layer _endLayer = null;
        private List<Layer> _layers;
        private Dictionary<string, Layer> _layerDict;

        public Net()
        {
            _layers = new List<Layer>();
            _layerDict = new Dictionary<string, Layer>();
        }

        /// <summary>
        /// 在末尾添加一个命名为 name 的层
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Net Append(Layer layer, string name)
        {
            if (_layerDict.ContainsKey(name))
            {
                Debug.LogWarning($"Layer \"{name}\" has been added. Layer getter will return last layer named \"{name}\".");
            }
            layer.name = name;
            _layerDict[name] = layer;
            _layers.Add(layer);
            if (_startLayer == null)
            {
                _startLayer = _endLayer = layer;
                return this;
            }
            _endLayer = _endLayer.Append(layer);
            return this;
        }

        public Layer GetLayer(string name) => _layerDict[name];
        public Layer GetLayer(int id) => _layers[id];

        public MatWritableView[] GetLayerParams(int id) => _paramViews[id];

        public MatWritableView[] GetLayerParams(string name) => _paramViewsDict[name];

        /// <summary>
        /// 使用一张现有的纹理作为初始值构建网络视图
        /// <para>构建时复制纹理数据 不会修改原纹理</para>
        /// </summary>
        /// <param name="initialInput"></param>
        public void BuildNetworkView(Texture2D initialInput)
        {
            BuildNetworkViewInternal(new MatWritableView(initialInput));
        }

        /// <summary>
        /// 构建网络视图
        /// </summary>
        public void BuildNetworkView(int inputWidth, int inputHeight)
        {
            BuildNetworkViewInternal(new MatWritableView(inputWidth, inputHeight));
        }

        private void BuildNetworkViewInternal(MatWritableView input)
        {
            MatView cntOutputView = InputView = input;
            var cntLayer = _startLayer;
            // 除了输入之外的全部 WritableView
            var writableList = new List<MatWritableView[]>();
            // 所有网络层的参数表
            _paramViewsDict = new Dictionary<string, MatWritableView[]>();
            // 所有输出 View
            var outputList = new List<MatView>();
            // 遍历每一层
            while (cntLayer != null)
            {
                // 参数
                MatWritableView[] @params = cntLayer.GetParamShapes().Select(s => new MatWritableView(s)).ToArray();
                writableList.Add(@params);
                _paramViewsDict[cntLayer.name] = @params;
                // 输出过程量
                cntOutputView = new MatView(cntLayer.CalcOutputShape(cntOutputView.Shape));
                outputList.Add(cntOutputView);
                // 转移到下一层
                cntLayer = cntLayer.next;
            }
            // 记录生成的各类视图
            _paramViews = writableList.ToArray();
            OutputViews = outputList.ToArray();
            // 所有视图的合集
            var tempList = new List<MatView>();
            tempList.Add(InputView);
            foreach (var views in writableList)
            {
                tempList.AddRange(views);
            }
            tempList.AddRange(OutputViews);
            _views = tempList.ToArray();
            // 最后输出构建结果
            Debug.Log($"Build finished. Tocal MatView count: {_views.Length}, Mats: {string.Join("\n", (IEnumerable<MatView>)_views)}");
        }

        public void Eval()
        {
            if (_views == null)
            {
                Debug.LogError("No matview available. Did you forget to call BuildNetworkView()?");
                return;
            }

            foreach (var view in _views)
            {
                view.EnableAccess();
            }

            // 遍历每一层 指定好输入输出的频道数和矩阵
            var cntLayer = _startLayer;
            _startLayer.input = InputView.Mats;
            _startLayer.inChannel = InputView.Mats.Length;
            for (int layerId = 0; ; layerId++)
            {
                var mats = OutputViews[layerId].Mats;
                cntLayer.output = mats;
                cntLayer.outChannel = mats.Length;
                // 更新参数
                var _params = new List<Mat_Float>();
                foreach (var view in _paramViews[layerId])
                {
                    _params.AddRange(view.Mats);
                }
                cntLayer.ApplyParams(_params.ToArray());
                if (cntLayer.next == null) break;
                cntLayer.next.input = mats;
                cntLayer.next.inChannel = mats.Length;
                // 下一个
                cntLayer = cntLayer.next;
            }

            // 执行运算
            _startLayer.Eval();

            foreach (var view in _views)
            {
                view.UpdateTexture();
            }

        }
    }
}
