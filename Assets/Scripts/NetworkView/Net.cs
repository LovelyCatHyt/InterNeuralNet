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
        private MatWritableView _inputView;
        /// <summary>
        /// 所有可编辑参数的矩阵视图
        /// 由于参数可能为单个标量, 也可能有多个矩阵, 因此这里是个二维数组
        /// </summary>
        private MatWritableView[][] _paramViews;
        /// <summary>
        /// 所有过程输出的矩阵视图
        /// </summary>
        public MatView[] OutputViews { get; private set; }
        /// <summary>
        /// 所有视图
        /// </summary>
        private MatView[] _views;

        private Layer _startLayer = null;
        private Layer _endLayer = null;
        private Dictionary<string, Layer> _layerDict;

        public Net()
        {
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
            _layerDict[name] = layer;
            if (_startLayer == null)
            {
                _startLayer = _endLayer = layer;
                return this;
            }
            _endLayer = _endLayer.Append(layer);
            return this;
        }

        public Layer GetLayer(string name) => _layerDict[name];
        // TODO: 增加用下标的访问器

        public MatWritableView[] GetLayerParameters(int id) => _paramViews[id];

        /// <summary>
        /// 构建网络视图
        /// </summary>
        public void BuildNetworkView(int inputWidth, int inputHeight)
        {
            MatView cntOutputView = _inputView = new MatWritableView(inputWidth, inputHeight);
            var cntLayer = _startLayer;
            // 除了输入之外的全部 WritableView
            var writableList = new List<MatWritableView[]>();
            // 所有输出 View
            var outputList = new List<MatView>();
            // 遍历每一层
            while (cntLayer != null)
            {
                writableList.Add(cntLayer.GetParamShapes().Select(s => new MatWritableView(s)).ToArray());
                cntOutputView = new MatView(cntLayer.CalcOutputShape(cntOutputView.Shape));
                outputList.Add(cntOutputView);
                cntLayer = cntLayer.next;
            }
            // 记录生成的各类视图
            _paramViews = writableList.ToArray();
            OutputViews = outputList.ToArray();
            // 所有视图的合集
            var tempList = new List<MatView>();
            tempList.Add(_inputView);
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
                view.WriteBegin();
            }

            // 遍历每一层 指定好输入输出的频道数和矩阵
            var cntLayer = _startLayer;
            _startLayer.input = OutputViews[0].Mats;
            _startLayer.inChannel = OutputViews[0].Mats.Length;
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
                view.WriteEnd();
            }

        }
    }
}
