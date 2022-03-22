using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace HexPathResources.Scripts.DataStructs
{
    public class HexUnit : MonoBehaviour
    {
        
        
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
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.red; 
            Handles.Label(transform.position, coordinates.ToString(), style);
            
            for (int i =0; i < 6; i++ )
            {
                //if (pathVisualizer.units.Contains())
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

        
        #if UNITY_EDITOR
        private void OnMouseDown()
        {
            if (!isObstacle && !pathVisualizer.movingFlag && !IsPointerOverUIObject() && !pathVisualizer.isSwipingCamera && !_isSelected)
            {
                if (!isObstacle && !pathVisualizer.movingFlag)
                    pathVisualizer.SetAimToPosition(this);
                pathVisualizer.b = this;
                pathVisualizer.SetDecisionMode();
                _isSelected = true;

                /*
                Debug.Log("FindPath");
                
                pathVisualizer.FindPath();
                pathVisualizer.Move();*/
            }
            
            else if (!isObstacle && !pathVisualizer.movingFlag && !IsPointerOverUIObject() &&
                     !pathVisualizer.isSwipingCamera && _isSelected)
            {
                pathVisualizer.onManualDoubleClickEvent?.Invoke();
                _isSelected = false;
            }
            
        }
        #endif

        private void OnMouseEnter()
        {
            if (!isObstacle&& !IsPointerOverUIObject() )
                GetComponent<Renderer>().material.color = Color.yellow;
        }
        
        #if UNITY_EDITOR
        private void OnMouseExit()
        {
            if (!isObstacle)
                GetComponent<Renderer>().material.color = Color.white;
            
        }
        #endif

        #if UNITY_ANDROID || UNITY_IOS
        private void OnMouseUp()
        {
            if (!isObstacle && !pathVisualizer.isSwipingCamera)
                GetComponent<Renderer>().material.color = Color.white;
            
            if (!isObstacle && !pathVisualizer.movingFlag && !IsPointerOverUIObject() && pathVisualizer.isSwipingCamera && !_isSelected)
            {
                if (!isObstacle && !pathVisualizer.movingFlag)
                    pathVisualizer.SetAimToPosition(this);
                pathVisualizer.b = this;
                pathVisualizer.SetDecisionMode();
                _isSelected = true;
                /*
                Debug.Log("FindPath");
                
                pathVisualizer.FindPath();
                pathVisualizer.Move();*/
            }
            
            else if (!isObstacle && !pathVisualizer.movingFlag && !IsPointerOverUIObject() &&
                     pathVisualizer.isSwipingCamera && _isSelected)
            {
                pathVisualizer.onManualDoubleClickEvent?.Invoke();
                _isSelected = false;
            }
        }
        #endif

        [CanBeNull] public HexUnit TryGetNeighbourUnit()
        {
            return null;
        }
    }
}
