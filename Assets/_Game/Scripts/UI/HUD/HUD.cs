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
    [SerializeField] private Image _chargeBarL, _chargeBarR;
    private const float SB_MaxPercent = 0.19f;
    
    [Header("Xhair Colors")]
    [SerializeField] private Color _xhairColorWarp = new Color(0.6352941f, 0.7490196f, 0.9411765f, 1f);
    [SerializeField] private Color _xhairColorInteract = Color.green;
    private Color _xhairColorNormal = Color.white;

    [Header("Respawn Menu")] 
    [SerializeField] private GameObject _respawnMenu;

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
    [SerializeField] private Color _usedColor = new Color(0.5f, 0.75f, 0.5f, 0.75f);
    [SerializeField] private Color _readyToUseColor = new Color(0.8f, 0.7f, 0.4f, 0.6f);
    [SerializeField] private Color _failedColor = new Color(0.75f, 0.5f, 0.5f, 0.5f);
    private Color _disabledColor = new Color(1, 1, 1, 0.4f);
    private Color _normalColor = new Color(1, 1, 1, 1f);
    
    private void Awake()
    {
        // add listeners
        UIEvents.current.OnShowDebugHud += () => { _debugMode = true; DisplayDebugHUD(true); };
        UIEvents.current.OnHideDebugHud += () => { _debugMode = false; DisplayDebugHUD(false); };
        UIEvents.current.OnUnlockWarpAbility += UnlockWarp;
        UIEvents.current.OnUnlockResidueAbility += UnlockResidue;
        UIEvents.current.OnCastBolt += CastBolt;
        UIEvents.current.OnBoltReady += BoltReady;
        UIEvents.current.OnCastWarp += CastWarp;
        UIEvents.current.OnWarpReady += WarpReady;
        UIEvents.current.OnCastResidue += CastResidue;
        UIEvents.current.OnResidueReady += ResidueReady;
        UIEvents.current.OnChangeXhairColor += ChangeXhairColor;

        UIEvents.current.OnPlayerDied += () => DisplayRespawnMenu(true);
        UIEvents.current.OnPlayerRespawn += () => DisplayRespawnMenu(false);
    }

    private void Start()
    {
        UIEvents.current.OnNotifyChapter += (i, s) =>
            StartCoroutine(PlayChapterNotification(i, s));
            
        DisplayDebugHUD(_debugMode);
        _respawnMenu.SetActive(false);
        //UIEvents.current.NotifyChapter("CHAPTER III", "gm_flatgrass");
    }

    private IEnumerator InputDebug(Image image, bool successful)
    {
        image.color = successful ? _usedColor : _failedColor;
        yield return new WaitForSecondsRealtime(0.1f); // Change value for longer flash after input
        image.color = _normalColor;
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

    private void UnlockWarp(bool isUnlocked)
    {
        // Add ability unlocked HUD animation here...

        // Debug HUD coloring
        _warpImage.color = isUnlocked ? _normalColor : _disabledColor;
    }
    
    private void UnlockResidue(bool isUnlocked)
    {
        // Add ability unlocked HUD animation here...

        // Debug HUD coloring
        _residueImage.color = isUnlocked ? _normalColor : _disabledColor;
    }

    private void CastBolt(bool actionSuccessful)
    {
        // Update debug hud
        if (_debugMode)
            StartCoroutine(InputDebug(_boltImage, actionSuccessful));
        
        if (!actionSuccessful) return;
        
        // Play status bar animation
        StartCoroutine(FillBoltStatusBar(0.5f)); //TODO: add reference to bolt cast duration
    }

    private void BoltReady()
    {
        // Debug HUD coloring
        if (_debugMode)
            _boltImage.color = _readyToUseColor;

    }

    private void CastWarp(bool actionSuccessful)
    {
        // Update debug hud
        if (_debugMode)
            StartCoroutine(InputDebug(_warpImage, actionSuccessful));
    }
    
    private void WarpReady(bool isReady)
    {
        // Debug HUD coloring
        if (_debugMode)
            _warpImage.color = isReady ? _readyToUseColor : _normalColor;
    }
    
    private void CastResidue(bool actionSuccessful)
    {
        // Update debug hud
        if (_debugMode)
            StartCoroutine(InputDebug(_residueImage, actionSuccessful));
    }
    
    private void ResidueReady(bool isReady)
    {
        // Debug HUD coloring
        if (_debugMode)
            _residueImage.color = isReady ? _readyToUseColor : _normalColor;
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
    
    private IEnumerator FillBoltStatusBar(float duration)
    {
        float time = 0;
        while (time < duration)
        {
            // Borrowing this from the internet really quick
            float t = time / duration;
            t = t * t * (3f - 2f * t);
            
            float percentFilled = Mathf.Lerp(0, SB_MaxPercent, t);
            _chargeBarL.fillAmount = percentFilled;
            _chargeBarR.fillAmount = percentFilled;
            time += Time.deltaTime;
            yield return null;
        }
        _chargeBarL.fillAmount = SB_MaxPercent;
        _chargeBarR.fillAmount = SB_MaxPercent;
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
