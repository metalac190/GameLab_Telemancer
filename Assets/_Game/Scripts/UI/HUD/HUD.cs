using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The main HUD script.
/// Desperately needs to be split into numerous smaller scripts.
/// </summary>
public class HUD : MonoBehaviour
{
    public bool _debugMode;

    [Header("Ability Icons")]
    [SerializeField] private Image _boltImage = null;
    [SerializeField] private Image _warpImage = null;
    [SerializeField] private Image _residueImage = null;
    
    [Header("Xhair")]
    [SerializeField] private Image _xhair = null;
    [SerializeField] private GameObject _chargeBarContainer;
    [SerializeField] private Image _chargeBarL, _chargeBarR;
    private const float SB_MaxPercent = 0.19f;
    
    [Header("Xhair Colors")]
    [SerializeField] private Color _xhairColorWarp = new Color(0.6352941f, 0.7490196f, 0.9411765f, 1f);
    [SerializeField] private Color _xhairColorInteract = Color.green;
    private Color _xhairColorNormal = Color.white;

    [Header("Respawn Menu")] 
    [SerializeField] private GameObject _respawnMenu;

    [Header("Spotted Indicator")] 
    [SerializeField] private GameObject _spottedIndicatorPnl;
    
    [Header("Scroll Acquired Animation")] 
    [SerializeField] private GameObject _scrollAcquiredScreen;
    [SerializeField] private Text _spellNameTxt;
    [SerializeField] private Text _spellDescTxt;

    [Header("Area Notification")] 
    [SerializeField] private Text _chapterNumber;
    [SerializeField] private Text _chapterName;
    [SerializeField] private Color _chNumColor = Color.white;
    [SerializeField] private Color _chNameColor = Color.white;
    
    [Header("Area Notification Animation")]
    [SerializeField] private float _chNumFadeIn = 0.8f;
    [SerializeField] private float _chNameFadeIn = 1f;
    [SerializeField] private float _Pause = 1f;
    [SerializeField] private float _FadeOut = 1.2f;

    [Header("Debug HUD")] 
    [SerializeField] private GameObject _debugSpellsPnl;
    [SerializeField] private GameObject _debugStatsPnl;

    [Header("Debug HUD Ability Colors")]
    [SerializeField] private Color _readyToUseColor = new Color(0.8f, 0.7f, 0.4f, 0.6f);
    [SerializeField] private Color _inputDetectedColor = new Color(0.75f, 0.75f, 0.5f, 0.75f);
    [SerializeField] private Color _usedColor = new Color(0.5f, 0.75f, 0.5f, 0.75f);
    [SerializeField] private Color _failedColor = new Color(0.75f, 0.5f, 0.5f, 0.5f);
    [SerializeField] private Gradient _cooldownColor = new Gradient();
    private Color _disabledColor = new Color(1, 1, 1, 0.4f);
    private Color _normalColor = new Color(1, 1, 1, 1f);
    
    private void Awake()
    {
        // debug listeners
        UIEvents.current.OnShowDebugHud += () => { _debugMode = true; DisplayDebugHUD(true); };
        UIEvents.current.OnHideDebugHud += () => { _debugMode = false; DisplayDebugHUD(false); };

        // ability listeners
        UIEvents.current.OnUnlockBoltAbility += UnlockBolt;
        UIEvents.current.OnBoltDisplay += BoltDisplay;
        UIEvents.current.OnBoltCooldown += BoltCooldown;
        UIEvents.current.OnUnlockWarpAbility += UnlockWarp;
        UIEvents.current.OnWarpDisplay += WarpDisplay;
        UIEvents.current.OnWarpCooldown += WarpCooldown;
        UIEvents.current.OnUnlockResidueAbility += UnlockResidue;
        UIEvents.current.OnResidueDisplay += ResidueDisplay;
        UIEvents.current.OnResidueCooldown += ResidueCooldown;

        // xhair listener
        UIEvents.current.OnChangeXhairColor += ChangeXhairColor;

        // player listeners
        UIEvents.current.OnPlayerDied += () => DisplayRespawnMenu(true);
        UIEvents.current.OnPlayerRespawn += () => DisplayRespawnMenu(false);

        // scroll listeners
        UIEvents.current.OnAcquireWarpScroll += () => DisplayScrollAcquiredScreen("WARP");
        UIEvents.current.OnAcquireResidueScroll += () => DisplayScrollAcquiredScreen("RESIDUE");
        UIEvents.current.OnCloseScrollAcquiredScreen += () => DisplayScrollAcquiredScreen("CLOSE");
        
        // watched listener
        UIEvents.current.OnPlayerWatched += DisplayWatcherIndicator;
    }

    private void Start()
    {
        UIEvents.current.OnNotifyChapter += (i, s) =>
            StartCoroutine(PlayChapterNotification(i, s));
            
        DisplayDebugHUD(_debugMode);
        _respawnMenu.SetActive(false);
        _scrollAcquiredScreen.SetActive(false);
        _spottedIndicatorPnl.SetActive(false);
    }

    private void DisplayDebugHUD(bool isEnabled)
    {
        _debugSpellsPnl.SetActive(isEnabled);
        _debugStatsPnl.SetActive(isEnabled);
    }
    
    private void DisplayRespawnMenu(bool isEnabled)
    {
        _respawnMenu.SetActive(isEnabled);
        _xhair.transform.parent.gameObject.SetActive(!isEnabled);
        _debugSpellsPnl.SetActive(!isEnabled && _debugMode);
        
        // Set timescale
        Time.timeScale = isEnabled ? 0f : 1f;
        // Unlock / lock Cursor
        Cursor.lockState = isEnabled? CursorLockMode.None : CursorLockMode.Locked;
        
        // Reset the hud
        _boltImage.color = _normalColor;
        _warpImage.color = _normalColor;
        _residueImage.color = _normalColor;

        // Add UI animations here
    }

    // Ability HUD Unlocks:
    // -  After an ability is unlocked, this is called first and then Ability HUD Display, so a color change may not be noticeable
    // -  This is called when the Watcher disables player's abilities
    // -  Place animations and other effects as separate HUD elements above the ability image to avoid issues stated on the first line

    private void UnlockBolt(bool isUnlocked)
    {
        // Debug HUD coloring
        _boltImage.color = isUnlocked ? _normalColor : _disabledColor;
    }

    private void UnlockWarp(bool isUnlocked)
    {
        _chargeBarContainer.SetActive(isUnlocked);

        // Debug HUD coloring
        _warpImage.color = isUnlocked ? _normalColor : _disabledColor;
    }

    private void UnlockResidue(bool isUnlocked)
    {
        // Debug HUD coloring
        _residueImage.color = isUnlocked ? _normalColor : _disabledColor;
    }

    // Ability HUD Display:
    // -  Disabled.         The ability is not unlocked. Use OnAbilityUnlocked(bool unlocked) for control
    // -  Normal.           The ability is idle, cannot be used
    // -  Ready To Use.     The ability is idle, ready to be used
    // -  Input Detected.   User attempted to use ability. Used/Failed may have animation delay. This is immediate.
    // -  Used.             The ability was successfully used
    // -  Failed.           The ability was attempted and failed to be used.

    private void BoltDisplay(AbilityHudEnums displayType)
    {
        switch (displayType) {
            case AbilityHudEnums.Disabled:
                _boltImage.color = _disabledColor;
                break;
            case AbilityHudEnums.Normal:
                _boltImage.color = _normalColor;
                break;
            case AbilityHudEnums.ReadyToUse:
                if (_debugMode)
                    _boltImage.color = _readyToUseColor;
                break;
            case AbilityHudEnums.InputDetected:
                if (_debugMode)
                    _boltImage.color = _inputDetectedColor;
                break;
            case AbilityHudEnums.Used:
                if (_debugMode)
                    _boltImage.color = _usedColor;
                break;
            case AbilityHudEnums.Failed:
                if (_debugMode)
                    _boltImage.color = _failedColor;
                break;
        }
    }

    private void WarpDisplay(AbilityHudEnums displayType)
    {
        switch (displayType)
        {
            case AbilityHudEnums.Disabled:
                _warpImage.color = _disabledColor;
                break;
            case AbilityHudEnums.Normal:
                _warpImage.color = _normalColor;
                break;
            case AbilityHudEnums.ReadyToUse:
                if (_debugMode)
                    _warpImage.color = _readyToUseColor;
                break;
            case AbilityHudEnums.InputDetected:
                if (_debugMode)
                    _warpImage.color = _inputDetectedColor;
                break;
            case AbilityHudEnums.Used:
                if (_debugMode)
                    _warpImage.color = _usedColor;
                break;
            case AbilityHudEnums.Failed:
                if (_debugMode)
                    _warpImage.color = _failedColor;
                break;
        }
    }

    private void ResidueDisplay(AbilityHudEnums displayType)
    {
        switch (displayType)
        {
            case AbilityHudEnums.Disabled:
                _residueImage.color = _disabledColor;
                break;
            case AbilityHudEnums.Normal:
                _residueImage.color = _normalColor;
                break;
            case AbilityHudEnums.ReadyToUse:
                if (_debugMode)
                    _residueImage.color = _readyToUseColor;
                break;
            case AbilityHudEnums.InputDetected:
                if (_debugMode)
                    _residueImage.color = _inputDetectedColor;
                break;
            case AbilityHudEnums.Used:
                if (_debugMode)
                    _residueImage.color = _usedColor;
                break;
            case AbilityHudEnums.Failed:
                if (_debugMode)
                    _residueImage.color = _failedColor;
                break;
        }
    }

    // Ability HUD Cool-downs
    // -  These are controlled by PlayerToHud.cs
    // -  CooldownDelta goes from 0 to 1. 0 means the cooldown just started, 1 means the cooldown will end next frame
    // -  AbilityCooldown already accesses the animation data and cooldown duration timers through the CooldownDelta.

    private void BoltCooldown(float cooldownDelta)
    {
        if (_debugMode)
            _boltImage.color = _cooldownColor.Evaluate(cooldownDelta);
    }

    private void WarpCooldown(float cooldownDelta)
    {
        // Set both charge bars to the current cooldown value of warp
        _chargeBarL.fillAmount = cooldownDelta * SB_MaxPercent;
        _chargeBarR.fillAmount = cooldownDelta * SB_MaxPercent;
        
        if (_debugMode)
            _warpImage.color = _cooldownColor.Evaluate(cooldownDelta);
    }

    private void ResidueCooldown(float cooldownDelta)
    {
        if (_debugMode)
            _residueImage.color = _cooldownColor.Evaluate(cooldownDelta);
    }

    private void ChangeXhairColor(InteractableEnums target)
    {
        // Looking at Interactable is either -1, 0, or 1, for Null, Object, and Interactable, respectfully
        switch (target) {
            case InteractableEnums.WarpInteractable:
                _xhair.color = _xhairColorWarp;
                break;
            case InteractableEnums.PlayerInteractable:
                _xhair.color = _xhairColorInteract;
                break;
            case InteractableEnums.Object:
            case InteractableEnums.Null:
                _xhair.color = _xhairColorNormal;
                break;
        }
    }
    
    /*
    private IEnumerator FillBoltStatusBar(float duration)
    {
        float time = 0;
        while (time < duration)
        {
            // Borrowing this from the internet really quick
            float t = time / duration;
            //t = t * t * (3f - 2f * t);
            
            float percentFilled = Mathf.Lerp(0, SB_MaxPercent, t);
            _chargeBarL.fillAmount = percentFilled;
            _chargeBarR.fillAmount = percentFilled;
            time += Time.deltaTime;
            yield return null;
        }
        _chargeBarL.fillAmount = SB_MaxPercent;
        _chargeBarR.fillAmount = SB_MaxPercent;
    }
    */

    private void DisplayScrollAcquiredScreen(string scroll)
    {
        switch (scroll)
        {
            case "WARP":
                _spellNameTxt.text = "WARP BOLT";
                _spellDescTxt.text =
                    "Press [LMB] to cast a swirling ball of energy that has teleporting properties depending on the object it.";
                _debugSpellsPnl.SetActive(false);
                _scrollAcquiredScreen.SetActive(true);
                break;
            case "RESIDUE":
                _spellNameTxt.text = "WARP RESIDUE";
                _spellDescTxt.text = "Lorem Ipsum";
                _debugSpellsPnl.SetActive(false);
                _scrollAcquiredScreen.SetActive(true);
                break;
            default:
                _debugSpellsPnl.SetActive(_debugMode);
                _scrollAcquiredScreen.SetActive(false);
                break;
        }
    }

    private void DisplayWatcherIndicator(bool watched)
    {
        _spottedIndicatorPnl.SetActive(watched);
    }

    private IEnumerator PlayChapterNotification(string chapter, string title)
    {
        _chapterNumber.text = chapter;
        _chapterName.text = title;

        float time = 0;
        float alphaL1, alphaL2;
        float targetA1 = _chNumColor.a;
        float targetA2 = _chNameColor.a;
        
        // set both lines invisible
        _chapterNumber.gameObject.SetActive(true);
        _chapterName.gameObject.SetActive(true);
        _chapterNumber.color = new Color(_chNumColor.r, _chNumColor.g, _chNumColor.b, 0);
        _chapterName.color = new Color(_chNameColor.r, _chNameColor.g, _chNameColor.b, 0);
        
        // wait a little bit
        yield return new WaitForSecondsRealtime(1f);
        
        // fade in line one
        while (time < _chNumFadeIn)
        {
            alphaL1 = Mathf.Lerp(0, targetA1, time / _chNumFadeIn);
            _chapterNumber.color = new Color(_chNumColor.r, _chNumColor.g, _chNumColor.b, alphaL1);
            time += Time.deltaTime;
            yield return null;
        }
        _chapterNumber.color = new Color(_chNumColor.r, _chNumColor.g, _chNumColor.b, targetA1);
        
        // fade in line two
        time = 0;
        while (time < _chNameFadeIn)
        {
            alphaL2 = Mathf.Lerp(0, targetA2, time / _chNameFadeIn);
            _chapterName.color = new Color(_chNameColor.r, _chNameColor.g, _chNameColor.b, alphaL2);
            time += Time.deltaTime;
            yield return null;
        }
        _chapterName.color = new Color(_chNameColor.r, _chNameColor.g, _chNameColor.b, targetA2);
        
        // wait a little bit
        yield return new WaitForSecondsRealtime(_Pause);

        // fade out both
        time = 0;
        while (time < _FadeOut)
        {
            alphaL1 = Mathf.Lerp(1, 0, time / _FadeOut);
            _chapterNumber.color = new Color(_chNumColor.r, _chNumColor.g, _chNumColor.b, alphaL1);
            _chapterName.color = new Color(_chNameColor.r, _chNameColor.g, _chNameColor.b, alphaL1);
            time += Time.deltaTime;
            yield return null;
        }
        _chapterNumber.color = new Color(_chNumColor.r, _chNumColor.g, _chNumColor.b, 0);
        _chapterName.color = new Color(_chNameColor.r, _chNameColor.g, _chNameColor.b, 0);
        
        // disable both
        _chapterNumber.gameObject.SetActive(false);
        _chapterName.gameObject.SetActive(false);
    }
}
