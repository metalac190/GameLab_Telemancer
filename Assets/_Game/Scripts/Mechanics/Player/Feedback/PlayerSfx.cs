using System;
using AudioSystem;
using UnityEngine;

public class PlayerSfx : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private SFXOneShot _activateWarpSound = null;
    [SerializeField] private SFXOneShot _boltCastSound = null;
    [SerializeField] private SFXOneShot _activateResidueSound = null;
    [SerializeField] private SFXOneShot _playerJumpSound = null;
    [SerializeField] private SFXOneShot _playerLandSound = null;
    [SerializeField] private SFXOneShot _playerDeathSound = null;
    [SerializeField] private SFXOneShot _playerSnapSound = null;
    [SerializeField] private SFXLoop _playerWalkingSound = null;
    private bool isWalking = false;

    private void Start()
    {
        UIEvents.current.OnPlayerDied += OnPlayerKilled;
    }

    public void OnPlayerKilled()
    {
        if (_playerDeathSound != null) {
            _playerDeathSound.PlayOneShot(transform.position);
        }
    }

    // Player jumped
    public void OnPlayerJump()
    {
        if (_playerJumpSound != null) {
            _playerJumpSound.PlayOneShot(transform.position);
        }
    }

    // Player hit ground
    public void OnPlayerLand()
    {
        if (_playerLandSound != null) {
            _playerLandSound.PlayOneShot(transform.position);
        }
    }

    // Can be used for footsteps or slight wind noise when moving through air quickly
    public void SetPlayerMovementSpeed(Vector3 speed, bool grounded, bool walking)
    {
        if (walking) {
            if (!isWalking) {
                isWalking = true;
                _playerWalkingSound.Play(transform.position);
            }
        } else {
            isWalking = false;
        }
    }

    public void OnAnimationSnap()
    {
        if (_playerSnapSound != null) {
            _playerSnapSound.PlayOneShot(transform.position);
        }
    }

    public void InWatcherRange(bool inRange)
    {
        if (inRange) {
            // In sight of watcher
        } else {
            // No longer in sight of watcher
        }
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
    }

    public void OnResidueUsed()
    {
        if (_activateResidueSound != null) {
            _activateResidueSound.PlayOneShot(transform.position);
        }
    }
}