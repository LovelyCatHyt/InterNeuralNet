using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InterNeuralNet.CameraControl
{
    public class MainCamera : MonoBehaviour
    {
        public float scrollRate = 0.8f;

        private Camera _cam;
        private Transform _camTran;
        private Vector3 _lastMouseWorldPos;
        // private float _size;

        public void Awake()
        {
            _cam = Camera.main;
            _camTran = _cam.transform;
            // _size = _cam.orthographicSize;
        }

        public void Update()
        {
            Vector3 mouseWorldPos = _cam.ScreenToWorldPoint(Input.mousePosition);
            // 中键拖拽
            if (Input.GetMouseButtonDown(2))
            {
                _lastMouseWorldPos = _cam.ScreenToWorldPoint(Input.mousePosition);
            }
            if (Input.GetMouseButton(2))
            {
                _camTran.position -= mouseWorldPos - _lastMouseWorldPos;
            }
            mouseWorldPos = _cam.ScreenToWorldPoint(Input.mousePosition);
            // Ctrl+鼠标滚轮放缩
            if (Input.GetKey(KeyCode.LeftControl))
            {
                var scroll = Input.GetAxisRaw("Mouse ScrollWheel");

                var posDelta = mouseWorldPos - _camTran.position;
                var scale = Mathf.Pow(scrollRate, scroll);
                _cam.orthographicSize *= scale;
                posDelta *= scale;
                _camTran.position = mouseWorldPos - posDelta;
            }
            _lastMouseWorldPos = _cam.ScreenToWorldPoint(Input.mousePosition);
        }
    }

}
