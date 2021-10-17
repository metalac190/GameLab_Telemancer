using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject _background;
    [SerializeField] private GameObject _book;
    public bool isPaused = false;
    
    //TODO: create proper way of preventing the player from locking cursor while dead
    private bool _pauseRestricted = false; 

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
        
        _background.SetActive(false);
        _book.SetActive(false);
    }

    private void Update()
    {
        // TODO: move this somewhere that makes sense
        if (!_pauseRestricted && (Keyboard.current.escapeKey.wasPressedThisFrame || Keyboard.current.pKey.wasPressedThisFrame))
            UIEvents.current.PauseGame(!isPaused);
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
    }
}
