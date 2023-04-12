using InterNeuralNet.UserEditTool;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace InterNeuralNet.NetworkView
{
    [Serializable]
    public struct ViewInfo
    {
        public Vector2 minMax;
        public Texture2D colorMap;
    }

    /// <summary>
    /// 矩阵视图GameObject
    /// </summary>
    public class MatViewGO : MonoBehaviour
    {
        public List<SpriteRenderer> renderers = new List<SpriteRenderer>();
        public List<ViewInfo> infos = new List<ViewInfo>();
        public MatView view;

        private int _minMaxPropId = Shader.PropertyToID("_MinMax");
        private int _colorMapPropId = Shader.PropertyToID("_Gradient");
        private List<MatViewEventTrigger> _triggers = new List<MatViewEventTrigger>();

        private void Start()
        {
            if (renderers == null)
            {
                Debug.LogError("MatView doesn't have any renderers!");
                renderers = new List<SpriteRenderer>();
                return;
            }
            if (renderers.Count < view.textures.Length)
            {
                Debug.LogError("MatView's renderer count is less than matrix count!");
            }
            for (int i = 0; i < Mathf.Min(renderers.Count, view.textures.Length); i++)
            {
                renderers[i].sprite = SpriteCreator.CreateFixedWidth(view.textures[i], $"{name}[{i}]");
                renderers[i].material = new Material(renderers[i].sharedMaterial);
            }
            // 记录 triggers 并监听事件
            _triggers = new List<MatViewEventTrigger>();
            foreach (var r in renderers)
            {
                MatViewEventTrigger t = r.GetComponent<MatViewEventTrigger>();
                _triggers.Add(t);
                t.onMouseClick += OnMouseClick;
                t.onMouseOver += OnMouseOverSprite;
            }
            // 更新材质属性
            UpdateMaterialProp();
        }

        private void OnMouseOverSprite(int id, Vector2 uv)
        {
            Vector2Int pixelPos = GetPixelPosition(id, uv);
            if (Input.GetMouseButton(0)) ToolBox.Inst.OperateAt(view, id, pixelPos.x, pixelPos.y);
        }

        private void OnMouseClick(int id, Vector2 uv)
        {
            Vector2Int pixelPos = GetPixelPosition(id, uv);
            // ToolBox.Inst.OperateAt(view, id, pixelPos.x, pixelPos.y);
        }

        private Vector2Int GetPixelPosition(int id, Vector2 uv)
        {
            Texture2D tex = view.textures[id];
            var pixelPos = new Vector2Int((int)(uv.x * tex.width), (int)(uv.y * tex.height));
            return pixelPos;
        }

        private void UpdateMaterialProp()
        {
            for (int i = 0; i < renderers.Count; i++)
            {
                var material = renderers[i].material;
                material.SetVector(_minMaxPropId, infos[i].minMax);
                material.SetTexture(_colorMapPropId, infos[i].colorMap);
            }
        }

        private void Update()
        {
            UpdateMaterialProp();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (renderers == null)
            {
                renderers = new List<SpriteRenderer>();
                var temp = GetComponent<SpriteRenderer>();
                if (temp) renderers.Add(temp);
            }
            while (infos.Count < renderers.Count)
            {
                ViewInfo item;
                if (infos.Count > 0) item = infos.Last();
                else item = new ViewInfo();
                infos.Add(item);
            }

            for (int i = 0; i < renderers.Count; i++)
            {
                var r = renderers[i];
                var trigger = r.GetComponent<MatViewEventTrigger>();
                if (!trigger) continue;
                trigger.matViewGO = this;
                trigger.id = i;
            }
            // if(infos.Count > renderers.Count)
        }

        [ContextMenu("Update Data")]
        public void UpdateData()
        {
            OnValidate();
        }
#endif
    }

}
