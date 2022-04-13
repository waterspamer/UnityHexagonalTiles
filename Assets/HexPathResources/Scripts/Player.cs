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

        public Slider singleMoveCostSlider;
        public Slider maxFoodSlider;
        public Slider eventCostSlider;

        public Text singleMoveCostText;
        public Text maxFoodText;
        public Text eventCostText;


        public CameraController cameraController;
        

        public Text foodText;

        public HexUnit targetUnit;

        public int maxFood = 20;

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
                
            }
            
            if (!PlayerPrefs.HasKey("maxFood"))
            {
                maxFood = (int)maxFoodSlider.value;
                PlayerPrefs.SetFloat("maxFood", maxFoodSlider.value);
                PlayerPrefs.Save();
            }
            else
            {
                maxFood = (int)PlayerPrefs.GetFloat("maxFood");
                maxFoodSlider.value = maxFood;
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
            
            if (!PlayerPrefs.HasKey("singleMoveCost"))
            {
                singleUnitPathCost = (int)singleMoveCostSlider.value;
                PlayerPrefs.SetFloat("singleMoveCost", singleMoveCostSlider.value);
                PlayerPrefs.Save();
            }
            else
            {
                singleUnitPathCost = (int)PlayerPrefs.GetFloat("singleMoveCost");
                singleMoveCostSlider.value = singleUnitPathCost;
                singleMoveCostText.text =  $"Single move cost : {singleUnitPathCost}";
            }
            
            
            
            
            singleMoveCostSlider.onValueChanged.AddListener((value)=>
            {
                singleUnitPathCost = (int) value;
                singleMoveCostText.text = $"Single move cost : {singleUnitPathCost}";
                PlayerPrefs.SetFloat("singleMoveCost", singleMoveCostSlider.value);
                PlayerPrefs.Save();
            });
            maxFoodSlider.onValueChanged.AddListener((value)=>
            {
                maxFood = (int) value;
                if (currentFood > maxFood)
                    currentFood = maxFood;

                foodText.text = $"{currentFood}/{maxFood}";
                foodText.color = new Color(1f - ((float) currentFood / maxFood), ((float)currentFood / maxFood), 0);
                maxFoodText.text = $"Max food count : {maxFood}";
                PlayerPrefs.SetFloat("maxFood", maxFoodSlider.value);
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
