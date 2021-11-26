using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject _background = null;
    [SerializeField] private GameObject _book = null;
    [SerializeField] private GameObject _submenu = null;
    [SerializeField] private GameObject _confirmationPanel = null;
    public bool isPaused = false;

    //TODO: create proper way of preventing the player from locking cursor while dead
    private bool _pauseRestricted = false;
    [SerializeField] private bool _pauseLocked = false; // Secondary way of restricting pausing. Separate so they dont conflict

    private void Start()
    {
        UIEvents.current.OnPauseGame += DisplayPauseMenu; // I have to put this in start for some reason??
        UIEvents.current.OnPauseGame += PauseGame;
        
        UIEvents.current.OnPlayerDied += () => _pauseRestricted = true;
        UIEvents.current.OnPlayerRespawn += () => _pauseRestricted = false;
        
        UIEvents.current.OnAcquireWarpScroll += () =>
        {
            PauseGame(true);
            _pauseRestricted = true;
        };
        UIEvents.current.OnAcquireResidueScroll += () =>
        {
            PauseGame(true);
            _pauseRestricted = true;
        };
        UIEvents.current.OnCloseScrollAcquiredScreen += () => _pauseRestricted = false;
        UIEvents.current.OnDisableGamePausing += () => _pauseLocked = true;
        UIEvents.current.OnAllowGamePausing += () => _pauseLocked = false;

        _background.SetActive(false);
        _book.SetActive(false);
    }

    public void OnPauseKeyPressed(InputAction.CallbackContext value) {
        if(value.performed && !_pauseRestricted && !_pauseLocked) {
            if(isPaused && _confirmationPanel.activeSelf)
                _confirmationPanel.SetActive(false);
            else if(isPaused && _submenu.activeSelf)
                _submenu.SetActive(false);
            else
                UIEvents.current.PauseGame(!isPaused);
        }
    }

    private void DisplayPauseMenu(bool display)
    {
        isPaused = display;
        _background.SetActive(display);
        _book.SetActive(display);
    }

    private void PauseGame(bool paused)
    {
        // Set paused
        Time.timeScale = paused ? 0f : 1f;
        
        // Unlock / lock Cursor
        Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = paused;
    }
}
