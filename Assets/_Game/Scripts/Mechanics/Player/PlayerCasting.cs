using System.Collections;
using Mechanics.Bolt;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Mechanics.Player
{
    /// Summary:
    /// The controller for the Player Casting Sequence.
    /// Requires a reference to the WarpRoutine Bolt and a reference to the Player Animator
    /// Public functions are called by the Player Input System
    public class PlayerCasting : MonoBehaviour
    {
        [Header("Extra Bolt Delay on WarpRoutine")]
        [SerializeField] private float _maxDistFromGround = 200;
        [SerializeField] private float _delayCastAirDist = 5;
        [SerializeField] private float _delayCastAirTime = 1.25f;
        [Header("External References")]
        [SerializeField] private BoltManager _boltManagerPrefab = null;
        [Header("Internal References")]
        [SerializeField] private PlayerState _playerState;
        [SerializeField] private PlayerFeedback _playerFeedback;
        [SerializeField] private Transform _boltFirePosition;
        [SerializeField] private Transform _cameraLookDirection;

        private BoltManager _boltManager;
        private Coroutine _castRoutine;

        private bool _boltUnlocked;
        private bool _warpUnlocked;
        private bool _residueUnlocked;

        private bool _boltLock;
        private float _boltDelay;
        private float _boltDelayStart;
        private Coroutine _boltDelayRoutine;

        private bool _warpLock;
        private float _warpDelay;
        private float _warpDelayStart;
        private Coroutine _warpDelayRoutine;

        private bool _residueLock;
        private float _residueDelay;
        private float _residueDelayStart;
        private Coroutine _residueDelayRoutine;

        #region Flag Cant Act

        private bool _flagCantAct;

        public bool FlagCantAct
        {
            get => _flagCantAct;
            set
            {
                if (value) {
                    _boltManager.OnGamePaused();
                    _playerFeedback.OnGamePaused();
                    if (_castRoutine != null) {
                        StopCoroutine(_castRoutine);
                    }
                } else {
                    _boltLock = false;
                    _warpLock = false;
                    _residueLock = false;
                }
                _flagCantAct = value;
            }
        }

        #endregion

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
                _boltManager.OnBoltDissipate -= OnBoltDissipate;
            }
        }

        #endregion

        // -------------------------------------------------------------------------------------------

        #region Public Functions - Input

        public void CastBolt(InputAction.CallbackContext value)
        {
            if (FlagCantAct || !value.performed || _missingWarpBolt) return;
            AttemptToCastBolt();
        }

        public void ActivateWarp(InputAction.CallbackContext value)
        {
            if (FlagCantAct || !value.performed || _missingWarpBolt) return;
            AttemptToWarp();
        }

        public void ActivateResidue(InputAction.CallbackContext value)
        {
            if (FlagCantAct || !value.performed || _missingWarpBolt) return;
            AttemptToActivateResidue();
        }

        #endregion

        // -------------------------------------------------------------------------------------------

        #region Bolt Casting

        private void AttemptToCastBolt()
        {
            // Ensure that player has bolt ability
            if (!_boltUnlocked) return;

            // Ensure that casting is not locked
            if (_boltLock) {
                _playerFeedback.OnBoltAction(AbilityActionEnum.AttemptedUnsuccessful);
                return;
            }

            // Attempt to CastRoutine Bolt
            PrepareToCast();
        }

        private void PrepareToCast()
        {
            _playerFeedback.OnBoltAction(AbilityActionEnum.InputDetected);

            if (PlayerState.Settings.ClearResidueOnFire) {
                _boltManager.DisableResidue();
                _playerFeedback.SetResidueState(AbilityStateEnum.Idle);
            }
            _castRoutine = StartCoroutine(CastRoutine());
        }

        // The main Coroutine for casting the warp bolt
        private IEnumerator CastRoutine()
        {
            _boltLock = true;

            // Delay Casting (Allows for some animation time)
            yield return new WaitForSecondsRealtime(PlayerState.Settings.DelayBolt);
            if (_flagCantAct) yield break;

            _boltManager.PrepareToFire(GetBoltPosition(), GetBoltForward(), _residueUnlocked);

            // Time to cast (Holds bolt in position for given time. Allows for additional animation time)
            if (PlayerState.Settings.TimeToFire > 0) {
                for (float t = 0; t <= PlayerState.Settings.TimeToFire; t += Time.deltaTime) {
                    if (_flagCantAct) yield break;
                    SetCastDelta(t / PlayerState.Settings.TimeToFire);
                    yield return null;
                }
            }
            SetCastDelta(1);
            FireBolt();
        }

        private void SetCastDelta(float status)
        {
            _boltManager.SetCastStatus(status);
            _boltManager.SetPosition(GetBoltPosition(), GetBoltForward());
        }

        private void FireBolt()
        {
            _boltManager.Fire(GetBoltPosition(), GetBoltForward());

            _playerFeedback.OnBoltAction(AbilityActionEnum.Acted);
            if (_warpUnlocked) {
                _playerFeedback.SetWarpState(AbilityStateEnum.Ready);
            }

            float airTime = _playerFeedback.GetAirTime();
            float airDistance = GetDistanceFromGround();
            if (airTime >= _delayCastAirTime && airDistance >= _delayCastAirDist) {
                Debug.Log("Flying is not allowed");
                AddWarpDelay(PlayerState.Settings.ExtraWarpTimeInAir);
            }

            // Delay Bolt
            AddBoltDelay(PlayerState.Settings.TimeToNextBolt);
        }

        private void AddBoltDelay(float delay)
        {
            if (_boltDelayRoutine != null) {
                // Bolt delay exists. Add to current bolt delay
                _boltDelay += delay;
            } else {
                // Bolt delay does not exist. Create new bolt delay
                _boltDelay = delay;
                _boltDelayStart = Time.time;
                _boltDelayRoutine = StartCoroutine(DelayBoltRoutine());
            }
        }

        private IEnumerator DelayBoltRoutine()
        {
            _boltLock = true;
            _playerFeedback.StartBoltCooldown();
            while (Time.time < _boltDelayStart + _boltDelay) {
                float delta = Mathf.Clamp01((Time.time - _boltDelayStart) / _boltDelay);
                _playerFeedback.SetBoltCooldown(delta);
                yield return null;
            }
            _playerFeedback.SetBoltCooldown(1);
            _playerFeedback.EndBoltCooldown();
            _boltLock = false;
            _boltDelayRoutine = null;
        }

        #endregion

        #region Warping

        private void AttemptToWarp()
        {
            // Ensure that player has warp ability
            if (!_warpUnlocked) return;

            // Ensure that warping is not locked
            if (_warpLock) {
                _playerFeedback.OnWarpAction(AbilityActionEnum.AttemptedUnsuccessful);
                return;
            }

            // Attempt to WarpRoutine
            PrepareToWarp();
        }

        private void PrepareToWarp()
        {
            if (!_boltManager.CanWarp) {
                _playerFeedback.OnWarpAction(AbilityActionEnum.AttemptedUnsuccessful);
                return;
            }

            bool ready = _boltManager.PrepareToWarp();
            if (!ready) {
                _playerFeedback.OnWarpAction(AbilityActionEnum.AttemptedUnsuccessful);
                return;
            }

            _playerFeedback.OnWarpAction(AbilityActionEnum.InputDetected);

            StartCoroutine(WarpRoutine());
        }

        private IEnumerator WarpRoutine()
        {
            _warpLock = true;
            float warpDelay = PlayerState.Settings.DelayWarp;
            for (float t = 0; t < warpDelay; t += Time.deltaTime) {
                yield return null;
            }
            Warp();
        }

        private void Warp()
        {
            _boltManager.OnWarp();
            _playerFeedback.OnWarpAction(AbilityActionEnum.Acted);
            _playerFeedback.SetWarpState(AbilityStateEnum.Idle);

            // Delay warp
            AddWarpDelay(PlayerState.Settings.TimeToNextWarp);


            // Delay Bolt
            if (PlayerState.Settings.BoltCooldownOnWarp) {
                AddBoltDelay(PlayerState.Settings.BoltTimeAfterWarp);
            }
        }

        private void AddWarpDelay(float delay)
        {
            if (_warpDelayRoutine != null) {
                // Bolt delay exists. Add to current bolt delay
                _warpDelay += delay;
            } else {
                // Bolt delay does not exist. Create new bolt delay
                _warpDelay = delay;
                _warpDelayStart = Time.time;
                _warpDelayRoutine = StartCoroutine(DelayWarpRoutine());
            }
        }

        private IEnumerator DelayWarpRoutine()
        {
            _warpLock = true;
            _playerFeedback.StartWarpCooldown();
            while (Time.time < _warpDelayStart + _warpDelay) {
                float delta = Mathf.Clamp01((Time.time - _warpDelayStart) / _warpDelay);
                _playerFeedback.SetWarpCooldown(delta);
                yield return null;
            }
            _playerFeedback.SetWarpCooldown(1);
            _playerFeedback.EndWarpCooldown();
            _warpLock = false;
            _warpDelayRoutine = null;
        }

        #endregion

        #region ResidueRoutine

        private void AttemptToActivateResidue()
        {
            // Ensure that player has residue ability and warp bolt exists
            if (!_residueUnlocked) return;

            // Ensure that residue is not locked
            if (_residueLock) {
                _playerFeedback.OnResidueAction(AbilityActionEnum.AttemptedUnsuccessful, true);
                return;
            }

            // Attempt to Activate ResidueRoutine
            PrepareForResidue();
        }

        private void PrepareForResidue()
        {
            if (!_boltManager.ResidueReady) {
                _playerFeedback.OnResidueAction(AbilityActionEnum.AttemptedUnsuccessful, true);
                return;
            }

            if (_boltManager.ReturnAnimationToHold) {
                _playerFeedback.OnResidueRelayAnimation();
            } else {
                _playerFeedback.OnResidueAction(AbilityActionEnum.InputDetected, true);
            }

            StartCoroutine(ResidueRoutine());
        }

        private IEnumerator ResidueRoutine()
        {
            _residueLock = true;
            float residueDelay = PlayerState.Settings.DelayResidue;
            for (float t = 0; t < residueDelay; t += Time.deltaTime) {
                yield return null;
            }
            OnUseResidue();
        }

        private void OnUseResidue()
        {
            if (_boltManager.OnActivateResidue()) {
                _playerFeedback.OnResidueAction(AbilityActionEnum.Acted, true);
                _playerFeedback.SetResidueState(AbilityStateEnum.Idle);

                AddResidueDelay(PlayerState.Settings.TimeToNextResidue);
            } else {
                _playerFeedback.OnResidueAction(AbilityActionEnum.AttemptedUnsuccessful, true);
                _residueLock = false;
            }
        }

        private void AddResidueDelay(float delay)
        {
            if (_residueDelayRoutine != null) {
                // Bolt delay exists. Add to current bolt delay
                _residueDelay += delay;
            } else {
                // Bolt delay does not exist. Create new bolt delay
                _residueDelay = delay;
                _residueDelayStart = Time.time;
                _residueDelayRoutine = StartCoroutine(DelayResidueRoutine());
            }
        }

        private IEnumerator DelayResidueRoutine()
        {
            _residueLock = true;
            _playerFeedback.StartResidueCooldown();
            while (Time.time < _residueDelayStart + _residueDelay) {
                float delta = Mathf.Clamp01((Time.time - _residueDelayStart) / _residueDelay);
                _playerFeedback.SetResidueCooldown(delta);
                yield return null;
            }
            _playerFeedback.SetResidueCooldown(1);
            _playerFeedback.EndResidueCooldown();
            _residueLock = false;
            _residueDelayRoutine = null;
        }

        #endregion

        // -------------------------------------------------------------------------------------------

        #region Private Functions

        // Controlled by Player State
        private void SetUnlocks(bool bolt, bool warp, bool residue)
        {
            _boltUnlocked = bolt;
            _warpUnlocked = warp;
            _residueUnlocked = residue;

            _playerFeedback.OnUpdateUnlockedAbilities(bolt, warp, residue);
        }

        private void OnBoltDissipate(bool residueReady)
        {
            _playerFeedback.SetWarpState(AbilityStateEnum.Idle);
            _playerFeedback.OnBoltDissipate(residueReady);
            if (residueReady) {
                _playerFeedback.SetResidueState(AbilityStateEnum.Ready);
            }
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

        private float GetDistanceFromGround()
        {
            Ray ray = new Ray(transform.position, Vector3.down);
            Physics.Raycast(ray, out var hit, _maxDistFromGround, PlayerState.Settings.LookAtMask, QueryTriggerInteraction.Ignore);
            return hit.distance;
        }

        private Vector3 GetRaycast()
        {
            if (_missingCamera) return transform.position + transform.forward;

            Ray ray = new Ray(_cameraLookDirection.position, _cameraLookDirection.forward);
            Physics.Raycast(ray, out var hit, PlayerState.Settings.MaxLookDistance, PlayerState.Settings.LookAtMask, QueryTriggerInteraction.Ignore);

            if (hit.point != Vector3.zero) {
                return hit.point;
            }
            return _cameraLookDirection.position + _cameraLookDirection.forward * PlayerState.Settings.MaxLookDistance;
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
            throw new MissingComponentException("Missing the WarpRoutine Bolt Reference on the Player Casting Script on " + gameObject);
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
                    Debug.LogWarning("Cannot find Bolt FireBolt Transform", gameObject);
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