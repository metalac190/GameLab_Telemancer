using System;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private GameObject _optionsMenu; 
    // TODO: find best object for attaching the options menu script
    // Henry - 9/18:
    // It seems obvious that you'd want to put the OptionsMenu script on the Options Menu Object,
    // but the script can't receive Events if it isn't active. So then I can't use SetActive(false).
    
    private void Start()
    {
        MenuEvents.current.ONOpenOptionsMenu += OnMenuOpen;
        MenuEvents.current.ONSaveCurrentSettings += SaveSettings;
    }

    private void OnMenuOpen()
    {
        _optionsMenu.SetActive(true);
    }

    private void OnMenuClose()
    {
        // Disregard any unsaved changes made to the player prefs
        
        // Hide the options menu
        _optionsMenu.SetActive(false);
    }

    private void SaveSettings()
    {
        
    }
}