using UnityEngine;

public class SpeedometerSelector : OptionSelector
{
    public override void OnItemSelected(int item)
    {
        switch (item)
        {
            case 0:
                // Hidden
                Debug.Log("Speedometer: Hidden");
                UIEvents.current.ShowSpeedometer(false);
                break;
            case 1: 
                // Visible
                Debug.Log("Speedometer: Visible");
                UIEvents.current.ShowSpeedometer(true);
                break;
            default:
                return;
        }
        
        // Only change the player pref if the base case wasn't hit
        PlayerPrefs.SetFloat("Speedometer", item);
    }
}