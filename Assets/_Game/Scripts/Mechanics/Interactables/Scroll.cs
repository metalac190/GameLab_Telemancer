﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using AudioSystem;
using Mechanics.Player;

/// <summary>
/// Script for the unlock scroll that appears at the end of each level
/// </summary>
public class Scroll : MonoBehaviour, IPlayerInteractable
{
    [SerializeField] private VisualEffect _disintigrateVFX = null;
    [SerializeField] private GameObject _chainsGroup = null;
    [SerializeField] private GameObject _scroll = null;
    [SerializeField] private float _pauseLength = 1;
    [SerializeField] private SFXOneShot scrollOpenSFX = null;
    [SerializeField] private int loadingScreenID = 1;
    private int nextlevelID;

    enum unlockEnum { WarpBolt, Residue, None }

    [SerializeField] private unlockEnum _scrollUnlock = unlockEnum.WarpBolt;

    private bool _used;

    public void Start()
    {
        UIEvents.current.OnRestartLevel += OnReset;
        UIEvents.current.OnPlayerRespawn += OnReset;
        
        _chainsGroup.SetActive(true);
        _scroll.SetActive(true);
        nextlevelID = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1;
    }

    public void OnReset()
    {
        _used = false;
        _chainsGroup.SetActive(true);
        _scroll.SetActive(true);
    }
    
    public void OnInteract()
    {
        if (_used) return;
        
        // play VFX
        _disintigrateVFX.Play();
        
        // hide chains
        _chainsGroup.SetActive(false);
        
        // hide scroll
        _scroll.SetActive(false);

        // TODO: Make this more efficient
        PlayerState playerState = FindObjectOfType<PlayerState>();
        playerState.LockPlayer(true);
        
        // set used
        _used = true;

        // wait
        StartCoroutine(DramaticPause());
    }

    IEnumerator DramaticPause()
    {
        yield return new WaitForSecondsRealtime(_pauseLength);

        //Play sound
        scrollOpenSFX.PlayOneShot(transform.position);
        
        // HUD - Show ability unlocked 
        switch (_scrollUnlock)
        {
            case unlockEnum.WarpBolt:
                UIEvents.current.UnlockWarpAbility(true);
                UIEvents.current.AcquireWarpScroll(); // PauseMenu.cs has the game pause on this event
                break;
            case unlockEnum.Residue:
                UIEvents.current.UnlockResidueAbility(true);
                UIEvents.current.AcquireResidueScroll();
                break;
        }

        if (_scrollUnlock != unlockEnum.None)
        {
            // Hack fraud way of waiting for player input
            while (!Keyboard.current.eKey.wasPressedThisFrame)
                yield return null;
        }
        
        // load next level
        UIEvents.current.CloseScrollAcquiredScreen();
        UIEvents.current.PauseGame(false);
        // Stops music of current level before switch
        MusicManager.Instance.StopMusic();
        // TODO: Add level switch code here
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        PlayerPrefs.SetInt("CurrentLevel", nextlevelID);
        PlayerPrefs.Save();
        TransitionManager.tm.ChangeLevel(1); //NEEDS TO GO TO LOADING SCREEN BUT IT WORKS IF HAVE TO
    }
}
