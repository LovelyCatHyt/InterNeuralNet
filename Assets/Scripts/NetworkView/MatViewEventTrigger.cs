using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace InterNeuralNet.NetworkView
{
    /// <summary>
    /// 挂在每个矩阵视图上, 处理鼠标事件的组件
    /// </summary>
    public class MatViewEventTrigger : MonoBehaviour
    {
        [HideInInspector] public MatViewGO matViewGO;
        public int id;
        /// <summary>
        /// 鼠标进入事件
        /// <para>参数1: Mat ID</para>
        /// <para>参数2: 纹理归一化坐标</para>
        /// </summary>
        public event Action<int, Vector2> onMouseOver;
        /// <summary>
        /// 鼠标左键点击事件
        /// <para>参数1: Mat ID</para>
        /// <para>参数2: 纹理归一化坐标</para>
        /// </summary>
        public event Action<int, Vector2> onMouseClick;

        private SpriteRenderer sprite;

        private void Awake()
        {
            sprite ??= GetComponent<SpriteRenderer>();
        }

        private void OnMouseUpAsButton()
        {
            onMouseClick?.Invoke(id, GetMousePosAsUV());
        }

        private void OnMouseOver()
        {
            onMouseOver?.Invoke(id, GetMousePosAsUV());
        }

        private Vector2 GetMousePosAsUV()
        {
            var localPos = transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            // Debug.Log($"localPos {localPos}");
            // 坐标计算是基于这样的约定: sprite 创建时使用的纹理无论什么尺寸, 都通过pixelPerUnit属性强制拉伸到固定宽度为1.
            var aspect = sprite.sprite.rect.width / sprite.sprite.rect.height;
            localPos.y *= aspect;
            return new Vector2(localPos.x + 0.5f, localPos.y + 0.5f);
        }

        public Vector2Int ScreenToSpriteUV(Vector2 mousePosition)
        {
            throw new NotImplementedException();
        }

    }

}
