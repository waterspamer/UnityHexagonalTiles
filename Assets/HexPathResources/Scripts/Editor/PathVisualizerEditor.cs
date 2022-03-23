using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using HexPathResources.Scripts.DataStructs;
using PlasticPipe.Tube;
using UnityEditor;
using UnityEngine;
using UnityEditor.EditorTools;
using UnityEditor.SceneManagement;

namespace HexPathResources.Scripts.Editor
{
    
    [CustomEditor(typeof(PathVisualizer))]
    public class PathVisualizerEditor : UnityEditor.Editor
    {
        
        SerializedProperty unitList;

        private void OnEnable()
        {
            unitList = serializedObject.FindProperty("units");
        }

        void OnSceneGUI( )
        {
            
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            var pTarget = target as PathVisualizer;
            
            if (pTarget.locker) return;
            foreach (var item in pTarget.possiblePlacedNewCoordsByNeighbours)
            {
                bool pressed = Handles.Button( item.worldPos, Quaternion.LookRotation(Vector3.down, Vector3.right), .3f, .3f, Handles.RectangleHandleCap );
                if( pressed )
                {
                    
                    Undo.RecordObject(pTarget, "Added new tile");
                    var list = new List<HexUnit>(pTarget.units);
                    Debug.Log(list.Count);
                    var newUnit = Instantiate((target as PathVisualizer).hexPrefab, item.worldPos, Quaternion.identity, (target as PathVisualizer).transform);
                    Undo.RegisterCreatedObjectUndo(newUnit, "Instantiation of new tile");
                    Undo.RecordObject(newUnit, "New unit properties");
                    var comp = newUnit.GetComponent<HexUnit>();
                    comp.coordinates = item.matrixCoords;
                    comp.neighbours = item.neighbours;
                    comp.isObstacle = item.isObstacle;
                    comp.pathVisualizer = (target as PathVisualizer);

                    
                    
                    (target as PathVisualizer).possiblePlacedNewCoordsByNeighbours.Remove(item);
                    foreach (var nb in item.neighbours)
                    {
                        Undo.RecordObject(nb, "Adding neighbours to existing hexes");
                        
                        nb.neighbours.Add(comp);
                        PrefabUtility.RecordPrefabInstancePropertyModifications(nb);
                    }
            
                    list.Add(comp);
                    Debug.Log(list.Count);
                    pTarget.units = list;
                    return;
                    /*
                    var property = serializedObject.FindProperty("units");
                    //property.GetArrayElementAtIndex(20);
                    property.InsertArrayElementAtIndex(property.arraySize -1);
                    
                    Debug.Log(property.GetArrayElementAtIndex(property.arraySize - 1).propertyType);
                    Debug.Log (property.arraySize);
                    Debug.Log(item.matrixCoords);
                    pTarget.AddNewHexUnit(item);
                    serializedObject.Update();
                     */
                }
            }
            //Debug.Log(serializedObject.hasModifiedProperties);
            EditorUtility.SetDirty(pTarget);
            serializedObject.ApplyModifiedProperties();
        }
        
        
        public void AddNewHexUnit(GeneratedHexDataWrapper unit)
        {
            var newUnit = Instantiate((target as PathVisualizer).hexPrefab, unit.worldPos, Quaternion.identity, (target as PathVisualizer).transform);
            var comp = newUnit.GetComponent<HexUnit>();
            comp.coordinates = unit.matrixCoords;
            comp.neighbours = unit.neighbours;
            comp.isObstacle = unit.isObstacle;
            comp.pathVisualizer = (target as PathVisualizer);
            (target as PathVisualizer).possiblePlacedNewCoordsByNeighbours.Remove(unit);
            foreach (var item in unit.neighbours)
            {
                item.neighbours.Add(comp);
            }
            
            (target as PathVisualizer).units.Add(comp);
            //locker = false;
        }
    }
}
