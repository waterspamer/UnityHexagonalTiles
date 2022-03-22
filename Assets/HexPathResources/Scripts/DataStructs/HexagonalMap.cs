using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace HexPathResources.Scripts.DataStructs
{
    public static class HexagonalMap
    {
        public static List<HexUnit> Units;

        public static HexUnit TryGetUnitByCoordinate(Vector3Int coordinate) =>
            Units.Where((unit => unit.coordinates == coordinate)).Last();
    }
}
