using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class OptionSelector : MonoBehaviour
{
    public enum PlayerPrefKey
    {
        FpsCounter,
        Tutorials,
        GraphicsQuality,
        VSync,
        Fullscreen,
        AntiAliasing,
        SimplifiedVisuals,
        ViewBobbing,
        SpeedrunTimer,
        Speedometer
    }
    
    // Have PlayerPrefKey selected via dropdown menu in the inspector
    [SerializeField] private PlayerPrefKey prefKey = PlayerPrefKey.FpsCounter;
    
    [SerializeField] private Button[] options = {};

    private void Start()
    {
        UIEvents.current.OnReloadSettings += LoadValue;
        
        // Create listeners for all the option buttons
        for (int x = 0; x < options.Length; x++)
        {
            var x1 = x;
            options[x].onClick.AddListener(delegate { SelectItem(x1); });
        }
        
        LoadValue();
    }

    private void OnBecameVisible()
    {
        LoadValue();
    }
    
    protected virtual void LoadValue()
    {
        int val = (int)PlayerPrefs.GetFloat(prefKey.ToString());

        for (var x = 0; x < options.Length; x++)
            options[x].interactable = x != val;
    }

    public void SelectItem(int item)
    {
        for (var x = 0; x < options.Length; x++)
        {
            // Make only the selected item non-interactable
            options[x].interactable = x != item; 
        }
        
        // Call method for handling interaction outcome
        OnItemSelected(item);
    }

    public abstract void OnItemSelected(int item);
}