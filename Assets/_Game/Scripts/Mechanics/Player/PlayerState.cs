using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Mechanics.Player
{
    /// Summary:
    /// The State of the Player
    /// This should link to PlayerPrefs State (Henry)
    public class PlayerState : MonoBehaviour
    {
        [SerializeField] private bool _unlockedBolt = false;
        [SerializeField] private bool _unlockedWarp = false;
        [SerializeField] private bool _unlockedResidue = false;
        // Temporary Checkpoint holder -- TODO: Make actual check points and a respawn script
        [SerializeField] private Vector3 _defaultCheckpoint = Vector3.zero;
        [SerializeField] private UnityEvent _onPlayerDeath = new UnityEvent();
        [SerializeField] private float _respawnTime = 3;
        [SerializeField] private UnityEvent _onPlayerRespawn = new UnityEvent();

        public event Action<bool, bool, bool> OnChangeUnlocks = delegate { };

        private PlayerController _playerController;
        private PlayerCasting _castingController;
        private bool _isAlive = true;
        private bool _isPaused = true;

        private bool FlagCantAct() => _isAlive && !_isPaused;

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
            UIEvents.current.OnPauseGame += GamePaused;
            UpdateUnlocks();
        }

        public void SetBoltUnlock(bool unlocked)
        {
            _unlockedBolt = unlocked;
            UpdateUnlocks();
        }

        public void SetWarpUnlock(bool unlocked)
        {
            _unlockedWarp = unlocked;
            UpdateUnlocks();
        }

        public void SetResidueUnlock(bool unlocked)
        {
            _unlockedResidue = unlocked;
            UpdateUnlocks();
        }

        public void GamePaused(bool paused)
        {
            // TODO: (It works) But why is it !paused?
            _isPaused = !paused;
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
        }

        private void UpdateUnlocks()
        {
            OnChangeUnlocks.Invoke(_unlockedBolt, _unlockedWarp, _unlockedResidue);
        }

        private void NullCheck()
        {
            _playerController = GetComponent<PlayerController>();
            if (_playerController == null) {
                _playerController = FindObjectOfType<PlayerController>();
                if (_playerController == null) {
                    _playerController = gameObject.AddComponent<PlayerController>();
                }
            }
            _castingController = GetComponentInChildren<PlayerCasting>();
            if (_castingController == null) {
                _castingController = GetComponent<PlayerCasting>();
                if (_castingController == null) {
                    Debug.LogError("No Player Casting component found on player");
                }
            }
        }
    }
}