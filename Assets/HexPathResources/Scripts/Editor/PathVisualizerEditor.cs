using UnityEditor;
using UnityEngine;

namespace HexPathResources.Scripts.Editor
{
    
    [CustomEditor(typeof(PathVisualizer))]
    public class PathVisualizerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("ShowPath"))
            {
                (target as PathVisualizer)?.FindPath();
            }
        }
    }
}
