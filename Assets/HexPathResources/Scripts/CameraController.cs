using System;
using UnityEngine;

namespace HexPathResources.Scripts
{
    public class CameraController : MonoBehaviour
    {
        public float cameraSpeed = 4f;
        public Transform cameraPivot;
        public Transform playerTransform;
        public PathVisualizer pathVisualizer;

        private Vector3 _offset;


        private void Awake()
        {
            _offset = cameraPivot.position - playerTransform.position;
            SetControllingType(2);
        }

        public void SetControllingType(int controllingMode)
        {
            cameraControllingMode = (CameraControllingMode) controllingMode;
        }

        public enum CameraControllingMode
        {
            Off,
            FreeFollow,
            Free
        }

        public CameraControllingMode cameraControllingMode;

        private void Update()
        {
            switch (cameraControllingMode)
            {
                case CameraControllingMode.Free:
                    return;
                case CameraControllingMode.Off:
                    transform.position = Vector3.Lerp(transform.position, playerTransform.position + _offset,
                        Time.deltaTime * cameraSpeed);
                    break;
                case CameraControllingMode.FreeFollow:
                    if (pathVisualizer.movingFlag)transform.position = Vector3.Lerp(transform.position, playerTransform.position + _offset,
                        Time.deltaTime * cameraSpeed);
                    else
                        return;
                    break;
            }
        }
    }
}
