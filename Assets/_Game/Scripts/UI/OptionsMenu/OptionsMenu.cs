using System;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private GameObject _optionsMenu = null; 
    // TODO: find best object for attaching the options menu script
    // Henry - 9/18:
    // It seems obvious that you'd want to put the OptionsMenu script on the Options Menu Object,
    // but the script can't receive Events if it isn't active. So then I can't use SetActive(false).

    private Dictionary<string, float> _prefs = new Dictionary<string, float>();

    private void Awake()
    {
        // NOTE: All PlayerPref keys and default values for player options need to be added here
        _prefs.Add("MasterVolume", 1000f);
        _prefs.Add("SfxVolume", 1000f);
        _prefs.Add("MusicVolume", 1000f);
        _prefs.Add("Fov", 59f);
        _prefs.Add("Sensitivity", 10f);
        _prefs.Add("FpsCounter", 0f);
        _prefs.Add("GraphicsQuality", 3f);
        _prefs.Add("VSync", 0f);
        _prefs.Add("Fullscreen", 1f);
        _prefs.Add("AntiAliasing", 0f);
        _prefs.Add("Resolution", -1f);
        _prefs.Add("SimplifiedVisuals", 0f);
    }

    private void Start()
    {
        // Add listener
        UIEvents.current.OnOpenOptionsMenu += OnMenuOpen;
        UIEvents.current.OnPauseGame += delegate(bool b) { if (!b) OnMenuClose(); };
        
        // Ensure menu is hidden
        _optionsMenu.SetActive(false);
    }

    private void OnMenuOpen()
    {
        // Set prefs to default if they don't exist
        foreach (var pref in _prefs)
        {
            if (!PlayerPrefs.HasKey(pref.Key))
                PlayerPrefs.SetFloat(pref.Key, pref.Value);
        }
        
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

    public void RestoreDefaultSettings()
    {
        // Set all player prefs to default values
        foreach (var pref in _prefs)
        {
            PlayerPrefs.SetFloat(pref.Key, pref.Value);
        }
        
        UIEvents.current.ReloadSettings();
    }
}