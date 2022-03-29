using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR;
using static HexPathResources.Scripts.DataStructs.SingleMoveDirection;

namespace HexPathResources.Scripts.DataStructs
{
    [Serializable]
    public class HexUnit : MonoBehaviour
    {

        public UnityEvent connectedEvent;
        public UnityEvent onSelectionInfo;
        
        public bool isObstacle = false;

        public Material space;
        public Material obstacle;
        public Material currentPos;

        public Vector3Int coordinates;

        private bool _isSelected;

        
        
        //TODO static class or Singleton
        public PathVisualizer pathVisualizer;

        public GameObject outlineRound;
        
        public int g;
        public int h;
        public int F => g + h;


        public UnityEvent onWalkEvent;

        
        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            
            //Gizmos.color = Color.red;
            //if (isObstacle) Gizmos.DrawSphere(transform.position, 1f);
            //f (connectedEvent.GetPersistentEventCount() != 0) Handles.DrawSolidDisc(transform.position, Vector3.up, .4f);
            if (connectedEvent.GetPersistentEventCount() == 3 && connectedEvent.GetPersistentTarget(2) == null) Gizmos.DrawCube(transform.position, Vector3.one);
            //Handles.Label(transform.position, coordinates.ToString());
            if (pathVisualizer == null || !pathVisualizer.editMode) return;

            
            Handles.color = new Color( Mathf.Pow(6 - (float) neighbours.Count / 6f, 2f), (float) neighbours.Count / 6f, 0f);
            Handles.DrawSolidDisc(transform.position, Vector3.up, .4f);
            var dict = pathVisualizer.units.ToDictionary(keySelector: unit => unit.coordinates);
            for (int i =0; i < 6; i++ )
            {
                if (dict.ContainsKey(coordinates.GetNeighbourMatrixCoordinateByDirection((Direction) i)))
                {
                    if (!isObstacle)
                    {
                        Handles.DrawLine(transform.position, transform.position + (this.NeighbourGlobalCoordinateByDirection((Direction)i) - transform.position)/2, 2f);
                    }
                    else
                    {
                        Handles.color = Color.yellow;
                        Handles.DrawLine(transform.position, transform.position + (this.NeighbourGlobalCoordinateByDirection((Direction)i) - transform.position)/2, 2f);
                    }
                }
                else {
                    Handles.color = Color.white;
                    Handles.DrawLine(transform.position, transform.position + (this.NeighbourGlobalCoordinateByDirection((Direction)i) - transform.position)/2, 2f);
                    Handles.DrawSolidDisc(transform.position + (this.NeighbourGlobalCoordinateByDirection((Direction)i) - transform.position)/2f +new Vector3(0,.01f, 0), Vector3.up, .1f);
                    
                    var govno = pathVisualizer.possiblePlacedNewCoordsByNeighbours.FirstOrDefault(hUnit =>
                        hUnit.matrixCoords == coordinates.GetNeighbourMatrixCoordinateByDirection((Direction) i));
                    if (govno!= null)
                    {
                        if (!govno.neighbours.Contains(this))
                            govno.neighbours.Add(this);

                        foreach (var item in govno.neighbours)
                        {
                            Gizmos.DrawLine(govno.worldPos, item.transform.position);
                        }
                    }
                    else
                        pathVisualizer.possiblePlacedNewCoordsByNeighbours.Add(new GeneratedHexDataWrapper(
                            coordinates.GetNeighbourMatrixCoordinateByDirection((Direction) i), new  []{this}.ToList(),
                            this.NeighbourGlobalCoordinateByDirection((Direction) i), false));
                }
            }
               

        }
        #endif

        public void SetPathNeeded(bool required) => outlineRound.SetActive(required);

        public void SetCurrent()
        {
            GetComponent<Renderer>().material = currentPos;
        }

        public void SetEmpty()
        {
            GetComponent<Renderer>().material = space;

        }

       

#if UNITY_EDITOR
        private void OnValidate()
        {
            GetComponent<Renderer>().material = isObstacle ? obstacle : space;
        }
        #endif

        public List<HexUnit> neighbours;
        //       0
        //     -----
        //   5/     \1
        //   4\     /2
        //     -----
        //       3
        
        
        private bool IsPointerOverUIObject() {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }

        private int _tapCount;
        
        #if UNITY_EDITOR
        private void OnMouseDown()
        {
            
            //_tapCount++;
            if (this == pathVisualizer.a) return;
            if (!isObstacle && !pathVisualizer.movingFlag && !IsPointerOverUIObject() && !pathVisualizer.isSwipingCamera)
            {
                if (pathVisualizer.currentSelectedUnit != this )
                    pathVisualizer.SetAimToPosition(this);
                pathVisualizer.b = this;
                pathVisualizer.SetDecisionMode();
                _isSelected = true;
            }
            
            if (!isObstacle && !pathVisualizer.movingFlag && !IsPointerOverUIObject() &&
                     !pathVisualizer.isSwipingCamera && pathVisualizer.currentSelectedUnit == this)
            {
                
                pathVisualizer.onManualDoubleClickEvent?.Invoke();
                _isSelected = false;
                return;
            }
            
            pathVisualizer.currentSelectedUnit = this;
        }
        #endif
        public bool eventHappened;
        /*
        private void OnMouseEnter()
        {
            if (!isObstacle&& !IsPointerOverUIObject() )
                GetComponent<Renderer>().material.color = Color.yellow;
        }*/
        
        #if UNITY_EDITOR
        private void OnMouseExit()
        {
            if (!isObstacle)
                GetComponent<Renderer>().material.color = Color.white;
            
        }
        #endif

        #if (UNITY_ANDROID || UNITY_IOS)&& !UNITY_EDITOR
        private void OnMouseUp()
        {
            if (pathVisualizer.movingFlag) return;
            if (!isObstacle && !pathVisualizer.movingFlag && !IsPointerOverUIObject() && pathVisualizer.isSwipingCamera)
            {
                if (pathVisualizer.currentSelectedUnit != this)
                    pathVisualizer.SetAimToPosition(this);
                pathVisualizer.b = this;
                pathVisualizer.SetDecisionMode();
                _isSelected = true;
            }
            
            if (!isObstacle && !pathVisualizer.movingFlag && !IsPointerOverUIObject() &&
                     pathVisualizer.isSwipingCamera && pathVisualizer.currentSelectedUnit == this)
            {
                
                pathVisualizer.onManualDoubleClickEvent?.Invoke();
                _isSelected = false;
                return;
            }
            pathVisualizer.currentSelectedUnit = this;
        }
        #endif

        [CanBeNull] public HexUnit TryGetNeighbourUnit()
        {
            return null;
        }
    }
}
