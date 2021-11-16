using System.Collections;
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

    [Header("Walking Controls")]
    [SerializeField] [MinMaxRange(0, 0.5f)] [Tooltip("The amount of time before the first footstep sound plays")]
    private RangedFloat _firstFootstepDelay = new RangedFloat(0.1f, 0.15f);
    [SerializeField] [MinMaxRange(0, 1)] [Tooltip("The random time interval between the *start* of footstep sounds")]
    private RangedFloat _walkingSoundInterval = new RangedFloat(0.55f, 0.7f);

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
                StartCoroutine(WalkingSounds());
            }
        } else {
            isWalking = false;
        }
    }

    // Play footstep sounds at random intervals while walking
    private IEnumerator WalkingSounds()
    {
        // Delay first footstep sound
        float waitTime = UnityEngine.Random.Range(_firstFootstepDelay.MinValue, _firstFootstepDelay.MaxValue);
        for (float i = 0; i < waitTime; i += Time.deltaTime) {
            if (!isWalking)
                yield break;
            yield return null;
        }

        // While walking, play a random footstep sound at a random interval
        while (isWalking) {
            _playerWalkingSound.Play(transform.position);

            waitTime = UnityEngine.Random.Range(_walkingSoundInterval.MinValue, _walkingSoundInterval.MaxValue);
            for (float i = 0; i < waitTime; i += Time.deltaTime) {
                if (!isWalking)
                    yield break;
                yield return null;
            }
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