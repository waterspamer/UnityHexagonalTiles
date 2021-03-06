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

        public Material defaultCylinderMat;
        public Material nonReachedCylinderMat;
        
        

        public Text lengthText;

        public Player player;

        public float singleMoveTime = .2f;

        public bool movingFlag = false;

        //public GameObject choosingCanvas;

        public UnityEvent onChooseEvent;

        public UnityEvent onStopMoveEvent;

        public UnityEvent onManualDoubleClickEvent;

        public UnityEvent onNoWayEvent;

        public ScrollAndPinch scrollAndPinch;

        public List<GeneratedHexDataWrapper> possiblePlacedNewCoordsByNeighbours;

        public GameObject hexPrefab;
        
        public GameObject highlight2;

        public GameObject highlightTrue;

        public GameObject highlightFalse;

        public Material ifWayExists;
        
        public Material ifWayNotExists;

        public Renderer hex;

        public float swipeDelta;

        public Slider slider;

        public Text sliderValue;

        public HexUnit trueStart;

        [HideInInspector] public HexUnit currentSelectedUnit;

        private Color _startColor;

        public bool isSwipingCamera => scrollAndPinch.swipeActive;

        public Vector2 lastCoord => scrollAndPinch.lastCoords;

        public Text mgn;


        public HexUnit lastMouseDownUnit;
        private void Awake()
        {
            _startColor = highLightMesh.GetComponent<Renderer>().sharedMaterial.color;
            if (!PlayerPrefs.HasKey("sensivity"))
            {
                swipeDelta = slider.value;
                PlayerPrefs.SetFloat("sensivity", slider.value);
                PlayerPrefs.Save();
            }
            else
            {
                swipeDelta = PlayerPrefs.GetFloat("sensivity");
                slider.value = swipeDelta;
                sliderValue.text = swipeDelta.ToString();
            }
            slider.onValueChanged.AddListener((value) =>
            {
                sliderValue.text = value.ToString();
                swipeDelta = value;
                PlayerPrefs.SetFloat("sensivity", slider.value);
                PlayerPrefs.Save();
            });
            possiblePlacedNewCoordsByNeighbours = new List<GeneratedHexDataWrapper>();
            Application.targetFrameRate = 60;

            if (!PlayerPrefs.HasKey("rootTileIndex"))
            {
                PlayerPrefs.SetInt("rootTileIndex", units.IndexOf(trueStart));
            }

            if (!PlayerPrefs.HasKey("lastTileIndex"))
            {
                PlayerPrefs.SetInt("lastTileIndex", units.IndexOf(a));
            }
            else
            {
                a = units[PlayerPrefs.GetInt("lastTileIndex")];
                a.SetCurrent();
                if (PlayerPrefs.GetInt("lastTileIndex") != PlayerPrefs.GetInt("rootTileIndex"))
                    trueStart.SetEmpty();
            }
            
        }



        private void Start()
        {
            //StartCoroutine(TestFindAllPaths());
        }

        private void OnApplicationQuit()
        {
            StopAllCoroutines();
            PlayerPrefs.Save();
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
            b.onSelectionInfo.RemoveAllListeners();
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
            
            if (!a.eventHappened)
                a.connectedEvent?.Invoke();
            if (a.connectedEvent.GetPersistentEventCount() != 0)
            {
                player.currentFood -= HexConfiguration.costByEvent[a.eventType];
                player.foodText.text = $"{player.currentFood}/{player.maxFood}";
                player.foodText.color = new Color(1f - ((float) player.currentFood / player.maxFood), ((float)player.currentFood / player.maxFood), 0);
                PlayerPrefs.SetInt("currentFood", player.currentFood);
                PlayerPrefs.Save();
            }
            PlayerPrefs.SetInt("lastTileIndex", units.IndexOf(a));
            PlayerPrefs.Save();
            a.eventHappened = true;
            a.SetPathNeeded(false);
            highlight2.GetComponent<Animator>().Play("HighlightDisappear");

            yield return new WaitForSeconds(.6f);
            
            movingFlag = false;
        }


        public int CalculatePathCost(List<HexUnit> units)
        {
            var cost = 0;

            for (int i = 1; i < units.Count; i++)
            {
                if (i == units.Count - 1 && b.connectedEvent.GetPersistentEventCount() != 0 && !b.eventHappened) continue;
                    cost += HexConfiguration.costByType[units[i].hexType];
            }
            return cost;
        }

        public Animator playerAnimator;
        public int pathLength;
        public void Move()
        {
            var path = FindPath(a, b);


            var pathCost = CalculatePathCost(path);
            
            if (path.Count == 0) {ResetAim();
                playerAnimator.Play("Elf_rider_female_Idle");
                Debug.Log("NOWAY"); onNoWayEvent?.Invoke();
                return;
            }
            
            
            if (pathCost + (b.connectedEvent.GetPersistentEventCount() != 0 && !b.eventHappened ? HexConfiguration.costByEvent[b.eventType] : 0) > player.currentFood)
            {
                //not enough food case
                ResetAim();
                playerAnimator.Play("Elf_rider_female_Idle");
                Debug.Log("!FOOD"); onNoWayEvent?.Invoke();
                return;
            }

            player.currentFood -= pathCost;
            player.foodText.text = $"{player.currentFood}/{player.maxFood}";
            player.foodText.color = new Color(1f - ((float) player.currentFood / player.maxFood), ((float)player.currentFood / player.maxFood), 0);
            PlayerPrefs.SetInt("currentFood", player.currentFood);
            PlayerPrefs.Save();
            
            playerAnimator.Play("Elf_rider_female_Run");
            StartCoroutine(MoveSet(path));
        }


        public Material empty;
        public Material hill;
        public Material forest;
        public Material swamp;

        
        public void SetAimToPosition(HexUnit unit)
        {
            b = unit;

            highlight2.transform.position = unit.transform.position + new Vector3(0, .12f, 0);
            highlight2.GetComponent<Animator>().Play("HighlightAppear");
            Debug.Log("Played HL Anim");
            if (b.connectedEvent.GetPersistentEventCount() != 0 && !movingFlag && !b.eventHappened)
            {
                Debug.Log(b.eventHappened);
                b.onSelectionInfo?.Invoke();
                Debug.Log("Invoked");
            }
            aimObject.GetComponent<AimScript>().targetPos = unit.transform.position + new Vector3(0, .168f, 0);
            var path = FindPath(a, b);

            var pathCost = CalculatePathCost(path);
            
            lengthText.color = path.Count == 0 ? Color.red : Color.yellow;
            /*
            var pathCost = b.connectedEvent.GetPersistentEventCount() != 0 && !b.eventHappened
                ? (path.Count - 1) * player.singleUnitPathCost + player.eventCost
                : (path.Count - 1) * player.singleUnitPathCost; */
            if (path.Count == 0)
            {
                lengthText.text = "NO";
            }
            else
            {
                lengthText.text = b.connectedEvent.GetPersistentEventCount()!=0 && !b.eventHappened ? $"{pathCost}+{HexConfiguration.costByEvent[b.eventType]}" : (pathCost).ToString();
               

                lengthText.color = pathCost > player.currentFood + HexConfiguration.costByEvent[b.eventType] ? Color.red : Color.yellow;
            }
            lRend.positionCount = path.Count;
            lRend.SetPositions(path.Select((x => x.transform.position  + new Vector3(0, .1f, 0))).Reverse().ToArray());


           
            
            for (int i = 1; i < units.Count; i++)
            {
                var cost = 0;
                var contains = path.Contains(units[i]);

                switch (units[i].hexType)
                {
                    case HexConfiguration.HexType.Empty:
                        units[i].outlineRound.GetComponent<MeshRenderer>().sharedMaterial = empty;
                        break;
                    case HexConfiguration.HexType.Hill:
                        units[i].outlineRound.GetComponent<MeshRenderer>().sharedMaterial = hill;
                        break;
                    case HexConfiguration.HexType.Forest:
                        units[i].outlineRound.GetComponent<MeshRenderer>().sharedMaterial = forest;
                        break;
                    case HexConfiguration.HexType.Swamp:
                        units[i].outlineRound.GetComponent<MeshRenderer>().sharedMaterial = swamp;
                        break;
                }
                    

                    if (contains)
                {
                    units[i].SetPathNeeded(true);

                    if (units[i] != path.Last())
                    {
                        cost += HexConfiguration.costByType[units[i].hexType];
                        var mat = highLightMesh.GetComponent<MeshRenderer>().sharedMaterial;
                        //units[i].SetReachable((cost <= player.currentFood));
                        
                        
                        //highLightMesh.GetComponent<Renderer>().sharedMaterial = (path.IndexOf(units[i])) * player.singleUnitPathCost <= player.currentFood ?ifWayExists: ifWayNotExists;
                    }


                    else
                    {
                        var t = units[i].connectedEvent.GetPersistentEventCount() != 0 && !units[i].eventHappened
                            ? 1
                            : 0;
                        //units[i].SetReachable(
                            //pathCost <= player.currentFood);
                        var mat = highLightMesh.GetComponent<MeshRenderer>().sharedMaterial;
                        //highLightMesh.GetComponent<Renderer>().material.color =pathCost <= player.currentFood ? _startColor: Color.red;
                        //highLightMesh.GetComponent<MeshRenderer>().sharedMaterial.SetColor("_Main", pathCost <= player.currentFood ? _startColor : Color.red);
                        //highLightMesh.GetComponent<Renderer>().sharedMaterial = pathCost <= player.currentFood ? ifWayExists: ifWayNotExists;
                        
                        highlightFalse.SetActive(!(pathCost + HexConfiguration.costByEvent[b.eventType] <= player.currentFood));
                        highlightTrue.SetActive(pathCost + HexConfiguration.costByEvent[b.eventType] <= player.currentFood);
                        
                        
                        Debug.Log(highLightMesh.GetComponent<Renderer>().material.name);
                    }
                        

                }
                
            }
            
            //hex.material = path.Count == 0 ? ifWayNotExists : ifWayExists;
            
            foreach (var unitObj in units)
            {
                unitObj.SetPathNeeded(path.Contains(unitObj));
            }
            
        }
        public GameObject highLightMesh;
        public void ResetAim()
        {
            //aimObject.GetComponent<AimScript>().targetPos = a.transform.position +new Vector3(0, .168f, 0);
            lRend.positionCount = 0;
            foreach (var unitObj in units)
            {
                
                unitObj.SetPathNeeded(false);
            }

            currentSelectedUnit = null;
            highlight2.GetComponent<Animator>().Play("HighlightDisappear");
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
                if (adjacentTile == null) Debug.LogWarning(adjacentTile);
                    if (adjacentTile.isObstacle || adjacentTile.connectedEvent.GetPersistentEventCount() != 0) //Second condition if hex has any "not null" interactions 
                    {
                        if (adjacentTile != endPoint && !adjacentTile.eventHappened) 
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
