using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

/// <summary>
/// Script for the unlock scroll that appears at the end of each level
/// </summary>
public class Scroll : MonoBehaviour, IPlayerInteractable
{
    [SerializeField] private VisualEffect _disintigrateVFX;
    [SerializeField] private GameObject _chainsGroup;
    [SerializeField] private GameObject _scroll;
    
    enum unlockEnum { WarpBolt, Residue }

    [SerializeField] private unlockEnum _scrollUnlock;

    private bool _used;

    public void Start()
    {
        UIEvents.current.OnRestartLevel += OnReset;
        UIEvents.current.OnPlayerRespawn += OnReset;
        
        _chainsGroup.SetActive(true);
        _scroll.SetActive(true);
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
        
        // set used
        _used = true;

        // wait
        StartCoroutine(DramaticPause());
    }

    IEnumerator DramaticPause()
    {
        yield return new WaitForSecondsRealtime(0.4f);
        
        // HUD - Show ability unlocked 
        if (_scrollUnlock == unlockEnum.WarpBolt)
            UIEvents.current.UnlockWarpAbility(true);
        else
            UIEvents.current.UnlockResidueAbility(true);
    }
}
