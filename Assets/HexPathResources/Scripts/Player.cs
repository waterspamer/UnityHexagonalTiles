using System;
using HexPathResources.Scripts.DataStructs;
using UnityEngine;

namespace HexPathResources.Scripts
{
    public class Player : MonoBehaviour
    {
        public bool isWalking;

        public PathVisualizer pathVisualizer;

        public HexUnit targetUnit;
        
        private void FixedUpdate()
        {
            transform.position = Vector3.Lerp(transform.position, targetUnit.transform.position, Time.deltaTime * 5f);
        }
    }
}
