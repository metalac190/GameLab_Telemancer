using UnityEngine;

public class FullscreenSelector : OptionSelector
{
    public override void OnItemSelected(int item)
    {
        switch (item)
        {
            case 0:
                // Off
                Screen.fullScreen = false;
                Debug.Log("Fullscreen: Off");
                break;
            case 1: 
                // On
                Screen.fullScreen = true;
                Debug.Log("Fullscreen: On");
                break;
            default:
                return;
        }
        
        // Only change the player pref if the base case wasn't hit
        PlayerPrefs.SetFloat("Fullscreen", item);
    }
}