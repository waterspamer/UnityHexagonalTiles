using System;
using UnityEngine;

namespace HexPathResources.Scripts
{
    public class CameraController : MonoBehaviour
    {
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
                    return;
                case CameraControllingMode.FreeFollow:
                    return;
            }
        }
    }
}
