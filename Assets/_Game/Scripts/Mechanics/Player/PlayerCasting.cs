using System.Collections;
using Mechanics.WarpBolt;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Mechanics.Player
{
    /// Summary:
    /// The controller for the Player Casting Sequence.
    /// Requires a reference to the Warp Bolt and a reference to the Player Animator
    /// Public functions are called by the Player Input System
    public class PlayerCasting : MonoBehaviour
    {
        [Header("Action Delays")]
        [SerializeField] private float _timeToNextFire = 0.5f;
        [SerializeField] private float _timeToNextWarp = 1.5f;
        [SerializeField] private float _timeToNextResidue = 1.5f;
        [Header("Settings")]
        [SerializeField] private bool _clearResidueOnFire = true;
        [SerializeField] private float _boltLookDistance = 20f;
        [SerializeField] private float _timeToFire = 0;
        [SerializeField] private LayerMask _lookAtMask = 1;
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
        private bool _lockResidue;

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
            if (!_missingWarpBolt) {
                _warpBolt.OnResidueReady += OnResidueReady;
                _warpBolt.OnWarpDissipate += OnWarpDissipate;
            }
        }

        private void OnDisable()
        {
            if (!_missingState) {
                _playerState.OnChangeUnlocks -= SetUnlocks;
            }
            if (!_missingWarpBolt) {
                _warpBolt.OnResidueReady -= OnResidueReady;
                _warpBolt.OnWarpDissipate -= OnWarpDissipate;
            }
        }

        private void Update()
        {
            // Update HUD color
            GetRaycast();
        }

        #endregion

        // -------------------------------------------------------------------------------------------

        #region Public Functions - Input

        public void CastBolt(InputAction.CallbackContext value)
        {
            if (!value.performed || _missingWarpBolt) return;
            // Ensure that casting is not locked and warp bolt exists
            if (_lockCasting) {
                _playerFeedback.OnPrepareToCast(false);
                return;
            }
            PrepareToCast();
            StartCoroutine(Cast());
            StartCoroutine(CastTimer());

            _playerFeedback.OnPrepareToCast();
        }

        public void ActivateWarp(InputAction.CallbackContext value)
        {
            if (!value.performed || _missingWarpBolt) return;
            // Ensure that player has warp ability and warp bolt exists
            if (!_warpAbility) return;

            // Lock the warp if it was successful
            if (!_lockWarp && _warpBolt.OnWarp()) {
                StartCoroutine(WarpTimer());

                _playerFeedback.OnActivateWarp();
            } else {
                _playerFeedback.OnActivateWarp(false);
            }
        }

        public void ActivateResidue(InputAction.CallbackContext value)
        {
            if (!value.performed || _missingWarpBolt) return;
            // Ensure that player has residue ability and warp bolt exists
            if (!_residueAbility) return;

            // Lock the residue if it was successful
            if (!_lockResidue && _warpBolt.OnActivateResidue()) {
                StartCoroutine(ResidueTimer());

                _playerFeedback.OnActivateResidue();
            } else {
                _playerFeedback.OnActivateResidue(false);
            }
        }

        #endregion

        // -------------------------------------------------------------------------------------------

        #region Warp Bolt Casting

        private void PrepareToCast()
        {
            _warpBolt.PrepareToFire(GetBoltPosition(), GetBoltForward(), _residueAbility);
            if (_clearResidueOnFire) {
                _warpBolt.DisableResidue();
                _playerFeedback.OnResidueReady(false);
            }
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

        private void CastStatus(float status)
        {
            // Later send status info to Animator? Or other way around
            // We pull time to fire from the Animator **
            _warpBolt.SetCastStatus(status);
        }

        private void HoldPosition()
        {
            _warpBolt.SetPosition(GetBoltPosition(), GetBoltForward());
        }

        private void Fire()
        {
            // Could tell animator to cast bolt, but it should be on the same page. Add check?
            _warpBolt.Fire(GetBoltPosition(), GetBoltForward());

            _playerFeedback.OnCastBolt();
            if (_warpAbility) {
                _playerFeedback.OnWarpReady();
            }
        }

        #endregion

        #region Private Functions

        // Controlled by Player State
        private void SetUnlocks(bool warp, bool residue)
        {
            _warpAbility = warp;
            _residueAbility = residue;

            _playerFeedback.OnUpdateUnlockedAbilities(warp, residue);
        }

        private void OnResidueReady()
        {
            _playerFeedback.OnResidueReady();
        }

        private void OnWarpDissipate()
        {
            _playerFeedback.OnWarpReady(false);
        }

        #endregion

        #region Timers

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

        private IEnumerator ResidueTimer()
        {
            _lockResidue = true;
            yield return new WaitForSecondsRealtime(_timeToNextResidue);
            _lockResidue = false;
        }

        #endregion

        #region Helper Functions

        private Vector3 GetBoltForward()
        {
            if (_missingCamera) {
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
            Physics.Raycast(ray, out var hit, _boltLookDistance, _lookAtMask, QueryTriggerInteraction.Ignore);

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

        private void AnimatorNullCheck()
        {
            if (_playerAnimator == null) {
                _playerAnimator = transform.parent != null ? transform.parent.GetComponentInChildren<PlayerAnimator>() : GetComponent<PlayerAnimator>();
                if (_playerAnimator == null) {
                    _playerAnimator = gameObject.AddComponent<PlayerAnimator>();
                    Debug.LogWarning("Cannot find the Player Animator for the Player Casting Script", gameObject);
                }
            }
        }

        private void FeedbackNullCheck()
        {
            if (_playerFeedback == null) {
                _playerFeedback = transform.parent != null ? transform.parent.GetComponentInChildren<PlayerFeedback>() : GetComponent<PlayerFeedback>();
                if (_playerFeedback == null) {
                    _playerFeedback = gameObject.AddComponent<PlayerFeedback>();
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