using System;
using HexPathResources.Scripts.DataStructs;
using UnityEngine;
using UnityEngine.UI;

namespace HexPathResources.Scripts
{
    public class Player : MonoBehaviour
    {
        public bool isWalking;

        public PathVisualizer pathVisualizer;

        
        [Header("Hex type settings")]

        
        public Slider emptyMoveCostSlider;
        public Slider hillMoveCostSlider;
        public Slider forestMoveCostSlider;
        public Slider swampMoveCostSlider;

        [Header("Event type settings")] 
        public Slider cheapEventSlider;
        public Slider mediumEventSlider;
        public Slider expensiveEventSlider;

        
        
        
        public Slider maxFoodSlider;

        public InputField maxFoodField;
        
        public Slider eventCostSlider;

        public Text emptyMoveCostText;
        public Text hillMoveCostText;
        public Text forestMoveCostText;
        public Text swampMoveCostText;


        public Text cheapEventText;
        public Text mediumEventText;
        public Text expensiveEventText;

        
        
        public Text maxFoodText;
        public Text eventCostText;


        public CameraController cameraController;
        

        public Text foodText;

        public HexUnit targetUnit;

        public int maxFood = 100;

        public int currentFood = 20;

        public int singleUnitPathCost;

        public int eventCost = 1;


        public void Walk(int pathLength)
        {
            currentFood -= pathLength * singleUnitPathCost;
        }

        public void ResetFood()
        {
            currentFood = maxFood;
            foodText.text = $"{currentFood}/{maxFood}";
            foodText.color = new Color(1f - ((float) currentFood / maxFood), ((float)currentFood / maxFood), 0);
        }

        private void Awake()
        {

            if (!PlayerPrefs.HasKey("currentFood"))
            {
                PlayerPrefs.SetInt("currentFood", currentFood);
                PlayerPrefs.Save();
            }
            else
            {
                currentFood = PlayerPrefs.GetInt("currentFood");
                if (currentFood == 0) currentFood = maxFood;
            }
            
            if (!PlayerPrefs.HasKey("maxFood"))
            {
                maxFood = Convert.ToInt32(maxFoodField.text);
                PlayerPrefs.SetFloat("maxFood", Convert.ToInt32(maxFoodField.text));
                PlayerPrefs.Save();
            }
            else
            {
                maxFood = (int)PlayerPrefs.GetFloat("maxFood");
                maxFoodField.text = maxFood.ToString();
                maxFoodText.text = $"Max food count : {maxFood}";
                foodText.text = $"{currentFood}/{maxFood}";
                foodText.color = new Color(1f - ((float) currentFood / maxFood), ((float)currentFood / maxFood), 0);
            }
            
            if (!PlayerPrefs.HasKey("eventCost"))
            {
                eventCost = (int)eventCostSlider.value;
                PlayerPrefs.SetFloat("eventCost", eventCostSlider.value);
                PlayerPrefs.Save();
            }
            
            else
            {
                eventCost = (int)PlayerPrefs.GetFloat("eventCost");
                eventCostSlider.value = eventCost;
                eventCostText.text = $"Interaction cost : {eventCost}";
            }
            
            
            //empty
            if (!PlayerPrefs.HasKey("singleMoveCost"))
            {
                HexConfiguration.costByType[HexConfiguration.HexType.Empty]= (int)emptyMoveCostSlider.value;
                PlayerPrefs.SetFloat("singleMoveCost", emptyMoveCostSlider.value);
                PlayerPrefs.Save();
            }
            else
            {
                HexConfiguration.costByType[HexConfiguration.HexType.Empty] = (int)PlayerPrefs.GetFloat("singleMoveCost");
                emptyMoveCostSlider.value = HexConfiguration.costByType[HexConfiguration.HexType.Empty];
                emptyMoveCostText.text =  $"Empty move cost : {HexConfiguration.costByType[HexConfiguration.HexType.Empty]}";
            }
            //hill
            if (!PlayerPrefs.HasKey("hillMoveCost"))
            {
                HexConfiguration.costByType[HexConfiguration.HexType.Hill] = (int)hillMoveCostSlider.value;
                PlayerPrefs.SetFloat("hillMoveCost", hillMoveCostSlider.value);
                PlayerPrefs.Save();
            }
            else
            {
                HexConfiguration.costByType[HexConfiguration.HexType.Hill] = (int)PlayerPrefs.GetFloat("hillMoveCost");
                hillMoveCostSlider.value = HexConfiguration.costByType[HexConfiguration.HexType.Hill];
                hillMoveCostText.text =  $"Hill move cost : {HexConfiguration.costByType[HexConfiguration.HexType.Hill]}";
            }
            //forest
            if (!PlayerPrefs.HasKey("forestMoveCost"))
            {
                HexConfiguration.costByType[HexConfiguration.HexType.Forest] = (int)forestMoveCostSlider.value;
                PlayerPrefs.SetFloat("singleMoveCost", forestMoveCostSlider.value);
                PlayerPrefs.Save();
            }
            else
            {
                HexConfiguration.costByType[HexConfiguration.HexType.Forest] = (int)PlayerPrefs.GetFloat("forestMoveCost");
                forestMoveCostSlider.value = HexConfiguration.costByType[HexConfiguration.HexType.Forest];
                forestMoveCostText.text =  $"Forest move cost : {HexConfiguration.costByType[HexConfiguration.HexType.Forest]}";
            }
            //swamp
            if (!PlayerPrefs.HasKey("swampMoveCost"))
            {
                HexConfiguration.costByType[HexConfiguration.HexType.Swamp] = (int)swampMoveCostSlider.value;
                PlayerPrefs.SetFloat("swampMoveCost", swampMoveCostSlider.value);
                PlayerPrefs.Save();
            }
            else
            {
                HexConfiguration.costByType[HexConfiguration.HexType.Swamp] = (int)PlayerPrefs.GetFloat("swampMoveCost");
                swampMoveCostSlider.value = HexConfiguration.costByType[HexConfiguration.HexType.Swamp];
                swampMoveCostText.text =  $"Swamp move cost : {HexConfiguration.costByType[HexConfiguration.HexType.Swamp]}";
            }
            
            //cheapEvent
            if (!PlayerPrefs.HasKey("cheapEventCost"))
            {
                HexConfiguration.costByEvent[HexConfiguration.EventType.Cheap] = (int)cheapEventSlider.value;
                PlayerPrefs.SetFloat("cheapEventCost", HexConfiguration.costByEvent[HexConfiguration.EventType.Cheap]);
                PlayerPrefs.Save();
            }
            else
            {
                HexConfiguration.costByEvent[HexConfiguration.EventType.Cheap] = (int)PlayerPrefs.GetFloat("cheapEventCost");
                cheapEventSlider.value = HexConfiguration.costByEvent[HexConfiguration.EventType.Cheap];
                cheapEventText.text =  $"Cheap event : {HexConfiguration.costByEvent[HexConfiguration.EventType.Cheap]}";
            }
            
            //mediumEvent
            if (!PlayerPrefs.HasKey("mediumEventCost"))
            {
                HexConfiguration.costByEvent[HexConfiguration.EventType.Medium] = (int)mediumEventSlider.value;
                PlayerPrefs.SetFloat("mediumEventCost", HexConfiguration.costByEvent[HexConfiguration.EventType.Medium]);
                PlayerPrefs.Save();
            }
            else
            {
                HexConfiguration.costByEvent[HexConfiguration.EventType.Medium] = (int)PlayerPrefs.GetFloat("mediumEventCost");
                mediumEventSlider.value = HexConfiguration.costByEvent[HexConfiguration.EventType.Medium];
                mediumEventText.text =  $"Medium event : {HexConfiguration.costByEvent[HexConfiguration.EventType.Medium]}";
            }
            //expensiveEvent
            if (!PlayerPrefs.HasKey("expensiveEventCost"))
            {
                HexConfiguration.costByEvent[HexConfiguration.EventType.Expensive] = (int)expensiveEventSlider.value;
                PlayerPrefs.SetFloat("expensiveEventCost", HexConfiguration.costByEvent[HexConfiguration.EventType.Expensive]);
                PlayerPrefs.Save();
            }
            else
            {
                HexConfiguration.costByEvent[HexConfiguration.EventType.Expensive] = (int)PlayerPrefs.GetFloat("expensiveEventCost");
                expensiveEventSlider.value = HexConfiguration.costByEvent[HexConfiguration.EventType.Expensive];
                expensiveEventText.text =  $"Expensive event : {HexConfiguration.costByEvent[HexConfiguration.EventType.Expensive]}";
            }
            
            
            
            
            emptyMoveCostSlider.onValueChanged.AddListener((value)=>
            {
                HexConfiguration.costByType[HexConfiguration.HexType.Empty] = (int) value;
                emptyMoveCostText.text = $"Empty move cost : {HexConfiguration.costByType[HexConfiguration.HexType.Empty]}";
                PlayerPrefs.SetFloat("singleMoveCost", emptyMoveCostSlider.value);
                PlayerPrefs.Save();
            });
            
            hillMoveCostSlider.onValueChanged.AddListener((value)=>
            {
                HexConfiguration.costByType[HexConfiguration.HexType.Hill] = (int) value;
                hillMoveCostText.text = $"Hill move cost : {HexConfiguration.costByType[HexConfiguration.HexType.Hill]}";
                PlayerPrefs.SetFloat("hillMoveCost", hillMoveCostSlider.value);
                PlayerPrefs.Save();
            });
            
            forestMoveCostSlider.onValueChanged.AddListener((value)=>
            {
                HexConfiguration.costByType[HexConfiguration.HexType.Forest] = (int) value;
                forestMoveCostText.text = $"Forest move cost : {HexConfiguration.costByType[HexConfiguration.HexType.Forest]}";
                PlayerPrefs.SetFloat("forestMoveCost", forestMoveCostSlider.value);
                PlayerPrefs.Save();
            });
            
            swampMoveCostSlider.onValueChanged.AddListener((value)=>
            {
                HexConfiguration.costByType[HexConfiguration.HexType.Swamp] = (int) value;
                swampMoveCostText.text = $"Swamp move cost : {HexConfiguration.costByType[HexConfiguration.HexType.Swamp]}";
                PlayerPrefs.SetFloat("swampMoveCost", swampMoveCostSlider.value);
                PlayerPrefs.Save();
            });
            
            cheapEventSlider.onValueChanged.AddListener((value)=>
            {
                HexConfiguration.costByEvent[HexConfiguration.EventType.Cheap] = (int) value;
                cheapEventText.text = $"Cheap event : {HexConfiguration.costByEvent[HexConfiguration.EventType.Cheap]}";
                PlayerPrefs.SetFloat("cheapEventCost", cheapEventSlider.value);
                PlayerPrefs.Save();
            });
            
            mediumEventSlider.onValueChanged.AddListener((value)=>
            {
                HexConfiguration.costByEvent[HexConfiguration.EventType.Medium] = (int) value;
                mediumEventText.text = $"Medium event : {HexConfiguration.costByEvent[HexConfiguration.EventType.Medium]}";
                PlayerPrefs.SetFloat("mediumEventCost", mediumEventSlider.value);
                PlayerPrefs.Save();
            });
            
            expensiveEventSlider.onValueChanged.AddListener((value)=>
            {
                HexConfiguration.costByEvent[HexConfiguration.EventType.Expensive] = (int) value;
                expensiveEventText.text = $"Expensive event : {HexConfiguration.costByEvent[HexConfiguration.EventType.Expensive]}";
                PlayerPrefs.SetFloat("expensiveEventCost", expensiveEventSlider.value);
                PlayerPrefs.Save();
            });
            
            
            
            
            maxFoodField.onValueChanged.AddListener((value)=>
            {
                maxFood = Convert.ToInt32(value);
                if (currentFood > maxFood)
                    currentFood = maxFood;

                foodText.text = $"{currentFood}/{maxFood}";
                foodText.color = new Color(1f - ((float) currentFood / maxFood), ((float)currentFood / maxFood), 0);
                maxFoodText.text = $"Max food count : {maxFood}";
                PlayerPrefs.SetFloat("maxFood", Convert.ToSingle(maxFoodField.text));
                PlayerPrefs.Save();
            });
            eventCostSlider.onValueChanged.AddListener((value)=>
            {
                eventCost = (int) value;
                eventCostText.text = $"Interaction cost : {eventCost}";
                PlayerPrefs.SetFloat("eventCost", eventCostSlider.value);
                PlayerPrefs.Save();
            });

            if (PlayerPrefs.HasKey("lastTileIndex"))
            {
                targetUnit = pathVisualizer.units[PlayerPrefs.GetInt("lastTileIndex")];
                transform.position = pathVisualizer.units[PlayerPrefs.GetInt("lastTileIndex")].transform.position;
                cameraController.transform.position = transform.position + new Vector3(-20, 24.3f, -11.6f);

            }
        }

        private void OnApplicationQuit()
        {
            PlayerPrefs.Save();
        }
        
        

        //public void Change
        
        private void FixedUpdate()
        {
            transform.position = Vector3.Lerp(transform.position, targetUnit.transform.position, Time.deltaTime * 5f);
        }
    }
}
