using System.Collections.Generic;
using UnityEngine;

namespace HexPathResources.Scripts.DataStructs
{
    public static class HexConfiguration
    {
        public enum HexType
        {
            Empty,
            Hill,
            Forest,
            Swamp
        }

        public enum EventType
        {
            None,
            Cheap,
            Medium,
            Expensive
        }

        public static Dictionary<HexType, int> costByType = new Dictionary<HexType, int>()
        {
            {HexType.Empty, 1},
            {HexType.Hill, 2},
            {HexType.Forest, 5},
            {HexType.Swamp, 10}
        };

        public static Dictionary<EventType, int> costByEvent = new Dictionary<EventType, int>()
        {
            {EventType.None, 0},
            {EventType.Cheap, 1},
            {EventType.Medium, 5},
            {EventType.Expensive, 10}
        };


    }
}
