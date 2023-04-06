using InterNeuralNet.CoreWrapper;
using InterNeuralNet.NetworkView;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour
{
    public SpriteRenderer r1;
    public SpriteRenderer r2;
    public SpriteRenderer r3;

    public SpriteCreator creator = new SpriteCreator();

    [Range(0, 1)] public float fill;
    public float multiply;
    public float add;

    private Net _testNet = new Net();
    private FillLayer _fillLayer;
    private MultiplyScalarLayer _multiplyLayer;
    private AddScalarLayer _addLayer;

    public void Awake()
    {
        _fillLayer = new FillLayer(fill);
        _multiplyLayer = new MultiplyScalarLayer(multiply);
        _addLayer = new AddScalarLayer(add);
        _testNet
            .Append(_fillLayer, "Fill")
            .Append(_multiplyLayer, "Multiply")
            .Append(_addLayer, "Add");
        _testNet.BuildNetworkView(32, 32);
        r1.sprite = creator.Create(_testNet.OutputViews[0].textures[0], "Output[0]");
        r2.sprite = creator.Create(_testNet.OutputViews[1].textures[0], "Output[1]");
        r3.sprite = creator.Create(_testNet.OutputViews[2].textures[0], "Output[2]");
    }

    // Update is called once per frame
    public void Update()
    {
        _fillLayer.scalar = fill;
        _multiplyLayer.scalar = multiply;
        _addLayer.scalar = add;
        _testNet.Eval();
    }
}
