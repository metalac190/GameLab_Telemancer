using System.Collections;
using Mechanics.WarpBolt;
using UnityEngine;

// The controller for the Player Casting Sequence.
// Requires a reference to the Warp Bolt and a reference to the Player Animator
// Public functions are called by the Player Input System
public class PlayerCasting : MonoBehaviour
{
    [Header("Unlocks")]
    [SerializeField] private bool _warpAbility = false;
    [Header("Settings")]
    [SerializeField] private float _boltLookDistance = 20f;
    [Header("References")]
    [SerializeField] private UIColorChanger _hitStateColor = null;
    [SerializeField] private PlayerAnimator _animator;
    [SerializeField] private BoltController _warpBolt = null;
    [SerializeField] private Transform _boltFirePosition = null;
    [SerializeField] private Transform _cameraLookDirection = null;

    private bool _isCasting;
    private int _lookingAtInteractable = -1;

    #region NullCheck

    private bool _missingAnimator;

    private void AnimatorNullCheck()
    {
        if (_animator == null) {
            _animator = transform.GetComponentInChildren<PlayerAnimator>();
            if (_animator == null) {
                _animator = GetComponent<PlayerAnimator>();
                if (_animator == null) {
                    _missingAnimator = true;
                    Debug.LogWarning("Cannot find the Player Animator for the Player Casting Script", gameObject);
                }
            }
        }
    }

    private bool _missingWarpBolt;

    private void WarpBoltNullCheck()
    {
        if (_warpBolt == null) {
            _missingWarpBolt = true;
            throw new MissingComponentException("Missing the Warp Bolt Reference on the Player Casting Script on " + gameObject);
        } else {
            PlayerController controller = GetComponent<PlayerController>();
            if (controller == null) {
                controller = FindObjectOfType<PlayerController>();
                if (controller == null) {
                    Debug.LogError("Cannot find Player Controller", gameObject);
                }
            }
            if (controller != null) {
                _warpBolt.BoltData.SetPlayerReference(controller);
            }
        }
    }

    #endregion

    #region Unity Functions

    private void OnEnable()
    {
        AnimatorNullCheck();
        WarpBoltNullCheck();
    }

    private void Update()
    {
        UpdateCastColor();
    }

    #endregion

    // -------------------------------------------------------------------------------------------

    #region Public Functions

    public void CastBolt()
    {
        // Called three times on quick click and called on click release too...
        // TODO: Fix Player Input left mouse clicking
        if (_isCasting || _missingWarpBolt) return;
        PrepareToCast();
        StartCoroutine(Cast());
    }

    public void ActivateBolt()
    {
        if (_missingWarpBolt) return;
        Warp();
    }

    #endregion

    // -------------------------------------------------------------------------------------------

    #region Private Functions

    private void PrepareToCast()
    {
        _warpBolt.PrepareToFire(GetBoltPosition(), _cameraLookDirection.rotation);
    }

    // The main Coroutine for casting the warp bolt
    private IEnumerator Cast()
    {
        _isCasting = true;
        float fireTime = GetTimeToFire();
        if (fireTime > 0) {
            for (float t = 0; t <= fireTime; t += Time.deltaTime) {
                float delta = t / fireTime;
                CastStatus(delta);
                HoldPosition();
                yield return null;
            }
        }
        CastStatus(1);
        Fire();
        _isCasting = false;
    }

    private void CastStatus(float status)
    {
        // Later send status info to Animator? Or other way around
        // We pull time to fire from the Animator **
        _warpBolt.SetCastStatus(status);
    }

    private void HoldPosition()
    {
        _warpBolt.SetPosition(GetBoltPosition());
    }

    private void Fire()
    {
        // Could tell animator to cast bolt, but it should be on the same page. Add check?
        _warpBolt.Fire(GetBoltPosition(), GetBoltForward());
    }

    private void Warp()
    {
        if (!_warpAbility) return;
        _warpBolt.OnWarp();
    }

    private void UpdateCastColor()
    {
        if (_hitStateColor == null) return;
        GetBoltForward(); // TODO: Better way to update?

        // Looking at Interactable is either -1, 0, or 1, for Null, Object, and Interactable, respectfully
        switch (_lookingAtInteractable) {
            case 1:
                _hitStateColor.SetColor(Color.cyan);
                break;
            case 0:
                _hitStateColor.SetColor(Color.white);
                break;
            default:
                _hitStateColor.SetColor(Color.white);
                break;
        }
    }

    #endregion

    #region Helper Functions

    private float GetTimeToFire()
    {
        return !_missingAnimator ? _animator.OnCastBolt() : 0.2f;
    }

    // A currently very messy script to get the Bolt's forward direction (should be towards the center of the camera)
    private Vector3 GetBoltForward()
    {
        Vector3 current = GetBoltPosition();
        if (_cameraLookDirection != null) {
            Ray ray = new Ray(_cameraLookDirection.position, _cameraLookDirection.forward);
            Physics.Raycast(ray, out var hit, _boltLookDistance);
            if (hit.point != Vector3.zero) {
                Vector3 angle = hit.point - current;
                if (hit.collider != null) {
                    var interactable = hit.collider.GetComponent<IWarpInteractable>();
                    _lookingAtInteractable = interactable != null ? 1 : 0;
                }
                return angle.normalized;
            }
            _lookingAtInteractable = -1;
            Vector3 angle1 = _cameraLookDirection.position + _cameraLookDirection.forward * _boltLookDistance - current;
            return angle1.normalized;
        }
        return _boltFirePosition != null ? _boltFirePosition.forward : transform.forward;
    }

    // A simple function to get the position of the warp bolt
    private Vector3 GetBoltPosition()
    {
        return _boltFirePosition != null ? _boltFirePosition.position : transform.position;
    }

    #endregion
}