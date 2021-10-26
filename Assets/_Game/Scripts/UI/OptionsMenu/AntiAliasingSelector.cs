using UnityEngine;

public class AntiAliasingSelector : OptionSelector
{
    public override void OnItemSelected(int item)
    {
        int antiAliasing = 0;
        
        switch (item)
        {
            case 0:
                // Off
                antiAliasing = 0;
                Debug.Log("Antialiasing: Off");
                break;
            case 1: 
                // 2x
                antiAliasing = 2;
                Debug.Log("Antialiasing: 2x");
                break;
            case 2: 
                // 4x
                antiAliasing = 4;
                Debug.Log("Antialiasing: 4x");
                break;
            case 3: 
                // 8x
                antiAliasing = 8;
                Debug.Log("Antialiasing: 8x");
                break;
            default:
                return;
        }
        
        // Only change the player pref if the base case wasn't hit
        PlayerPrefs.SetFloat("AntiAliasing", item);
        QualitySettings.antiAliasing = antiAliasing;
    }
}