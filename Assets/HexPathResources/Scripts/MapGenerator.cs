using System;
using System.Collections.Generic;
using HexPathResources.Scripts.DataStructs;
using UnityEngine;

namespace HexPathResources.Scripts
{
    
    //TODO 
    public class MapGenerator : MonoBehaviour
    {

        [Header("Generation Settings")]
        [SerializeField]
        
        public GameObject hexagonPrefab;

        public float obstacleProbability;

        public int radius = 1;

        public float xOffset;
        public float zOffset;

        public List<HexUnit> units;

        private void Awake()
        {
            
        }


        public void GenerateMap()
        {

            GenerateNeighbours();
            
        }

        public void GenerateNeighbours()
        {
            for (int i = -radius; i <= radius; ++i)
            {
                for (int j = -radius; j <= radius; ++j)
                {
                    for (int k = -radius; k <= radius; ++k)
                    {

                        if (i % 2 != 0 && k % 2 !=0)
                        {
                            Instantiate(hexagonPrefab, new Vector3(i * xOffset, 0, k * zOffset), Quaternion.identity);
                            hexagonPrefab.GetComponent<HexUnit>().coordinates = new Vector3Int(i, j, k);
                        }
                        
                    }
                }
            }
        }



    }
}
