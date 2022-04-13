using HexPathResources.Scripts.DataStructs;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HexPathResources.Scripts
{
    public class SceneLoader : MonoBehaviour
    {
        public PathVisualizer pathVisualizer;
        public Player player;

        public void LoadScene(string name)
        {
            
            PlayerPrefs.SetFloat("currentFood", player.currentFood);
            PlayerPrefs.SetInt("lastTileIndex", pathVisualizer.units.IndexOf(pathVisualizer.trueStart));
            SceneManager.LoadScene(name);
            
            
            
        }
    }
}
