using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles all menu-related events.
/// </summary>
public class UIEvents : MonoBehaviour
{
    private static UIEvents _current;

    // OnEnable and Awake can happen simultaneously, causing errors
    // This is a hotfix to make calling current Find this object if it is null
    // Was a major issue in Level 1
    // TODO: Actual fix, this was a hotfix by Brandon
    public static UIEvents current
    {
        get
        {
            if (_current == null) {
                _current = FindObjectOfType<UIEvents>();
            }
            return _current;
        }
        set => _current = value;
    }

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

    public event Action<bool> OnShowFpsCounter;

    public void ShowFpsCounter(bool isShown)
    {
        OnShowFpsCounter?.Invoke(isShown);
    }

    public event Action<bool> OnShowSpeedometer;

    public void ShowSpeedometer(bool isShown)
    {
        OnShowSpeedometer?.Invoke(isShown);
    }

    public event Action<bool> OnShowTimer;

    public void ShowTimer(bool isShown)
    {
        OnShowTimer?.Invoke(isShown);
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

    public event Action<bool> OnCastBolt; // Kept OnCastBolt to avoid errors in StatsMenu.cs

    public void SetCastBolt(bool wasSuccessful)
    {
        OnCastBolt?.Invoke(wasSuccessful);
    }

    public event Action<AbilityHudEnums> OnBoltDisplay;

    public void SetBoltDisplay(AbilityHudEnums displayType)
    {
        OnBoltDisplay?.Invoke(displayType);
    }

    public event Action<AbilityHudEnums> OnWarpDisplay;

    public void SetWarpDisplay(AbilityHudEnums displayType)
    {
        OnWarpDisplay?.Invoke(displayType);
    }

    public event Action<AbilityHudEnums> OnResidueDisplay;

    public void SetResidueDisplay(AbilityHudEnums displayType)
    {
        OnResidueDisplay?.Invoke(displayType);
    }

    public event Action<float> OnBoltCooldown;

    public void SetBoltCooldown(float cooldownDelta)
    {
        OnBoltCooldown?.Invoke(cooldownDelta);
    }

    public event Action<float> OnWarpCooldown;

    public void SetWarpCooldown(float cooldownDelta)
    {
        OnWarpCooldown?.Invoke(cooldownDelta);
    }

    public event Action<float> OnResidueCooldown;

    public void SetResidueCooldown(float cooldownDelta)
    {
        OnResidueCooldown?.Invoke(cooldownDelta);
    }

    public event Action<InteractableEnums> OnChangeXhairColor;

    public void ChangeXhairColor(InteractableEnums type)
    {
        OnChangeXhairColor?.Invoke(type);
    }

    public event Action<bool> OnUnlockBoltAbility;

    public void UnlockBoltAbility(bool isUnlocked)
    {
        OnUnlockBoltAbility?.Invoke(isUnlocked);
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

    public event Action OnAcquireWarpScroll;

    public void AcquireWarpScroll()
    {
        OnAcquireWarpScroll?.Invoke();
    }

    public event Action OnAcquireResidueScroll;

    public void AcquireResidueScroll()
    {
        OnAcquireResidueScroll?.Invoke();
    }

    public event Action OnAcquireGameEndScroll;

    public void AcquireGameEndScroll()
    {
        OnAcquireGameEndScroll?.Invoke();
    }

    public event Action OnCloseScrollAcquiredScreen;

    public void CloseScrollAcquiredScreen()
    {
        OnCloseScrollAcquiredScreen?.Invoke();
    }

    public event Action<bool> OnOpenScrollMenu;

    public void OpenScrollMenu(bool open)
    {
        OnOpenScrollMenu?.Invoke(open);
    }
    
    public event Action<bool> OnOpenLevelSelectMenu;

    public void OpenLevelSelectMenu(bool open)
    {
        OnOpenLevelSelectMenu?.Invoke(open);
    }

    public event Action<bool> OnPlayerWatched;

    public void PlayerWatched(bool watched)
    {
        OnPlayerWatched?.Invoke(watched);
    }

    public event Action OnShowTutorials;

    public void ShowTutorials()
    {
        OnShowTutorials?.Invoke();
    }

    public event Action OnHideTutorials;

    public void HideTutorials()
    {
        OnHideTutorials?.Invoke();
    }

    public event Action<string> OnNotifyPlayer;

    public void NotifyPlayer(string str)
    {
        OnNotifyPlayer?.Invoke(str);
    }

    #endregion

    /* ----------------------------------------------------------------------------------------- */

    #region Options Menu

    public event Action OnOpenOptionsMenu;

    public void OpenOptionsMenu()
    {
        OnOpenOptionsMenu?.Invoke();
    }

    public event Action OnOpenCodeMenu;

    public void OpenCodeMenu()
    {
        OnOpenCodeMenu?.Invoke();
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

    #region Game State

    public event Action<bool> OnPauseGame;

    public void PauseGame(bool isPaused)
    {
        OnPauseGame?.Invoke(isPaused);
    }

    public event Action OnRestartLevel;

    public void RestartLevel()
    {
        OnRestartLevel?.Invoke();
    }

    public event Action OnPlayerDied;

    public void PlayerDied()
    {
        OnPlayerDied?.Invoke();
    }

    public event Action OnPlayerRespawn;

    public void PlayerRespawn()
    {
        OnPlayerRespawn?.Invoke();
    }

    //public event Action OnQuitGame;
    public void QuitGame()
    {
        //OnQuitGame?.Invoke();
        Application.Quit();
    }

    public event Action OnQuitToMenu;

    public void QuitToMenu()
    {
        OnQuitToMenu?.Invoke();
        SceneManager.LoadScene(0);
    }

    public event Action OnDisableGamePausing;

    public void DisableGamePausing()
    {
        OnDisableGamePausing?.Invoke();
    }

    public event Action OnAllowGamePausing;

    public void EnableGamePausing()
    {
        OnAllowGamePausing?.Invoke();
    }

    #endregion
}