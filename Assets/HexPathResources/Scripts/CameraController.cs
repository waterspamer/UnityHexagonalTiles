using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HexPathResources.Scripts
{
    public class CameraController : MonoBehaviour
    {
        public float cameraSpeed = 4f;
        public Transform cameraPivot;
        public Transform playerTransform;
        public PathVisualizer pathVisualizer;

        private Vector3 _offset;
        
        public Toggle toggleOff;
        public Toggle toggleFreeFollow;
        public Toggle toggleFree;
        

        private void Awake()
        {

            toggleOff.onValueChanged.AddListener(( value)=>
            {
                if (value)
                    SetControllingType(0);
            });
            
            toggleFreeFollow.onValueChanged.AddListener(( value)=>
            {
                if (value)
                    SetControllingType(1);
            });
            
            toggleFree.onValueChanged.AddListener(( value)=>
            {
                if (value)
                    SetControllingType(2);
            });
            
            
            
            _offset = cameraPivot.position - playerTransform.position;
            SetControllingType(2);
        }
        
        

        public void SetControllingType(int controllingMode)
        {
            if (controllingMode == -1) return;
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
