using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace InterNeuralNet.UI
{
    public class MatViewFrame : MonoBehaviour
    {
        public Text title;
        public Text size;
        public string sizeFormat = "尺寸: {0}x{1}, 数量: {2}";
        public RawImage colorMapImg;

        public void SetContent(string _title, int height, int width, int count)
        {
            title.text = _title;
            size.text = string.Format(sizeFormat, height, width, count);
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

 
    }

}
