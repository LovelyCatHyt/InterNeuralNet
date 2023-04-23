using InterNeuralNet.UI;
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
        public string label;
        public int frameType = 0;
        public bool dynamicMinMax = false;

        public List<SpriteRenderer> renderers = new List<SpriteRenderer>();
        public List<ViewInfo> infos = new List<ViewInfo>();
        public MatView view;

        private int _minMaxPropId = Shader.PropertyToID("_MinMax");
        private int _colorMapPropId = Shader.PropertyToID("_Gradient");
        private List<MatViewEventTrigger> _triggers = new List<MatViewEventTrigger>();
        private MatViewFrame _frame;

        private void Start()
        {
            if (renderers == null || renderers.Count == 0)
            {
                Debug.LogError($"\"{name}\" doesn't have any renderers!", this);
                renderers = new List<SpriteRenderer>();
                return;
            }
            if (view == null)
            {
                Debug.LogError($"\"{name}\" doesn't have a view binded!", this);
                return;
            }
            if (renderers.Count < view.textures.Length)
            {
                Debug.LogWarning($"\"{name}\"'s renderer count is less than matrix count!", this);
            }
            for (int i = 0; i < Mathf.Min(renderers.Count, view.textures.Length); i++)
            {
                renderers[i].sprite = SpriteCreator.CreateFixedWidth(view.textures[i], $"{name}[{i}]");
                var aspect = (float)view.textures[i].height / view.textures[i].width;
                // 设置对应的碰撞体尺寸 因为上面的Sprite创建只能保证宽度相同, 碰撞体初始形态仍然为正方形
                var collider = renderers[i].GetComponent<BoxCollider2D>();
                if (collider)
                {
                    collider.size = new Vector2(1, aspect);
                }
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
            // 创建外框UI
            _frame = UIManager.Inst.CreateMatViewFrame(frameType);
            _frame.Init(this);
            _frame.OnDeselect();
        }

        public void OnDeselect()
        {
            _frame.OnDeselect();
        }

        private void OnMouseOverSprite(int id, Vector2 uv)
        {
            if (id >= view.textures.Length) return;
            var toolBox = ToolBox.Inst;
            if (toolBox.focus != view) return;
            Vector2Int pixelPos = GetPixelPosition(id, uv);
            UIManager.Inst.statsUI.SetStats($"{name}({id})", pixelPos.y, pixelPos.x, view.ReadAt(id, pixelPos.y, pixelPos.x));
            if (Input.GetMouseButton(0)) toolBox.OperateAt(view, id, pixelPos.x, pixelPos.y);
        }

        private void OnMouseClick(int id, Vector2 uv)
        {
            if (id >= view.textures.Length) return;
            _frame.OnSelect();
            UIManager.Inst.SelectViewGO(this);
            // Vector2Int pixelPos = GetPixelPosition(id, uv);
            // ToolBox.Inst.OperateAt(view, id, pixelPos.x, pixelPos.y);
        }

        private Vector2Int GetPixelPosition(int id, Vector2 uv)
        {
            Texture2D tex = view.textures[id];
            var pixelPos = new Vector2Int((int)(uv.x * tex.width), (int)(uv.y * tex.height));
            return pixelPos;
        }

        /// <summary>
        /// 更新材质属性
        /// </summary>
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
            if (view == null) return;
            if (dynamicMinMax && view.IsPtrAvailable)
            {
                UpdateMinMaxInfo();
            }
            view.UpdateTexture();
            UpdateMaterialProp();
            UpdateFrame();
        }

        /// <summary>
        /// 更新视图框架
        /// </summary>
        private void UpdateFrame()
        {
            if (!_frame) return;
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue);
            foreach (var r in renderers)
            {
                var extents = r.sprite.bounds.extents;
                extents.z = 0;
                var tran = r.transform;
                var a = tran.TransformPoint(extents);
                var b = tran.TransformPoint(-extents);
                // 考虑到负数缩放 干脆a, b都一起比较
                min = Vector3.Min(min, a);
                min = Vector3.Min(min, b);
                max = Vector3.Max(max, a);
                max = Vector3.Max(max, b);
            }
            min = Camera.main.WorldToScreenPoint(min);
            max = Camera.main.WorldToScreenPoint(max);
            _frame.SetPosition(min, max);
        }

        private void UpdateMinMaxInfo()
        {
            for (int i = 0; i < Mathf.Min(renderers.Count, view.Shape.count); i++)
            {
                var info = infos[i];
                info.minMax = new Vector2(view.Mats[i].Min, view.Mats[i].Max);
                infos[i] = info;
            }
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
