using UnityEngine;

public class SpeedrunTimerSelector : OptionSelector
{
    public override void OnItemSelected(int item)
    {
        switch (item)
        {
            case 0:
                // Hidden
                Debug.Log("Speedrun Timer: Hidden");
                UIEvents.current.ShowTimer(false);
                break;
            case 1: 
                // Visible
                Debug.Log("Speedrun Timer: Visible");
                UIEvents.current.ShowTimer(true);
                break;
            default:
                return;
        }
        
        // Only change the player pref if the base case wasn't hit
        PlayerPrefs.SetFloat("SpeedrunTimer", item);
    }
}