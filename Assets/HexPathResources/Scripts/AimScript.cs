using UnityEngine;

namespace HexPathResources.Scripts
{
    public class AimScript : MonoBehaviour
    {
        public Vector3 targetPos;
        public float speed;

        // Update is called once per frame
        void Update() =>
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * speed);
    }
}
