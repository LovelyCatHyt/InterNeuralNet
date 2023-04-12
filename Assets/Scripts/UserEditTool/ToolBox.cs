using InterNeuralNet.NetworkView;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace InterNeuralNet.UserEditTool
{
    /// <summary>
    /// 编辑工具箱
    /// </summary>
    public class ToolBox : MonoBehaviour
    {
        public static ToolBox Inst { get; private set; }

        public ITool activate;

        public void Awake()
        {
            if (Inst)
            {
                Debug.LogWarning($"Duplicated {nameof(ToolBox)}!", this);
                return;
            }
            Inst = this;
        }

        public void OperateAt(MatView view, int matId, int x, int y)
        {
            activate?.OperateAt(view, matId, x, y, Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
        }
    }
}
