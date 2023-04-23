using InterNeuralNet.CoreWrapper;
using InterNeuralNet.NetworkView.NetworkDefine;
using InterNeuralNet.UI;
using InterNeuralNet.UserEditTool;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace InterNeuralNet.NetworkView
{
    public class Lenet5Viewer : MonoBehaviour
    {
        public MatViewGO input;
        public MatViewGO conv1Weight;
        public MatViewGO conv1Bias;
        public MatViewGO conv2Weight;
        public MatViewGO conv2Bias;
        public MatViewGO fc1Weight;
        public MatViewGO fc1Bias;
        public MatViewGO fc2Weight;
        public MatViewGO fc2Bias;
        public MatViewGO fc3Weight;
        public MatViewGO fc3Bias;
        public MatViewGO conv1Output;
        public MatViewGO pa1Output;
        public MatViewGO conv2Output;
        public MatViewGO pa2Output;
        public MatViewGO fc1Output;
        public MatViewGO fc2Output;
        public MatViewGO fc3Output;
        // public MatViewGO finalOutput;

        public string networkParamsDataPath = "../NeuralNetworkCore/lenet-5.float32";

        private Net lenet5;
        public void Awake()
        {
            MatrixFunc.SetLogger(DefaultLogger);
            fts.NativePluginLoader.singleton.onBeforeUnloadPlugins += OnBeforeUnload;

            // 初始化, 并读取网络参数
            lenet5 = LeNet5.CreateFromFile(networkParamsDataPath);
            lenet5.InputView.EnableAccess();
            var inputMat = lenet5.InputView.Mat;
            MatrixFunc.Fill(ref inputMat, 0);
            // 连接视图
            SetViews();
        }

        private void OnBeforeUnload()
        {
            MatrixFunc.SetLogger(null);
        }

        private static void DefaultLogger(string message)
        {
            Debug.Log(message);
        }

        public void Start()
        {
            // 初始化绘图工具
            ToolBox.Inst.activate = ToolBox.Inst.GetComponent<ITool>();
            ToolBox.Inst.focus = (MatWritableView)input.view;
        }

        private void SetViews()
        {
            input.view = lenet5.InputView;
            // temp.view = lenet5.GetLayerParams("c1")[0]
            var cntParams = lenet5.GetLayerParams("c1");
            conv1Weight.view = cntParams[0];
            conv1Bias.view = cntParams[1];
            cntParams = lenet5.GetLayerParams("c2");
            conv2Weight.view = cntParams[0];
            conv2Bias.view = cntParams[1];
            cntParams = lenet5.GetLayerParams("fc1");
            fc1Weight.view = cntParams[0];
            fc1Bias.view = cntParams[1];
            cntParams = lenet5.GetLayerParams("fc2");
            fc2Weight.view = cntParams[0];
            fc2Bias.view = cntParams[1];
            cntParams = lenet5.GetLayerParams("fc3");
            fc3Weight.view = cntParams[0];
            fc3Bias.view = cntParams[1];


            conv1Output.view = lenet5.OutputViews[0];
            pa1Output.view = lenet5.OutputViews[1];
            conv2Output.view = lenet5.OutputViews[2];
            pa2Output.view = lenet5.OutputViews[3];
            fc1Output.view = lenet5.OutputViews[4];
            fc2Output.view = lenet5.OutputViews[5];
            fc3Output.view = lenet5.OutputViews[6];
        }

        // Update is called once per frame
        public void Update()
        {
            lenet5.InputView.EnableAccess();
            // infoText.text = $"core result: {lenet5Core.Eval(lenet5.InputView.Mat)}";
            lenet5.Eval();
            UIManager.Inst.statsUI.SetResult(fc3Output.view.Mat.ArgMaxInt());
        }
    }

}
