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
    private bool _isDead = false; 

    private void Start()
    {
        UIEvents.current.OnPauseGame += PauseGame; // I have to put this in start for some reason??
        UIEvents.current.OnPlayerDied += () => _isDead = true;
        UIEvents.current.OnPlayerRespawn += () => _isDead = false;
        
        _background.SetActive(false);
        _book.SetActive(false);
    }

    private void Update()
    {
        // TODO: move this somewhere that makes sense
        if (!_isDead && (Keyboard.current.escapeKey.wasPressedThisFrame || Keyboard.current.pKey.wasPressedThisFrame))
            UIEvents.current.PauseGame(!isPaused);
    }

    private void PauseGame(bool paused)
    {
        // Set paused
        isPaused = paused;
        
        // Set timescale
        Time.timeScale = paused ? 0f : 1f;
        
        // Unlock / lock Cursor
        Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;

        _background.SetActive(paused);
        _book.SetActive(paused);
    }
}
