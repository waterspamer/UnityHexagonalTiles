using System.Collections.Generic;
using UnityEngine;

namespace HexPathResources.Scripts.DataStructs
{
    [System.Serializable]
    public class GeneratedHexDataWrapper
    {
        public Vector3Int matrixCoords;
        public List<HexUnit> neighbours;
        public Vector3 worldPos;
        public bool isObstacle;
        
        public GeneratedHexDataWrapper(Vector3Int matrixCoords, List<HexUnit> neighbours, UnityEngine.Vector3 worldPos, bool isObstacle)
        {
            this.matrixCoords = matrixCoords;
            this.neighbours = neighbours;
            this.worldPos = worldPos;
            this.isObstacle = isObstacle;
        }
        
    }
}
