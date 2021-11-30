using UnityEngine;

public class DynamicResolutionSelector : OptionSelector
{
    public override void OnItemSelected(int item)
    {
        switch (item)
        {
            case 0:
                // Hidden
                Debug.Log("Dynamic Resolution: Off");
                Camera.main.allowDynamicResolution = false;
                break;
            case 1: 
                // Visible
                Debug.Log("Dynamic Resolution: On");
                Camera.main.allowDynamicResolution = true;
                break;
            default:
                return;
        }
        PlayerPrefs.SetFloat("DynamicResolution", item);
    }
}
