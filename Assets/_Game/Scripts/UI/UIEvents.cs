using System;
using UnityEngine;

/// <summary>
/// Handles all menu-related events.
/// </summary>

public class UIEvents : MonoBehaviour
{
    public static UIEvents current;

    private void Awake()
    {
        current = this;
    }

    /* ----------------------------------------------------------------------------------------- */
    
    #region HUD

    public event Action OnShowDebugHud;
    public void ShowDebugHud()
    {
        OnShowDebugHud?.Invoke();
    }
    
    public event Action OnHideDebugHud;
    public void HideDebugHud()
    {
        OnHideDebugHud?.Invoke();
    }

    public event Action<string, string> OnNotifyChapter;
    public void NotifyChapter(string chapterNumber, string title)
    {
        OnNotifyChapter?.Invoke(chapterNumber, title);
    }

    public event Action<string> OnDisplayTooltip;
    public void DisplayTooltip(string message)
    {
        OnDisplayTooltip?.Invoke(message);
    }

    public event Action<bool> OnCastBolt;
    public void CastBolt(bool actionSuccessful)
    {
        OnCastBolt?.Invoke(actionSuccessful);
    }

    public event Action OnBoltReady;
    public void BoltReady()
    {
        OnBoltReady?.Invoke();
    }

    public event Action<bool> OnCastWarp;
    public void CastWarp(bool actionSuccessful)
    {
        OnCastWarp?.Invoke(actionSuccessful);
    }

    public event Action OnWarpReady;
    public void WarpReady()
    {
        OnWarpReady?.Invoke();
    }

    public event Action<bool> OnCastResidue;
    public void CastResidue(bool actionSuccessful)
    {
        OnCastResidue?.Invoke(actionSuccessful);
    }

    public event Action OnResidueReady;
    public void ResidueReady()
    {
        OnResidueReady?.Invoke();
    }

    public event Action<InteractableEnums> OnChangeXhairColor;
    public void ChangeXhairColor(InteractableEnums type)
    {
        OnChangeXhairColor?.Invoke(type);   
    }

    public event Action<bool> OnUnlockWarpAbility;
    public void UnlockWarpAbility(bool isUnlocked)
    {
        OnUnlockWarpAbility?.Invoke(isUnlocked);
    }
    
    public event Action<bool> OnUnlockResidueAbility;
    public void UnlockResidueAbility(bool isUnlocked)
    {
        OnUnlockResidueAbility?.Invoke(isUnlocked);
    }

    #endregion
    
    /* ----------------------------------------------------------------------------------------- */
    
    #region Options Menu

    public event Action OnOpenOptionsMenu;
    public void OpenOptionsMenu()
    {
        OnOpenOptionsMenu?.Invoke();
    }

    public event Action OnSaveCurrentSettings;
    public void SaveCurrentSettings()
    {
        OnSaveCurrentSettings?.Invoke();
    }

    public event Action OnReloadSettings;
    public void ReloadSettings()
    {
        OnReloadSettings?.Invoke();
    }
    
    #endregion
    
    /* ----------------------------------------------------------------------------------------- */

    #region Pause Menu

    public event Action<bool> OnPauseGame;
    public void PauseGame(bool isPaused)
    {
        OnPauseGame?.Invoke(isPaused);
    }

    //public event Action OnQuitGame;
    public void QuitGame()
    {
        //OnQuitGame?.Invoke();
        Application.Quit();
    }

    #endregion


}
