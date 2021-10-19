using System.Collections;
using Mechanics.Bolt;
using UnityEngine;
using UnityEngine.InputSystem;

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
        [Header("Action Animation Time")]
        [SerializeField] private float _delayBolt = 0;
        [SerializeField] private float _timeToFire = 0;
        [SerializeField] private float _delayWarp = 0;
        [SerializeField] private float _delayResidue = 0;
        [Header("Settings")]
        [SerializeField] private bool _clearResidueOnFire = true;
        [SerializeField] private float _boltLookDistance = 20f;
        [SerializeField] private LayerMask _lookAtMask = 1;
        [Header("External References")]
        [SerializeField] private BoltManager _boltManagerPrefab;
        [Header("Internal References")]
        [SerializeField] private PlayerState _playerState;
        [SerializeField] private PlayerFeedback _playerFeedback;
        [SerializeField] private Transform _boltFirePosition = null;
        [SerializeField] private Transform _cameraLookDirection = null;

        private BoltManager _boltManager;

        private bool _boltAbility;
        private bool _warpAbility;
        private bool _residueAbility;

        private bool _lockCasting;
        private bool _lockWarp;
        private bool _lockResidue;

        private bool _flagCantAct;
        public bool FlagCantAct
        {
            get => _flagCantAct;
            set
            {
                if (value) {
                    _boltManager.Dissipate();
                } else {
                    _lockCasting = false;
                    _lockWarp = false;
                    _lockResidue = false;
                }
                _flagCantAct = value;
            }
        }

        #region Unity Functions

        private void OnEnable()
        {
            NullChecks();

            if (!_missingState) {
                _playerState.OnChangeUnlocks += SetUnlocks;
            } else {
                SetUnlocks(false, false, false);
            }
            if (!_missingWarpBolt) {
                _boltManager.OnResidueReady += OnResidueReady;
                _boltManager.OnBoltDissipate += OnBoltDissipate;
            }
            FlagCantAct = false;
        }

        private void OnDisable()
        {
            if (!_missingState) {
                _playerState.OnChangeUnlocks -= SetUnlocks;
            }
            if (!_missingWarpBolt) {
                _boltManager.OnResidueReady -= OnResidueReady;
                _boltManager.OnBoltDissipate -= OnBoltDissipate;
            }
        }

        #endregion

        // -------------------------------------------------------------------------------------------

        #region Public Functions - Input

        public void CastBolt(InputAction.CallbackContext value)
        {
            // Default Checks for valid input
            if (FlagCantAct || !value.performed || _missingWarpBolt) return;

            // Ensure that player has bolt ability
            if (!_boltAbility) return;

            // Ensure that casting is not locked
            if (_lockCasting) {
                _playerFeedback.OnBoltAction(AbilityActionEnum.AttemptedUnsuccessful);
                return;
            }

            // Attempt to Cast Bolt
            PrepareToCast();
        }

        public void ActivateWarp(InputAction.CallbackContext value)
        {
            // Default Checks for valid input
            if (FlagCantAct || !value.performed || _missingWarpBolt) return;

            // Ensure that player has warp ability
            if (!_warpAbility) return;

            // Ensure that warping is not locked
            if (_lockWarp) {
                _playerFeedback.OnWarpAction(AbilityActionEnum.AttemptedUnsuccessful);
                return;
            }

            // Attempt to Warp
            PrepareToWarp();
        }

        public void ActivateResidue(InputAction.CallbackContext value)
        {
            // Default Checks for valid input
            if (FlagCantAct || !value.performed || _missingWarpBolt) return;

            // Ensure that player has residue ability and warp bolt exists
            if (!_residueAbility) return;

            // Ensure that residue is not locked
            if (_lockResidue) {
                _playerFeedback.OnResidueAction(AbilityActionEnum.AttemptedUnsuccessful);
                return;
            }

            // Attempt to Activate Residue
            PrepareForResidue();
        }

        #endregion

        // -------------------------------------------------------------------------------------------

        #region Warp Bolt Casting

        private void PrepareToCast()
        {
            _playerFeedback.OnBoltAction(AbilityActionEnum.InputDetected);

            if (_clearResidueOnFire) {
                _boltManager.DisableResidue();
                _playerFeedback.SetResidueState(AbilityStateEnum.Idle);
            }
            StartCoroutine(Cast());
        }

        // The main Coroutine for casting the warp bolt
        private IEnumerator Cast()
        {
            _lockCasting = true;

            // Delay Casting
            yield return new WaitForSecondsRealtime(_delayBolt);
            _boltManager.PrepareToFire(GetBoltPosition(), GetBoltForward(), _residueAbility);

            // Time to cast
            if (_timeToFire > 0) {
                for (float t = 0; t <= _timeToFire; t += Time.deltaTime) {
                    if (_flagCantAct) yield break;
                    float delta = t / _timeToFire;
                    CastStatus(delta);
                    HoldPosition();
                    yield return null;
                }
            }

            // Cast
            CastStatus(1);
            Fire();
        }

        private void CastStatus(float status)
        {
            _boltManager.SetCastStatus(status);
        }

        private void HoldPosition()
        {
            _boltManager.SetPosition(GetBoltPosition(), GetBoltForward());
        }

        private void Fire()
        {
            _boltManager.Fire(GetBoltPosition(), GetBoltForward());

            _playerFeedback.OnBoltAction(AbilityActionEnum.Acted);
            if (_warpAbility) {
                _playerFeedback.SetWarpState(AbilityStateEnum.Ready);
            }

            StartCoroutine(CastTimer());
        }

        private IEnumerator CastTimer()
        {
            _lockCasting = true;
            _playerFeedback.SetBoltCooldown(_timeToNextFire);
            yield return new WaitForSecondsRealtime(_timeToNextFire);
            _lockCasting = false;
        }

        #endregion

        #region Warping

        private void PrepareToWarp()
        {
            if (!_boltManager.CanWarp) {
                _playerFeedback.OnWarpAction(AbilityActionEnum.AttemptedUnsuccessful);
                return;
            }

            _playerFeedback.OnWarpAction(AbilityActionEnum.InputDetected);

            StartCoroutine(Warp());
        }

        private IEnumerator Warp()
        {
            _lockWarp = true;
            yield return new WaitForSecondsRealtime(_delayWarp);
            OnWarp();
        }

        private void OnWarp()
        {
            if (_boltManager.OnWarp()) {
                _playerFeedback.OnWarpAction(AbilityActionEnum.Acted);
                _playerFeedback.SetWarpState(AbilityStateEnum.Idle);

                StartCoroutine(WarpTimer());
            } else {
                _playerFeedback.OnWarpAction(AbilityActionEnum.AttemptedUnsuccessful);
                _lockWarp = false;
            }
        }

        private IEnumerator WarpTimer()
        {
            _lockWarp = true;
            _playerFeedback.SetWarpCooldown(_timeToNextWarp);
            yield return new WaitForSecondsRealtime(_timeToNextWarp);
            _lockWarp = false;
        }

        #endregion

        #region Residue

        private void PrepareForResidue()
        {
            if (!_boltManager.ResidueReady) {
                _playerFeedback.OnResidueAction(AbilityActionEnum.AttemptedUnsuccessful);
                return;
            }

            _playerFeedback.OnResidueAction(AbilityActionEnum.InputDetected);

            StartCoroutine(Residue());
        }

        private IEnumerator Residue()
        {
            _lockResidue = true;
            yield return new WaitForSecondsRealtime(_delayResidue);
            OnUseResidue();
        }

        private void OnUseResidue()
        {
            if (_boltManager.OnActivateResidue()) {
                _playerFeedback.OnResidueAction(AbilityActionEnum.Acted);
                _playerFeedback.SetResidueState(AbilityStateEnum.Idle);

                StartCoroutine(ResidueTimer());
            } else {
                _playerFeedback.OnResidueAction(AbilityActionEnum.AttemptedUnsuccessful);
                _lockResidue = false;
            }
        }

        private IEnumerator ResidueTimer()
        {
            _lockResidue = true;
            _playerFeedback.SetResidueCooldown(_timeToNextResidue);
            yield return new WaitForSecondsRealtime(_timeToNextResidue);
            _lockResidue = false;
        }

        #endregion

        #region Private Functions

        // Controlled by Player State
        private void SetUnlocks(bool bolt, bool warp, bool residue)
        {
            _boltAbility = bolt;
            _warpAbility = warp;
            _residueAbility = residue;

            _playerFeedback.OnUpdateUnlockedAbilities(bolt, warp, residue);
        }

        private void OnResidueReady()
        {
            _playerFeedback.SetResidueState(AbilityStateEnum.Ready);
        }

        private void OnBoltDissipate(bool residueReady)
        {
            _playerFeedback.SetWarpState(AbilityStateEnum.Idle);
            _playerFeedback.OnBoltDissipate(residueReady);
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

        private void NullChecks()
        {
            StateNullCheck();
            FeedbackNullCheck();
            WarpBoltNullCheck();
            TransformNullCheck();
        }

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
            if (_boltManager != null) return;
            _boltManager = FindObjectOfType<BoltManager>();
            if (_boltManager != null) return;
            if (_boltManagerPrefab != null) {
                if (_boltManagerPrefab.gameObject.activeInHierarchy) {
                    _boltManager = _boltManagerPrefab;
                } else {
                    _boltManager = Instantiate(_boltManagerPrefab);
                    Debug.LogWarning("No Bolt Manager in scene, but one was referenced by the player. Instantiating", gameObject);
                }
                return;
            }
            _missingWarpBolt = true;
            throw new MissingComponentException("Missing the Warp Bolt Reference on the Player Casting Script on " + gameObject);
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