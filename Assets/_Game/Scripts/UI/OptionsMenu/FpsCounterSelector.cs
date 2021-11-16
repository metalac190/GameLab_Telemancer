using UnityEngine;

public class FpsCounterSelector : OptionSelector
{
    public override void OnItemSelected(int item)
    {
        switch (item)
        {
            case 0:
                // Hidden
                Debug.Log("FPS Counter: Hidden");
                UIEvents.current.HideDebugHud();
                break;
            case 1: 
                // Visible
                Debug.Log("FPS Counter: Visible");
                UIEvents.current.ShowDebugHud();
                break;
            default:
                return;
        }
        
        // Only change the player pref if the base case wasn't hit
        PlayerPrefs.SetFloat("FpsCounter", item);
    }
}