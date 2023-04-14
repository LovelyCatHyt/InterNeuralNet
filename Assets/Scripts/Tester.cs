using InterNeuralNet.CoreWrapper;
using InterNeuralNet.NetworkView;
using InterNeuralNet.NetworkView.NetworkDefine;
using InterNeuralNet.UserEditTool;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tester : MonoBehaviour
{
    public MatViewGO input;
    public MatViewGO finalOutput;

    // public Texture2D intialInputTexture;

    public string networkParamsDataPath = "../NeuralNetworkCore/lenet-5.float32";

    private Net lenet5;

    public void Awake()
    {
        // 初始化, 并读取网络参数
        lenet5 = LeNet5.CreateFromFile(networkParamsDataPath);
        lenet5.InputView.EnableAccess();
        var inputMat = lenet5.InputView.Mat;
        MatrixFunc.Fill(ref inputMat, 0);
        // 连接视图
        SetViews();
    }

    public void Start()
    {
        // 初始化绘图工具
        ToolBox.Inst.activate = ToolBox.Inst.GetComponent<ITool>();
    }

    private void SetViews()
    {
        input.view = lenet5.InputView;
        finalOutput.view = lenet5.OutputViews.Last();
    }

    // Update is called once per frame
    public void Update()
    {
        lenet5.Eval();
    }
}
