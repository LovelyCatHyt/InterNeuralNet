using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace InterNeuralNet.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Inst { get; private set; }

        public StatsUI statsUI;

        [SerializeField] private MatViewFrame[] _framePrefabs;

        public MatViewFrame CreateMatViewFrame(int type)
        {
            var go = Instantiate(_framePrefabs[type].gameObject, transform);
            return go.GetComponent<MatViewFrame>();
        }

        private void Awake()
        {
            if (Inst)
            {
                Debug.LogWarning($"Duplicated {nameof(UIManager)}!");
                Destroy(this);
                return;
            }
            Inst = this;
            if (statsUI == null)
            {
                statsUI = FindObjectOfType<StatsUI>();
            }
        }
    }

}
