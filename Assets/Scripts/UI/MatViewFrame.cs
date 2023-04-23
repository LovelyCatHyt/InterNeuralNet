using InterNeuralNet.NetworkView;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace InterNeuralNet.UI
{
    public class MatViewFrame : MonoBehaviour
    {
        public string sizeFormat = "尺寸: {0}x{1}, 数量: {2}";


        public MatViewGO viewGO;
        public RawImage frameImg;
        public GameObject frameGO;
        public Color selectColor;
        public Color unSelectColor;

        private Vector3 initScale;

        public void Init(MatViewGO viewGO)
        {
            this.viewGO = viewGO;
            initScale = viewGO.transform.localScale;
        }

        /// <summary>
        /// 使用屏幕坐标的两个点定位
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public void SetPosition(Vector3 min, Vector3 max)
        {
            var rectTran = transform as RectTransform;
            rectTran.anchoredPosition = min;
            rectTran.sizeDelta = max - min;
        }

        public void HideOrShow()
        {
            if (viewGO.transform.localScale.sqrMagnitude > 0)
            {
                viewGO.transform.localScale = Vector3.zero;
            }
            else
            {
                viewGO.transform.localScale = initScale;
                UIManager.Inst.SelectViewGO(viewGO);
            }

        }

        public void OnSelect()
        {
            // TODO: 更丰富的效果
            frameGO.SetActive(true);
            frameImg.color = selectColor;
        }

        public void OnDeselect()
        {
            if (viewGO.transform.localScale.sqrMagnitude > 0) frameGO.SetActive(false);
            frameImg.color = unSelectColor;
        }
    }

}
