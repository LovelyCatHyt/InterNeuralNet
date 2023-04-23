using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace InterNeuralNet.UI
{
    public class MatViewInfo : MonoBehaviour
    {
        public Text title;
        public Text size;
        public string sizeFormat = "尺寸: {0}x{1}, 数量: {2}";

        void Awake()
        {
            title.text = "";
            size.text = "";
        }

        public void SetContent(string _title, int height, int width, int count)
        {
            title.text = _title;
            size.text = string.Format(sizeFormat, height, width, count);
        }
    }

}
