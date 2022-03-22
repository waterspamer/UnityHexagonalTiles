using System.Collections.Generic;
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

            if (GUILayout.Button("Fill Possible Hexes"))
            {
                (target as PathVisualizer)?.FindPath();
            }
        }
        
        void OnSceneGUI( )
        {
            var b = target as PathVisualizer;
            var listOfPossibleUnits = b.possiblePlacedNewCoordsByNeighbours;
            foreach (var item in listOfPossibleUnits)
            {
                bool pressed = Handles.Button( item.worldPos, Quaternion.LookRotation(Vector3.down, Vector3.right), .3f, 6, Handles.RectangleHandleCap );
                if( pressed )
                {
                    b.AddNewHexUnit(item);
                    Debug.Log( "Instantiated!" );
                }
            }
        }
    }
}
