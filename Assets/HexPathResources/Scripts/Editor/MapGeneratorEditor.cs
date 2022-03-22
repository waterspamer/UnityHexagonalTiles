using HexPathResources.Scripts.DataStructs;
using UnityEditor;
using UnityEngine;

namespace HexPathResources.Scripts.Editor
{
    [CustomEditor(typeof (MapGenerator))]
    public class MapGeneratorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Generate"))
            {
                HexagonalMap.Units = (target as MapGenerator)?.units;

            }
        }
    }
}
