using System;
using AudioSystem;
using UnityEngine;

public class PlayerSfx : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private SFXOneShot _activateWarpSound = null;
    [SerializeField] private SFXOneShot _boltCastSound = null;
    [SerializeField] private SFXOneShot _activateResidueSound = null;
    [SerializeField] private SFXOneShot _objectImpactResidueSound = null;

    // Player jumped
    public void OnPlayerJump()
    {

    }

    // Player hit ground
    public void OnPlayerLand()
    {

    }

    // Can be used for footsteps or slight wind noise when moving through air quickly
    public void SetPlayerMovementSpeed(Vector3 speed, bool grounded)
    {

    }
    
    public void OnBoltReady()
    {
    }

    public void OnBoltUsed()
    {
        if (_boltCastSound != null) {
            _boltCastSound.PlayOneShot(transform.position);
        }
    }

    public void OnWarpReady()
    {
    }

    public void OnWarpUsed()
    {
        if (_activateWarpSound != null) {
            _activateWarpSound.PlayOneShot(transform.position);
        }
    }

    public void OnResidueReady()
    {
        if (_objectImpactResidueSound != null) {
            _objectImpactResidueSound.PlayOneShot(transform.position);
        }
    }

    public void OnResidueUsed()
    {
        if (_activateResidueSound != null) {
            _activateResidueSound.PlayOneShot(transform.position);
        }
    }
}