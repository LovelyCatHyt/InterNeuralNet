using InterNeuralNet.NetworkView;
using InterNeuralNet.UserEditTool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace InterNeuralNet.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Inst { get; private set; }

        public MatViewGO SelectedViewGO { get; private set; }
        public StatsUI statsUI;
        public MatViewInfo viewInfo;

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

        public void SelectViewGO(MatViewGO matViewGO)
        {
            if (SelectedViewGO == matViewGO) return;
            if (SelectedViewGO)
            {
                SelectedViewGO.OnDeselect();
            }
            SelectedViewGO = matViewGO;
            var view = matViewGO.view;
            if (view is MatWritableView writableView)
            {
                ToolBox.Inst.focus = writableView;
            }
            else
            {
                ToolBox.Inst.focus = null;
            }
            viewInfo.SetContent(matViewGO.label, view.Shape.height, view.Shape.width, view.Shape.count);
        }   
    }

}
