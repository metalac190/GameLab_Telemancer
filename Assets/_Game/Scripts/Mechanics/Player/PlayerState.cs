using System;
using UnityEngine;
using UnityEngine.Events;

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
        [Header("Death and Respawn")]
        [SerializeField] private Vector3 _defaultCheckpoint = Vector3.zero;
        [SerializeField] private UnityEvent _onPlayerDeath = new UnityEvent();
        [SerializeField] private UnityEvent _onPlayerRespawn = new UnityEvent();
        [Header("References")]
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private PlayerCasting _castingController;
        [SerializeField] private PlayerFeedback _playerFeedback;

        public event Action<bool, bool, bool> OnChangeUnlocks = delegate { };

        private bool _boltAbility;
        private bool _warpAbility;
        private bool _residueAbility;

        private bool _locked;
        private bool _isAlive = true;
        private bool _isPaused = true;

        private bool FlagCantAct() => _locked || (_isAlive && _isPaused);

        #region Unity Functions

        private void OnValidate()
        {
            UpdateUnlocks();
        }

        private void Awake()
        {
            NullCheck();
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
            _castingController.FlagCantAct = FlagCantAct();
            _playerController.flag_cantAct = FlagCantAct();
        }

        public void OnGamePaused(bool paused)
        {
            _isPaused = paused;
            _castingController.FlagCantAct = FlagCantAct();
            _playerController.flag_cantAct = FlagCantAct();
        }

        public void OnKill()
        {
            if (!_isAlive) return;
            _isAlive = false;
            _onPlayerDeath.Invoke();
            UIEvents.current.PlayerDied();

            _playerController.flag_cantAct = true;
            _castingController.FlagCantAct = true;
        }

        public void OnRespawn()
        {
            _isAlive = true;
            _onPlayerRespawn.Invoke();

            if (CheckpointManager.current == null) {
                _playerController.TeleportToPosition(_defaultCheckpoint);
                return;
            }

            _playerController.TeleportToPosition(CheckpointManager.current.RespawnPoint.position);
            // TODO: This might need to be moved to the player controller script?
            // Also set the player rotation on respawn
            _playerController.gameObject.transform.rotation = CheckpointManager.current.RespawnPoint.rotation;

            _playerController.flag_cantAct = FlagCantAct();
            _castingController.FlagCantAct = FlagCantAct();
            _castingController.HardResetBoltAndAnimator();
        }

        private void UpdateUnlocks()
        {
            OnChangeUnlocks.Invoke(_boltAbility, _warpAbility, _residueAbility);
        }

        #region Null Checks

        private void NullCheck()
        {
            if (Settings != null) _playerSettings = Settings;
            if (_playerSettings == null) {
                _playerSettings = FindObjectOfType<GameSettingsData>();
                if (_playerSettings == null) {
                    _playerSettings = new GameSettingsData();
                }
            }
            Settings = _playerSettings;
            _playerController = GetComponent<PlayerController>();
            if (_playerController == null) {
                _playerController = GetComponent<PlayerController>();
                if (_playerController == null) {
                    _playerController = FindObjectOfType<PlayerController>();
                    if (_playerController == null) {
                        _playerController = gameObject.AddComponent<PlayerController>();
                    }
                }
            }
            if (_castingController == null) {
                _castingController = GetComponentInChildren<PlayerCasting>();
                if (_castingController == null) {
                    _castingController = GetComponent<PlayerCasting>();
                    if (_castingController == null) {
                        Debug.LogError("No Player Casting component found on player");
                    }
                }
            }
            if (_playerFeedback == null) {
                _playerFeedback = GetComponentInChildren<PlayerFeedback>();
                if (_playerFeedback == null) {
                    _playerFeedback = GetComponent<PlayerFeedback>();
                    if (_playerFeedback == null) {
                        Debug.LogError("No Player Feedback component found on player");
                    }
                }
            }
        }

        #endregion
    }
}