using UnityEngine;

namespace HexPathResources.Scripts
{
    public class SimpleCanvasController : MonoBehaviour
    {
        public Transform cameraTransform;
        // Update is called once per frame
        void Update()
        {
            transform.LookAt(cameraTransform);   
        }
    }
}
