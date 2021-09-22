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
        [Header("Action Delays")]
        [SerializeField] private float _timeToNextFire = 0.5f;
        [SerializeField] private float _timeToNextWarp = 1.5f;
        [Header("Settings")]
        [SerializeField] private float _boltLookDistance = 20f;
        [SerializeField] private float _timeToFire = 0;
        [Header("External References")]
        [SerializeField] private BoltController _warpBolt;
        [Header("Internal References")]
        [SerializeField] private PlayerState _playerState;
        [SerializeField] private PlayerAnimator _playerAnimator;
        [SerializeField] private PlayerFeedback _playerFeedback;
        [SerializeField] private Transform _boltFirePosition = null;
        [SerializeField] private Transform _cameraLookDirection = null;

        private bool _warpAbility;
        private bool _residueAbility;
        private bool _lockCasting;
        private bool _lockWarp;

        #region Unity Functions

        private void OnEnable()
        {
            StateNullCheck();
            AnimatorNullCheck();
            FeedbackNullCheck();
            WarpBoltNullCheck();
            TransformNullCheck();

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
            if (_lockCasting || _missingWarpBolt) return;
            PrepareToCast();
            StartCoroutine(Cast());
            StartCoroutine(CastTimer());
        }

        public void ActivateBolt()
        {
            if (_lockWarp || _missingWarpBolt) return;
            if (_warpBolt.ResidueReady) {
                ActivateResidue();
            } else {
                Warp();
            }
            StartCoroutine(WarpTimer());
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
            _warpBolt.PrepareToFire(GetBoltPosition(), GetCameraRotation(), _residueAbility);
        }

        // The main Coroutine for casting the warp bolt
        private IEnumerator Cast()
        {
            _lockCasting = true;
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
            _lockCasting = false;
        }

        private IEnumerator CastTimer()
        {
            _lockCasting = true;
            yield return new WaitForSecondsRealtime(_timeToNextFire);
            _lockCasting = false;
        }

        private IEnumerator WarpTimer()
        {
            _lockWarp = true;
            yield return new WaitForSecondsRealtime(_timeToNextWarp);
            _lockWarp = false;
        }

        private void CastStatus(float status)
        {
            // Later send status info to Animator? Or other way around
            // We pull time to fire from the Animator **
            _warpBolt.SetCastStatus(status);
        }

        private void HoldPosition()
        {
            _warpBolt.SetPosition(GetBoltPosition(), GetCameraRotation());
        }

        private void Fire()
        {
            // Could tell animator to cast bolt, but it should be on the same page. Add check?
            _warpBolt.Fire(GetBoltPosition(), GetBoltForward());

            if (!_missingFeedback) {
                _playerFeedback.OnCastBolt();
            }
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

        private Quaternion GetCameraRotation()
        {
            return !_missingCamera ? _cameraLookDirection.rotation : Quaternion.identity;
        }

        private Vector3 GetBoltForward()
        {
            if (!_missingCamera) {
                return _boltFirePosition != null ? _boltFirePosition.forward : transform.forward;
            }

            Vector3 current = GetBoltPosition();
            Vector3 angle = GetRaycast() - current;
            return angle.normalized;
        }

        private Vector3 GetRaycast()
        {
            if (_missingCamera) return transform.position + transform.forward;

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
            return !_missingBoltFiringPosition ? _boltFirePosition.position : transform.position;
        }

        #endregion

        // -------------------------------------------------------------------------------------------

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


        private bool _missingFeedback;

        private void FeedbackNullCheck()
        {
            if (_playerFeedback == null) {
                _playerFeedback = transform.parent != null ? transform.parent.GetComponentInChildren<PlayerFeedback>() : GetComponent<PlayerFeedback>();
                if (_playerFeedback == null) {
                    _missingFeedback = true;
                    Debug.LogWarning("Cannot find the Player Feedback for the Player Casting Script", gameObject);
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

        private bool _missingCamera;
        private bool _missingBoltFiringPosition;

        private void TransformNullCheck()
        {
            Camera main = Camera.main;
            if (_boltFirePosition == null) {
                if (main != null) {
                    _boltFirePosition = main.transform;
                } else {
                    _missingBoltFiringPosition = true;
                    Debug.LogWarning("Cannot find Bolt Fire Transform", gameObject);
                }
            }
            if (_cameraLookDirection == null) {
                if (main != null) {
                    _cameraLookDirection = main.transform;
                } else {
                    _missingCamera = true;
                    Debug.LogWarning("Cannot find Camera Transform", gameObject);
                }
            }
        }

        #endregion
    }
}