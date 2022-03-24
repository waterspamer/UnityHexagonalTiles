using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HexPathResources.Scripts.DataStructs;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HexPathResources.Scripts
{
    [ExecuteInEditMode][System.Serializable]
    
    public class PathVisualizer : MonoBehaviour
    {
        [Range(0, 1)] public int controlType = 0;

        public bool editMode = true;

        public HexUnit a;
        public HexUnit b;

        public GameObject aimObject;

        public LineRenderer lRend;
        
        public List<HexUnit> m_Tst;

        [SerializeField] public List<HexUnit> units;
        

        public Text lengthText;

        public Player player;

        public float singleMoveTime = .2f;

        public bool movingFlag = false;

        //public GameObject choosingCanvas;

        public UnityEvent onChooseEvent;

        public UnityEvent onStopMoveEvent;

        public UnityEvent onManualDoubleClickEvent;

        public ScrollAndPinch scrollAndPinch;

        public List<GeneratedHexDataWrapper> possiblePlacedNewCoordsByNeighbours;

        public GameObject hexPrefab;

        [HideInInspector] public HexUnit currentSelectedUnit;

        public bool isSwipingCamera => scrollAndPinch.swipeActive;
        

        
        private void Awake()
        {
            possiblePlacedNewCoordsByNeighbours = new List<GeneratedHexDataWrapper>();
            Application.targetFrameRate = 60;
            a.SetCurrent();
        }

        private void Start()
        {
            //StartCoroutine(TestFindAllPaths());
        }

        private void OnApplicationQuit()
        {
            StopAllCoroutines();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!editMode) return;
            int i = 0;
            if (possiblePlacedNewCoordsByNeighbours == null)
                possiblePlacedNewCoordsByNeighbours = new List<GeneratedHexDataWrapper>();
            else 
                foreach (var item in possiblePlacedNewCoordsByNeighbours)
                {
                    Handles.DrawSolidDisc(item.worldPos, Vector3.up, .4f);
                    Handles.Label(item.worldPos, i.ToString());
                    i++;
                }
            
            //Debug.Log(possiblePlacedNewCoordsByNeighbours.Count);
        }
        //[SerializeField]
        #endif
        public bool locker = false;
        
        public void AddNewHexUnit(GeneratedHexDataWrapper unit)
        {
           //EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            //locker = true;
            //EditorUtility.SetDirty(this);
            var newUnit = Instantiate(hexPrefab, unit.worldPos, Quaternion.identity, this.transform);
            var comp = newUnit.GetComponent<HexUnit>();
            comp.coordinates = unit.matrixCoords;
            comp.neighbours = unit.neighbours;
            comp.isObstacle = unit.isObstacle;
            comp.pathVisualizer = this;
            possiblePlacedNewCoordsByNeighbours.Remove(unit);
            foreach (var item in unit.neighbours)
            {
                item.neighbours.Add(comp);
            }
            //Undo.RecordObject(comp);
            units.Add(comp);
            //locker = false;
        }


        public void SetDecisionMode()
        {
            onChooseEvent?.Invoke();
        }

        IEnumerator TestFindAllPaths()
        {
            foreach (var hex in units)
            {
                if (!hex.isObstacle)
                {
                    SetAimToPosition(hex);
                    if (FindPath(a, hex).Count == -1)
                        Debug.Log("fault");
                }
                    
                yield return new WaitForSeconds(.016f);
            }
        }
        
        public IEnumerator MoveSet(List<HexUnit> hexUnits)
        {
            
            Debug.Log(hexUnits.Count);
            movingFlag = true;
            a.SetEmpty();
            player.targetUnit = hexUnits[1];
            player.transform.LookAt(hexUnits[1].transform);
            a.SetPathNeeded(false);
            lRend.positionCount--;
            yield return new WaitForSeconds(singleMoveTime);
            for (int i = 2; i < hexUnits.Count; i++)
            {
                hexUnits[i-1].SetPathNeeded(false);
                lRend.positionCount--;
                player.targetUnit = hexUnits[i];
                player.transform.LookAt(hexUnits[i].transform);
                yield return new WaitForSeconds(singleMoveTime);
                //hexUnits[i-1].SetPathNeeded(false);
                
            }
            hexUnits.Last().SetCurrent();
            a = hexUnits.Last();
            onStopMoveEvent?.Invoke();
            movingFlag = false;
            a.connectedEvent?.Invoke();
        }

        


        public void Move()
        {
            
            StartCoroutine(MoveSet(FindPath(a, b)));
        }
        
        public void SetAimToPosition(HexUnit unit)
        {
            b = unit;
            
            aimObject.GetComponent<AimScript>().targetPos = unit.transform.position + new Vector3(0, .168f, 0);
            var path = FindPath(a, b);
            lengthText.text = (path.Count - 1).ToString();
            lRend.positionCount = path.Count;
            lRend.SetPositions(path.Select((x => x.transform.position  + new Vector3(0, .06f, 0))).Reverse().ToArray());
            
            foreach (var unitObj in units)
            {
                unitObj.SetPathNeeded(path.Contains(unitObj));
            }
        }
        

        public void FindPath()
        {
            
            

            //StartCoroutine(Visualize(path));

        }


        IEnumerator Visualize(List<HexUnit> units)
        {
            units[0].SetEmpty();
            foreach (var unit in units)
            {
                //unit.onWalkEvent?.Invoke();
                unit.SetCurrent();
                yield return new WaitForSeconds(.1f);
                unit.SetEmpty();
            }

            a = b;
            b.SetCurrent();
            yield break;
        }


        public  List<HexUnit> FindPath(HexUnit startPoint, HexUnit endPoint)
    {

        List<HexUnit> openPathTiles = new List<HexUnit>();    
        List<HexUnit> closedPathTiles = new List<HexUnit>();
        
        HexUnit currentTile = startPoint;

        currentTile.g = 0;
        currentTile.h = GetEstimatedPathCost(startPoint.coordinates, endPoint.coordinates);
        
        openPathTiles.Add(currentTile);

        while (openPathTiles.Count != 0)
        {
            openPathTiles = openPathTiles.OrderBy(x => x.F).ThenByDescending(x => x.g).ToList();
            currentTile = openPathTiles[0];
            openPathTiles.Remove(currentTile);
            closedPathTiles.Add(currentTile);

            int g = currentTile.g + 1;

            if (closedPathTiles.Contains(endPoint))
            {
                break;
            }
            foreach (HexUnit adjacentTile in currentTile.neighbours)
            {
                if (adjacentTile.isObstacle || adjacentTile.connectedEvent.GetPersistentEventCount() != 0) //Second condition if hex has any "not null" interactions 
                {
                    if (adjacentTile != endPoint) 
                        continue;
                }
                if (closedPathTiles.Contains(adjacentTile))
                {
                    continue;
                }

                if (!(openPathTiles.Contains(adjacentTile)))
                {
                    adjacentTile.g = g;
                    adjacentTile.h = GetEstimatedPathCost(adjacentTile.coordinates, endPoint.coordinates);
                    openPathTiles.Add(adjacentTile);
                }
                else if (adjacentTile.F > g + adjacentTile.h)
                {
                    adjacentTile.g = g;
                }
            }
        }

        List<HexUnit> finalPathTiles = new List<HexUnit>();

        if (closedPathTiles.Contains(endPoint))
        {
            currentTile = endPoint;
            finalPathTiles.Add(currentTile);

            for (int i = endPoint.g - 1; i >= 0; i--)
            {
                currentTile = closedPathTiles.Find(x => x.g == i && currentTile.neighbours.Contains(x));
                finalPathTiles.Add(currentTile);
            }

            finalPathTiles.Reverse();
        }

        return finalPathTiles;
    }
        
        
        protected static int GetEstimatedPathCost(Vector3Int startPosition, Vector3Int targetPosition)
        {
            return Mathf.Max(Mathf.Abs(startPosition.z - targetPosition.z), Mathf.Max(Mathf.Abs(startPosition.x - targetPosition.x), Mathf.Abs(startPosition.y - targetPosition.y)));
        }

    }
}
