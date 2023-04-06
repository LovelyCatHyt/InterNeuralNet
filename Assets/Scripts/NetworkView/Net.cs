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
        /// </summary>
        private MatWritableView[] _paramViews;
        /// <summary>
        /// 所有过程输出的矩阵视图
        /// </summary>
        private MatView[] _outputViews;

        private Layer _startLayer = null;
        private Layer _endLayer = null;

        public Net Append(Layer layer)
        {
            if (_startLayer == null)
            {
                _startLayer = _endLayer = layer;
                return this;
            }
            _endLayer = _endLayer.Append(layer);
            return this;
        }

        /// <summary>
        /// 构建网络视图
        /// </summary>
        public void BuildNetworkView(int inputWidth, int inputHeight)
        {
            MatView cntView = _inputView = new MatWritableView(inputWidth, inputHeight);
            var cntLayer = _startLayer;
            // 除了输入之外的全部 WritableView
            var writableList = new List<MatWritableView>();
            // 所有输出 View
            var outputList = new List<MatView>();
            while (cntLayer != null)
            {
                writableList.AddRange(cntLayer.GetParamShapes().Select(s => new MatWritableView(s)));
                cntView = new MatView(cntLayer.CalcOutputShape(cntView.Shape));
                outputList.Add(cntView);
            }
            _paramViews = writableList.ToArray();
            _outputViews = outputList.ToArray();
        }

    }
}
