using UnityEngine;

public class ViewBobbingSelector : OptionSelector
{
    [SerializeField] private GameSettingsData _gameSettingsData;
    
    public override void OnItemSelected(int item)
    {
        switch (item)
        {
            case 0:
                // Off
                _gameSettingsData.ViewBobbingEnabled = false;
                Debug.Log("View Bobbing: Off");
                break;
            case 1: 
                // On
                _gameSettingsData.ViewBobbingEnabled = true;
                Debug.Log("View Bobbing: On");
                break;
            default:
                return;
        }
    
        // Only change the player pref if the base case wasn't hit
        PlayerPrefs.SetFloat("ViewBobbing", item);
    }
}