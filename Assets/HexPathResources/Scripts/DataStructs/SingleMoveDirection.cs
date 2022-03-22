
using System;
using System.Numerics;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

namespace HexPathResources.Scripts.DataStructs
{
    public static class SingleMoveDirection
    {
        public enum Direction
        {
            Up, //0 Y+ Z-
            UpRight, //X+ 0 Z+ 
            DownRight, //X+ Y- 0
            Down, //0 Y- Z-
            DownLeft, //X- 0 Z+
            UpLeft// X- Y+ 0
        }
        
        
        
        //       0
        //     -----
        //   5/     \1
        //   4\     /2
        //     -----
        //       3

        private static Vector3Int _offsetByDirection(this Direction dir)
        {
            switch (dir)
            {
                case Direction.Up:
                    return new Vector3Int(1, 1, 0);
                case Direction.UpRight:
                    return new Vector3Int(1, 0, -1);
                case Direction.DownRight:
                    return new Vector3Int(0, -1, -1);
                case Direction.Down:
                    return new Vector3Int(-1, -1, 0);
                case Direction.DownLeft:
                    return new Vector3Int(-1, 0, 1);
                case Direction.UpLeft:
                    return new Vector3Int(0, 1, 1);
                default:
                    return new Vector3Int();
            }
        }

        public static Vector3Int GetNeighbourCoordinateByDirection(this Vector3Int coord, Direction direction) =>
            coord + _offsetByDirection(direction);
    }
}
