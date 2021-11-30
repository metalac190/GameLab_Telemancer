using UnityEngine;

public class VisualsSelector : OptionSelector
{
    public delegate void VisualsChanged();
    public static event VisualsChanged OnVisualsChanged;

    public override void OnItemSelected(int item)
    {
        switch (item)
        {
            case 0:
                // Disabled
                Debug.Log("Simplified Visuals: Off");
                break;
            case 1: 
                // Enabled
                Debug.Log("Simplified Visuals: On");
                break;
            default:
                return;
        }
    
        // Only change the player pref if the base case wasn't hit
        PlayerPrefs.SetFloat("SimplifiedVisuals", item);
        PlayerPrefs.Save();

        OnVisualsChanged();
    }
}