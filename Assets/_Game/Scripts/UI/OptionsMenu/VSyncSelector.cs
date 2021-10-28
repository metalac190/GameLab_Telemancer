using UnityEngine;

public class VSyncSelector : OptionSelector
{
    public override void OnItemSelected(int item)
    {
        switch (item)
        {
            case 0:
                // Off
                QualitySettings.vSyncCount = 0;
                Debug.Log("VSync: Off");
                break;
            case 1: 
                // On
                QualitySettings.vSyncCount = 1;
                Debug.Log("VSync: On");
                break;
            default:
                return;
        }
        
        // Only change the player pref if the base case wasn't hit
        PlayerPrefs.SetFloat("VSync", item);
    }
}