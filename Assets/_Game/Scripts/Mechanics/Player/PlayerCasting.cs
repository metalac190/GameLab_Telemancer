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
        [Header("Extra Bolt Delay on Warp")]
        [SerializeField] private float _maxDistFromGround = 200;
        [SerializeField] private float _delayCastStartDist = 40;
        [Header("External References")]
        [SerializeField] private BoltManager _boltManagerPrefab = null;
        [Header("Internal References")]
        [SerializeField] private PlayerState _playerState;
        [SerializeField] private PlayerFeedback _playerFeedback;
        [SerializeField] private Transform _boltFirePosition;
        [SerializeField] private Transform _cameraLookDirection;

        private BoltManager _boltManager;
        private Coroutine _castRoutine;

        private bool _boltAbility;
        private bool _warpAbility;
        private bool _residueAbility;

        private bool _lockCasting;
        private bool _lockWarp;
        private bool _lockResidue;

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
                    _lockCasting = false;
                    _lockWarp = false;
                    _lockResidue = false;
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
                _playerFeedback.OnResidueAction(AbilityActionEnum.AttemptedUnsuccessful, true);
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

            if (PlayerState.Settings.ClearResidueOnFire) {
                _boltManager.DisableResidue();
                _playerFeedback.SetResidueState(AbilityStateEnum.Idle);
            }
            _castRoutine = StartCoroutine(Cast());
        }

        // The main Coroutine for casting the warp bolt
        private IEnumerator Cast()
        {
            _lockCasting = true;

            // Delay Casting
            yield return new WaitForSecondsRealtime(PlayerState.Settings.DelayBolt);
            _boltManager.PrepareToFire(GetBoltPosition(), GetBoltForward(), _residueAbility);

            // Time to cast
            if (PlayerState.Settings.TimeToFire > 0) {
                for (float t = 0; t <= PlayerState.Settings.TimeToFire; t += Time.deltaTime) {
                    if (_flagCantAct) yield break;
                    float delta = t / PlayerState.Settings.TimeToFire;
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
            float timer = PlayerState.Settings.TimeToNextBolt;
            _playerFeedback.SetBoltCooldown(timer);
            yield return new WaitForSecondsRealtime(timer);
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

            bool ready = _boltManager.PrepareToWarp();
            if (!ready) {
                _playerFeedback.OnWarpAction(AbilityActionEnum.AttemptedUnsuccessful);
                return;
            }

            _playerFeedback.OnWarpAction(AbilityActionEnum.InputDetected);

            StartCoroutine(Warp());
        }

        private IEnumerator Warp()
        {
            _lockWarp = true;
            yield return new WaitForSecondsRealtime(PlayerState.Settings.DelayWarp);
            OnWarp();
        }

        private void OnWarp()
        {
            _boltManager.OnWarp();
            _playerFeedback.OnWarpAction(AbilityActionEnum.Acted);
            _playerFeedback.SetWarpState(AbilityStateEnum.Idle);

            StartCoroutine(WarpTimer());
            if (PlayerState.Settings.BoltCooldownOnWarp) {
                StartCoroutine(WarpToBoltTimer());
            }
        }

        private IEnumerator WarpToBoltTimer()
        {
            _lockCasting = true;
            float timer = PlayerState.Settings.WarpTimeToNextBolt;
            float dist = GetDistanceFromGround();
            if (dist > _delayCastStartDist) {
                dist -= _delayCastStartDist;
                float additive = PlayerState.Settings.AdditiveTimePerHeight;
                timer += dist * additive;
            }
            _playerFeedback.SetBoltCooldown(timer);
            yield return new WaitForSecondsRealtime(timer);
            _lockCasting = false;
        }

        private IEnumerator WarpTimer()
        {
            _lockWarp = true;
            float timer = PlayerState.Settings.TimeToNextWarp;
            _playerFeedback.SetWarpCooldown(timer);
            yield return new WaitForSecondsRealtime(timer);
            _lockWarp = false;
        }

        #endregion

        #region Residue

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

            StartCoroutine(Residue());
        }

        private IEnumerator Residue()
        {
            _lockResidue = true;
            yield return new WaitForSecondsRealtime(PlayerState.Settings.DelayResidue);
            OnUseResidue();
        }

        private void OnUseResidue()
        {
            if (_boltManager.OnActivateResidue()) {
                _playerFeedback.OnResidueAction(AbilityActionEnum.Acted, true);
                _playerFeedback.SetResidueState(AbilityStateEnum.Idle);

                StartCoroutine(ResidueTimer());
            } else {
                _playerFeedback.OnResidueAction(AbilityActionEnum.AttemptedUnsuccessful, true);
                _lockResidue = false;
            }
        }

        private IEnumerator ResidueTimer()
        {
            _lockResidue = true;
            float timer = PlayerState.Settings.TimeToNextResidue;
            _playerFeedback.SetResidueCooldown(timer);
            yield return new WaitForSecondsRealtime(timer);
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