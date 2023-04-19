using InterNeuralNet.CoreWrapper;
using InterNeuralNet.NetworkView;
using InterNeuralNet.NetworkView.NetworkDefine;
using InterNeuralNet.UserEditTool;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Tester : MonoBehaviour
{
    public MatViewGO input;
    public MatViewGO temp;
    public MatViewGO output1;
    public MatViewGO output2;
    public MatViewGO finalOutput;

    public Text infoText;

    // public Texture2D intialInputTexture;

    public string networkParamsDataPath = "../NeuralNetworkCore/lenet-5.float32";

    private Net lenet5;

    private Lenet5Core lenet5Core;

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
        // Debug: 创建一个c++核心的网络, 由c++执行内存分配和参数传递
        lenet5Core = new Lenet5Core(networkParamsDataPath);
    }

    private void OnBeforeUnload()
    {
        MatrixFunc.SetLogger(null);
        lenet5Core.Release();
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
        temp.view = lenet5.GetLayerParams("c1")[0];
        output1.view = lenet5.OutputViews[0];
        output2.view = lenet5.OutputViews[3];
        finalOutput.view = lenet5.OutputViews.Last();
    }

    // Update is called once per frame
    public void Update()
    {
        lenet5.InputView.EnableAccess();
        infoText.text = $"core result: {lenet5Core.Eval(lenet5.InputView.Mat)}";
        lenet5.Eval();
    }
}
