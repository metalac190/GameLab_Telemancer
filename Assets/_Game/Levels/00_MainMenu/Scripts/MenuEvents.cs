using System;
using UnityEngine;

/// <summary>
/// Handles all menu-related events.
/// </summary>

public class MenuEvents : MonoBehaviour
{
    public static MenuEvents current;

    private void Awake()
    {
        current = this;
    }

    public event Action ONOpenOptionsMenu;
    public void OpenOptionsMenu()
    {
        ONOpenOptionsMenu?.Invoke();
    }

    public event Action ONSaveCurrentSettings;

    public void SaveCurrentSettings()
    {
        ONSaveCurrentSettings?.Invoke();
    }
}
