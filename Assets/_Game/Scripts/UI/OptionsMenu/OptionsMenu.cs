using System;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private GameObject _optionsMenu; 
    // TODO: find best object for attaching the options menu script
    // Henry - 9/18:
    // It seems obvious that you'd want to put the OptionsMenu script on the Options Menu Object,
    // but the script can't receive Events if it isn't active. So then I can't use SetActive(false).

    private Dictionary<string, float> _prefs = new Dictionary<string, float>();

    private void Awake()
    {
        // NOTE: All PlayerPref keys and default values for player options need to be added here
        _prefs.Add("MasterVolume", 100f);
        _prefs.Add("SfxVolume", 100f);
        _prefs.Add("MusicVolume", 100f);
        _prefs.Add("Fov", 90f);
        _prefs.Add("Sensitivity", 20f);
        _prefs.Add("FpsCounter", 0f);
        _prefs.Add("GraphicsQuality", 3f);
    }

    private void Start()
    {
        // Add listener
        UIEvents.current.OnOpenOptionsMenu += OnMenuOpen;
        
        // Ensure menu is hidden
        _optionsMenu.SetActive(false);
    }

    private void OnMenuOpen()
    {
        UIEvents.current.ReloadSettings();
        _optionsMenu.SetActive(true);
    }

    private void OnMenuClose()
    {
        // Save changes made to player prefs
        PlayerPrefs.Save();
        UIEvents.current.SaveCurrentSettings();
        
        // Hide the options menu
        _optionsMenu.SetActive(false);
    }

    public void CloseMenuFromButton()
    {
        OnMenuClose();
    }

    public void RestoreDefualtSettings()
    {
        // Set all player prefs to default values
        foreach (var pref in _prefs)
        {
            PlayerPrefs.SetFloat(pref.Key, pref.Value);
        }
        
        UIEvents.current.ReloadSettings();
    }
}