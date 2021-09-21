using System.Collections;
using Mechanics.WarpBolt;
using UnityEngine;

namespace Mechanics.Player
{
    // The controller for the Player Casting Sequence.
    // Requires a reference to the Warp Bolt and a reference to the Player Animator
    // Public functions are called by the Player Input System
    public class PlayerCasting : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _boltLookDistance = 20f;
        [SerializeField] private float _timeToFire = 0;
        [Header("External References")]
        [SerializeField] private BoltController _warpBolt;
        [Header("Internal References")]
        [SerializeField] private PlayerState _playerState;
        [SerializeField] private PlayerAnimator _playerAnimator;
        [SerializeField] private Transform _boltFirePosition = null;
        [SerializeField] private Transform _cameraLookDirection = null;

        private bool _warpAbility;
        private bool _residueAbility;
        private bool _isCasting;

        #region NullCheck

        private bool _missingState;

        private void StateNullCheck()
        {
            if (_playerState == null) {
                if (transform.parent != null) {
                    _playerState = transform.parent.GetComponent<PlayerState>();
                    if (transform.parent != null) {
                        _playerState = transform.parent.GetComponentInChildren<PlayerState>();
                    }
                } else {
                    _playerState = GetComponent<PlayerState>();
                }
                if (_playerState == null) {
                    _missingState = true;
                    Debug.LogWarning("Cannot find the Player State for the Player Casting Script", gameObject);
                }
            }
        }

        //private bool _missingAnimator;

        private void AnimatorNullCheck()
        {
            if (_playerAnimator == null) {
                _playerAnimator = transform.parent != null ? transform.parent.GetComponentInChildren<PlayerAnimator>() : GetComponent<PlayerAnimator>();
                if (_playerAnimator == null) {
                    //_missingAnimator = true;
                    Debug.LogWarning("Cannot find the Player Animator for the Player Casting Script", gameObject);
                }
            }
        }

        private bool _missingWarpBolt;

        private void WarpBoltNullCheck()
        {
            if (_warpBolt == null) {
                _warpBolt = FindObjectOfType<BoltController>();
                if (_warpBolt == null) {
                    _missingWarpBolt = true;
                    throw new MissingComponentException("Missing the Warp Bolt Reference on the Player Casting Script on " + gameObject);
                }
            }
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

        #endregion

        #region Unity Functions

        private void OnEnable()
        {
            StateNullCheck();
            AnimatorNullCheck();
            WarpBoltNullCheck();

            if (!_missingState) {
                _playerState.OnChangeUnlocks += SetUnlocks;
            } else {
                SetUnlocks(false, false);
            }
        }

        private void OnDisable()
        {
            if (!_missingState) {
                _playerState.OnChangeUnlocks -= SetUnlocks;
            }
        }

        private void Update()
        {
            // Update HUD color
            GetRaycast();
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
            if (_warpBolt.ResidueReady) {
                ActivateResidue();
            } else {
                Warp();
            }
        }

        #endregion

        // -------------------------------------------------------------------------------------------

        #region Private Functions

        private void SetUnlocks(bool warp, bool residue)
        {
            _warpAbility = warp;
            _residueAbility = residue;
        }

        private void PrepareToCast()
        {
            _warpBolt.PrepareToFire(GetBoltPosition(), _cameraLookDirection.rotation, _residueAbility);
        }

        // The main Coroutine for casting the warp bolt
        private IEnumerator Cast()
        {
            _isCasting = true;
            if (_timeToFire > 0) {
                for (float t = 0; t <= _timeToFire; t += Time.deltaTime) {
                    float delta = t / _timeToFire;
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
            _warpBolt.SetPosition(GetBoltPosition(), _cameraLookDirection.rotation);
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

        private void ActivateResidue()
        {
            if (!_residueAbility) return;
            _warpBolt.OnActivateResidue();
        }

        #endregion

        #region Helper Functions

        // A currently very messy script to get the Bolt's forward direction (should be towards the center of the camera)
        private Vector3 GetBoltForward()
        {
            if (_cameraLookDirection == null) {
                return _boltFirePosition != null ? _boltFirePosition.forward : transform.forward;
            }

            Vector3 current = GetBoltPosition();
            Vector3 angle = GetRaycast() - current;
            return angle.normalized;
        }

        private Vector3 GetRaycast()
        {
            Ray ray = new Ray(_cameraLookDirection.position, _cameraLookDirection.forward);
            Physics.Raycast(ray, out var hit, _boltLookDistance);

            if (hit.point != Vector3.zero) {
                return hit.point;
            }
            return _cameraLookDirection.position + _cameraLookDirection.forward * _boltLookDistance;
        }

        // A simple function to get the position of the warp bolt
        private Vector3 GetBoltPosition()
        {
            return _boltFirePosition != null ? _boltFirePosition.position : transform.position;
        }

        #endregion
    }
}