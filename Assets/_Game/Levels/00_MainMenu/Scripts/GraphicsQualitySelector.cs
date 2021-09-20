using UnityEngine;

public class GraphicsQualitySelector : OptionSelector
{
    public override void OnItemSelected(int item)
    {
        switch (item)
        {
            case 0:
                // Low Quality
                Debug.Log("Graphics quality selected: Low");
                break;
            case 1:
                // Medium Quality
                Debug.Log("Graphics quality selected: Medium");
                break;
            case 2:
                // High Quality
                Debug.Log("Graphics quality selected: High");
                break;
            case 3:
                // Ultra Quality
                Debug.Log("Graphics quality selected: Ultra");
                break;
            default:
                return;
        }
        
        // Only change the player pref if the base case wasn't hit
        PlayerPrefs.SetFloat("GraphicsQuality", item);
    }
}