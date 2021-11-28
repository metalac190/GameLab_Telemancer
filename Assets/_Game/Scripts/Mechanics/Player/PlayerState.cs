using System;
using Mechanics.Player.Feedback.Options;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
        [SerializeField] private PlayerOptionsController _optionController;

        public event Action<bool, bool, bool> OnChangeUnlocks = delegate { };

        private bool _boltAbility;
        private bool _warpAbility;
        private bool _residueAbility;
        private int _boltAbilityState;
        private int _warpAbilityState;
        private int _residueAbilityState;

        private Vector3 _defaultCheckpoint;
        private bool _locked;
        private bool _invincible;
        private bool _isAlive = true;
        private bool _isPaused = true;

        private bool FlagCantAct() => _locked || _isAlive && !_isPaused;

        #region Unity Functions

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
            CheckExistingCode();
        }

        private int _stage;

        private void Update()
        {
            CheckCode();
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
            if (!_isAlive || _invincible) return;
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
            bool bolt = _boltAbilityState == 0 ? _boltAbility : _boltAbilityState > 0;
            bool warp = _warpAbilityState == 0 ? _warpAbility : _warpAbilityState > 0;
            bool residue = _residueAbilityState == 0 ? _residueAbility : _residueAbilityState > 0;
            OnChangeUnlocks.Invoke(bolt, warp, residue);
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

        #region Konami Code

        private bool _codeActive;
        private bool _resetGamePausing;
        private PlayerOptionsController _codeController;

        private void CheckCode()
        {
            if (FlagCantAct()) return;
            if (_codeActive) {
                if (Keyboard.current.cKey.wasPressedThisFrame) {
                    EnableCodeMenu(!_codeController.isActiveAndEnabled);
                } else if (_codeController.isActiveAndEnabled && Keyboard.current.escapeKey.wasPressedThisFrame) {
                    _resetGamePausing = true;
                    EnableCodeMenu(false);
                } else if (_resetGamePausing && Keyboard.current.escapeKey.wasReleasedThisFrame) {
                    UIEvents.current.EnableGamePausing();
                    _resetGamePausing = false;
                }
            }
            bool success = false;
            switch (_stage) {
                case 0:
                case 1:
                    if (Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame) success = true;
                    break;
                case 2:
                case 3:
                    if (Keyboard.current.sKey.wasPressedThisFrame || Keyboard.current.downArrowKey.wasPressedThisFrame) success = true;
                    break;
                case 4:
                case 6:
                    if (Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.leftArrowKey.wasPressedThisFrame) success = true;
                    break;
                case 5:
                case 7:
                    if (Keyboard.current.dKey.wasPressedThisFrame || Keyboard.current.rightArrowKey.wasPressedThisFrame) success = true;
                    break;
                case 8:
                    if (Keyboard.current.qKey.wasPressedThisFrame || Keyboard.current.bKey.wasPressedThisFrame) success = true;
                    break;
                case 9:
                    if (Keyboard.current.eKey.wasPressedThisFrame || Keyboard.current.aKey.wasPressedThisFrame) success = true;
                    break;
                case 10:
                    if (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame) success = true;
                    break;
            }
            if (success) {
                _stage++;
                if (_stage > 10) ActivateCode();
            } else if (Keyboard.current.anyKey.wasPressedThisFrame) {
                _stage = Keyboard.current.wKey.wasPressedThisFrame ? 1 : 0;
            }
        }

        private void CheckExistingCode()
        {
            _codeController = PlayerOptionsController.Instance;
            if (_codeController == null) return;
            _codeActive = true;
            RefreshCode();
        }

        private void ActivateCode()
        {
            _codeActive = true;
            CheckCodeController();
            RefreshCode();
            EnableCodeMenu(true);
            AchievementManager.current.unlockAchievement(AchievementManager.Achievements.KonamiCode);
        }

        private void EnableCodeMenu(bool enable)
        {
            _codeController.gameObject.SetActive(enable);
            if (enable) {
                UIEvents.current.DisableGamePausing();
            } else if (!_resetGamePausing) {
                UIEvents.current.EnableGamePausing();
            }
            Time.timeScale = enable ? 0 : 1;
            Cursor.lockState = enable ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = enable;
            bool flagCantAct = FlagCantAct() || enable;
            _castingController.FlagCantAct = flagCantAct;
            _playerController.flag_cantAct = flagCantAct;
            _interactionsController.FlagCantAct = flagCantAct;
        }

        private void CheckCodeController()
        {
            if (_codeController == null) {
                _codeController = PlayerOptionsController.Instance;
                if (_codeController == null) {
                    _codeController = Instantiate(_optionController);
                    DontDestroyOnLoad(_codeController);
                }
            }
        }

        private void RefreshCode()
        {
            _codeController.LevelSelector.OnChangeLevel += ChangeLevel;
            _codeController.Invincibility.OnSelect += SetInvincibility;
            _codeController.InfiniteJumps.OnSelect += SetInfiniteJumps;
            _codeController.BoltUnlocked.OnSelect += SetBoltUnlocked;
            _codeController.WarpUnlocked.OnSelect += SetWarpUnlocked;
            _codeController.ResidueUnlocked.OnSelect += SetResidueUnlocked;
            _codeController.NoBoltCooldown.OnSelect += SetNoBoltCooldown;
            _codeController.NoWarpCooldown.OnSelect += SetNoWarpCooldown;
            _codeController.NoResidueCooldown.OnSelect += SetNoResidueCooldown;
            _codeController.InfiniteBoltDistance.OnSelect += SetInfiniteBoltDistance;
            _codeController.BoltMoveSpeed.OnSetValue += SetBoltMoveSpeed;
            _codeController.MortalTed.OnSelect += SetTedMortal;
            _codeController.OnDisable += DisableMenu;
            _codeController.Refresh();
        }

        private void ChangeLevel(int levelId)
        {
            EnableCodeMenu(false);
            SceneManager.LoadScene(levelId);
        }

        private void SetInvincibility(bool active)
        {
            //Debug.Log("Invincibility " + (active ? "Active" : "Disabled"));
            _invincible = active;
        }

        private void SetInfiniteJumps(bool active)
        {
            //Debug.Log("Infinite Jumps " + (active ? "Active" : "Disabled"));
            _playerController.SetInfiniteJumps(active);
        }

        private void SetBoltUnlocked(int state)
        {
            //Debug.Log("Warp " + (active ? "Unlocked" : "Locked"));
            _boltAbilityState = state;
            UpdateUnlocks();
        }

        private void SetWarpUnlocked(int state)
        {
            //Debug.Log("Warp " + (active ? "Unlocked" : "Locked"));
            _warpAbilityState = state;
            UpdateUnlocks();
        }

        private void SetResidueUnlocked(int state)
        {
            //Debug.Log("Residue " + (active ? "Unlocked" : "Locked"));
            _residueAbilityState = state;
            UpdateUnlocks();
        }

        private void SetNoBoltCooldown(bool active)
        {
            //Debug.Log("Bolt Cooldown " + (active ? "Disabled" : "Re-enabled"));
            _castingController.BoltIgnoreCooldown = active;
        }

        private void SetNoWarpCooldown(bool active)
        {
            //Debug.Log("Warp Cooldown " + (active ? "Disabled" : "Re-enabled"));
            _castingController.WarpIgnoreCooldown = active;
        }

        private void SetNoResidueCooldown(bool active)
        {
            //Debug.Log("Residue Cooldown " + (active ? "Disabled" : "Re-enabled"));
            _castingController.ResidueIgnoreCooldown = active;
        }

        private void SetInfiniteBoltDistance(bool active)
        {
            //Debug.Log("Bolt Infinite Distance " + (active ? "Enabled" : "Disabled"));
            _castingController.SetBoltInfiniteDistance(active);
        }

        private void SetBoltMoveSpeed(float value)
        {
            //Debug.Log("Bolt Move Speed Set To " + Mathf.FloorToInt(value * 100) + "%");
            _castingController.SetBoltMoveSpeed(value);
        }

        private void SetTedMortal(bool active)
        {
            //Debug.Log("Ted is " + (active ? "Now Mortal" : "Still Immortal"));
            _castingController.SetTedMortal(active);
        }

        private void DisableMenu()
        {
            EnableCodeMenu(false);
        }

        #endregion

        #endregion
    }
}