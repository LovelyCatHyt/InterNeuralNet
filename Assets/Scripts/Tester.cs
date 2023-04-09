using InterNeuralNet.CoreWrapper;
using InterNeuralNet.NetworkView;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour
{
    public SpriteRenderer input;
    public SpriteRenderer kernelSprite;
    public SpriteRenderer r1;
    public SpriteRenderer r2;
    public SpriteRenderer r3;

    public SpriteCreator creator = new SpriteCreator();

    public Texture2D intialInputTexture;

    [Range(0, 1)] public float fill;
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
        var convViews = _testNet.GetLayerParameters(2);
        var weightView = convViews[0];
        var biasView = convViews[1];
        weightView.WriteBegin();
        biasView.WriteBegin();
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                weightView.WriteAt(row, col, kernel[row][col]);
            }
        }
        biasView.WriteAt(0, 0, 0);
        weightView.WriteEnd();
        biasView.WriteEnd();

        // 创建 Sprite
        input.sprite = creator.Create(_testNet.InputView.textures[0]);
        kernelSprite.sprite = creator.Create(weightView.textures[0]);
        r1.sprite = creator.Create(_testNet.OutputViews[0].textures[0], "Output[0]");
        r2.sprite = creator.Create(_testNet.OutputViews[1].textures[0], "Output[1]");
        r3.sprite = creator.Create(_testNet.OutputViews[2].textures[0], "Output[2]");
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
