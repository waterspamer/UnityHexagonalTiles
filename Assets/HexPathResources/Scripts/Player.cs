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
            singleMoveCostSlider.onValueChanged.AddListener((value)=>
            {
                singleUnitPathCost = (int) value;
                singleMoveCostText.text = $"Single move cost : {singleUnitPathCost}";

            });
            maxFoodSlider.onValueChanged.AddListener((value)=>
            {
                maxFood = (int) value;
                if (currentFood > maxFood)
                    currentFood = maxFood;

                foodText.text = $"{currentFood}/{maxFood}";
                foodText.color = new Color(1f - ((float) currentFood / maxFood), ((float)currentFood / maxFood), 0);
                maxFoodText.text = $"Max food count : {maxFood}";
            });
            eventCostSlider.onValueChanged.AddListener((value)=>
            {
                eventCost = (int) value;
                eventCostText.text = $"Interaction cost : {eventCost}";
            });

            
        }


        //public void Change
        
        private void FixedUpdate()
        {
            transform.position = Vector3.Lerp(transform.position, targetUnit.transform.position, Time.deltaTime * 5f);
        }
    }
}
