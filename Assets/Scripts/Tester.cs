using InterNeuralNet.CoreWrapper;
using InterNeuralNet.NetworkView;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour
{
    public MatViewGO input;
    public MatViewGO mulOutput;
    public MatViewGO addOutput;
    public MatViewGO convWeight;
    public MatViewGO convOutput;

    public Texture2D intialInputTexture;

    public float multiply;
    public float add;

    private Net _testNet = new Net();
    private MultiplyScalarLayer _multiplyLayer;
    private AddScalarLayer _addLayer;
    private ConvolutionLayer _convLayer;

    private float[][] kernel = new[]{
        new float[]{1,0,-1},
        new float[]{2,0,-2},
        new float[]{1,0,-1},
    };

    public void Awake()
    {
        _multiplyLayer = new MultiplyScalarLayer(multiply);
        _addLayer = new AddScalarLayer(add);
        _convLayer = new ConvolutionLayer(1, 1, 3, 0);
        _testNet
            .Append(_multiplyLayer, "Multiply")
            .Append(_addLayer, "Add")
            .Append(_convLayer, "Conv");
        // 构建视图 调用后已经分配纹理的内存空间 可以读写了
        _testNet.BuildNetworkView(intialInputTexture);

        // 写入卷积核
        var convViews = _testNet.GetLayerParams("Conv");
        var weightView = convViews[0];
        var biasView = convViews[1];
        weightView.EnableAccess();
        biasView.EnableAccess();
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                weightView.WriteAt(row, col, kernel[row][col]);
            }
        }
        biasView.WriteAt(0, 0, 0);
        weightView.UpdateTexture();
        biasView.UpdateTexture();

        // 连接视图
        SetViews();
    }

    private void SetViews()
    {
        input.view = _testNet.InputView;
        mulOutput.view = _testNet.OutputViews[0];
        addOutput.view = _testNet.OutputViews[1];
        convWeight.view = _testNet.GetLayerParams("Conv")[0];
        convOutput.view = _testNet.OutputViews[2];
    }

    // Update is called once per frame
    public void Update()
    {
        _multiplyLayer.scalar = multiply;
        _addLayer.scalar = add;
        // Debug.Log("tick");
        _testNet.Eval();
    }
}
