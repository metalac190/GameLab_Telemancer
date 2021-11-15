using System;
using UnityEngine;

namespace Mechanics.Player
{
    /// Summary:
    /// The State of the Player
    /// This should link to PlayerPrefs State (Henry)
    public class PlayerState : MonoBehaviour
    {
        [SerializeField] private GameSettingsData _playerSettings;
        public static GameSettingsData Settings { get; private set; }
        [Header("Abilities")]
        [SerializeField] private bool _unlockedBolt = true;
        [SerializeField] private bool _unlockedWarp = false;
        [SerializeField] private bool _unlockedResidue = false;
        [Header("References")]
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private PlayerCasting _castingController;
        [SerializeField] private PlayerInteractions _interactionsController;
        [SerializeField] private PlayerAnimator _animationController;
        [SerializeField] private PlayerFeedback _playerFeedback;

        public event Action<bool, bool, bool> OnChangeUnlocks = delegate { };

        private bool _boltAbility;
        private bool _warpAbility;
        private bool _residueAbility;

        private Vector3 _defaultCheckpoint;
        private bool _locked;
        private bool _isAlive = true;
        private bool _isPaused = true;

        private bool FlagCantAct() => _locked || _isAlive && !_isPaused;

        #region Unity Functions

        private void OnValidate()
        {
            UpdateUnlocks();
        }

        private void Awake()
        {
            NullChecks();
        }

        private void Start()
        {
            _defaultCheckpoint = transform.position;
            UIEvents.current.OnPlayerRespawn += OnRespawn;
            UIEvents.current.OnPauseGame += OnGamePaused;
            _boltAbility = _unlockedBolt;
            _warpAbility = _unlockedWarp;
            _residueAbility = _unlockedResidue;
            UpdateUnlocks();
        }

        #endregion

        public void SetWatcherLocks(bool boltLocked, bool warpLocked, bool residueLocked)
        {
            if (boltLocked) {
                _boltAbility = false;
            }
            if (warpLocked) {
                _warpAbility = false;
            }
            if (residueLocked) {
                _residueAbility = false;
            }
            _playerFeedback.SetWatcherLock(true);
            UpdateUnlocks();
        }

        public void ResetWatcherLocks()
        {
            _boltAbility = _unlockedBolt;
            _warpAbility = _unlockedWarp;
            _residueAbility = _unlockedResidue;
            _playerFeedback.SetWatcherLock(false);
            UpdateUnlocks();
        }

        public void LockPlayer(bool locked)
        {
            _locked = locked;
            bool flagCantAct = FlagCantAct();
            _castingController.FlagCantAct = flagCantAct;
            _playerController.flag_cantAct = flagCantAct;
            _interactionsController.FlagCantAct = flagCantAct;
        }

        public void OnGamePaused(bool paused)
        {
            _isPaused = !paused;
            bool flagCantAct = FlagCantAct();
            _castingController.FlagCantAct = flagCantAct;
            _playerController.flag_cantAct = flagCantAct;
            _interactionsController.FlagCantAct = flagCantAct;
        }

        public void OnKill()
        {
            if (!_isAlive) return;
            _isAlive = false;
            _animationController.OnKill();
            UIEvents.current.PlayerDied();

            _playerController.flag_cantAct = true;
            _castingController.FlagCantAct = true;
        }

        public void OnRespawn()
        {
            _isAlive = true;
            _animationController.ResetToIdle();

            if (CheckpointManager.current == null) {
                _playerController.TeleportToPosition(_defaultCheckpoint);
            } else {
                _playerController.TeleportToPosition(CheckpointManager.current.RespawnPoint.position);
                // TODO: This might need to be moved to the player controller script?
                // Also set the player rotation on respawn
                _playerController.gameObject.transform.rotation = CheckpointManager.current.RespawnPoint.rotation;
            }

            bool flagCantAct = FlagCantAct();
            _castingController.FlagCantAct = flagCantAct;
            _playerController.flag_cantAct = flagCantAct;
            _interactionsController.FlagCantAct = flagCantAct;
            _castingController.HardResetBoltAndAnimator();
        }

        private void UpdateUnlocks()
        {
            OnChangeUnlocks.Invoke(_boltAbility, _warpAbility, _residueAbility);
        }

        #region Null Checks

        private void NullChecks()
        {
            NullCheckPlayerSettings();
            NullCheckPlayerController();
            NullCheckCastingController();
            NullCheckInteractionsController();
            NullCheckAnimationController();
            NullCheckFeedbackController();
        }

        private void NullCheckPlayerSettings()
        {
            if (Settings != null) _playerSettings = Settings;
            if (_playerSettings == null) {
                _playerSettings = FindObjectOfType<GameSettingsData>();
                if (_playerSettings == null) {
                    _playerSettings = new GameSettingsData();
                }
            }
            Settings = _playerSettings;
        }

        private void NullCheckPlayerController()
        {
            if (_playerController == null) {
                _playerController = GetComponent<PlayerController>();
                if (_playerController == null) {
                    _playerController = FindObjectOfType<PlayerController>();
                    if (_playerController == null) {
                        Debug.LogError("No Player Controller component found on player", gameObject);
                    }
                }
            }
        }

        private void NullCheckCastingController()
        {
            if (_castingController == null) {
                _castingController = GetComponentInChildren<PlayerCasting>();
                if (_castingController == null) {
                    _castingController = GetComponent<PlayerCasting>();
                    if (_castingController == null) {
                        Debug.LogError("No Player Casting component found on player", gameObject);
                    }
                }
            }
        }

        private void NullCheckInteractionsController()
        {
            if (_interactionsController == null) {
                _interactionsController = GetComponentInChildren<PlayerInteractions>();
                if (_interactionsController == null) {
                    _interactionsController = GetComponent<PlayerInteractions>();
                    if (_interactionsController == null) {
                        Debug.LogError("No Player Interactions component found on player", gameObject);
                    }
                }
            }
        }

        private void NullCheckAnimationController()
        {
            if (_animationController == null) {
                _animationController = GetComponentInChildren<PlayerAnimator>();
                if (_animationController == null) {
                    _animationController = GetComponent<PlayerAnimator>();
                    if (_animationController == null) {
                        Debug.LogError("No Player Animator component found on player", gameObject);
                    }
                }
            }
        }

        private void NullCheckFeedbackController()
        {
            if (_playerFeedback == null) {
                _playerFeedback = GetComponentInChildren<PlayerFeedback>();
                if (_playerFeedback == null) {
                    _playerFeedback = GetComponent<PlayerFeedback>();
                    if (_playerFeedback == null) {
                        Debug.LogError("No Player Feedback component found on player", gameObject);
                    }
                }
            }
        }

        #endregion
    }
}