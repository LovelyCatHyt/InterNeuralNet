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

    [Range(0, 1)] public float fill;
    public float multiply;
    public float add;

    private Net testNet = new Net();

    public void Awake()
    {
        testNet
            .Append(new FillLayer(fill))
            .Append(new MultiplyScalarLayer(multiply))
            .Append(new AddScalarLayer(add));
        testNet.BuildNetworkView(32, 32);
    }

    // Update is called once per frame
    public void Update()
    {

    }
}
