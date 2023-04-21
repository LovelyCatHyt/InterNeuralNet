using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace InterNeuralNet.UI
{

    public class StatsUI : MonoBehaviour
    {
        public Text valueText;
        public string statsFormat = "{0}[{1}, {2}]: {3:f4}";
        public Text resultText;
        public string resultFormat = "当前识别结果: {0}";

        private void Update()
        {
            var mousePos = Input.mousePosition;
            if (!Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(mousePos)))
            {
                valueText.text = "<void>";
            }
        }

        public void SetStats(string label, int row, int col, float value)
        {
            valueText.text = string.Format(statsFormat, label, row, col, value);
        }

        public void SetResult(int result)
        {
            resultText.text = string.Format(resultFormat, result);
        }
    }

}
