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
        [SerializeField] private bool _unlockedWarp = false;
        [SerializeField] private bool _unlockedResidue = false;
        // Temporary Checkpoint holder -- TODO: Make actual check points and a respawn script
        [SerializeField] private Vector3 _lastCheckpoint;
        [SerializeField] private UnityEvent _onPlayerDeath = new UnityEvent();
        [SerializeField] private float _respawnTime = 3;
        [SerializeField] private UnityEvent _onPlayerRespawn = new UnityEvent();

        public event Action<bool, bool> OnChangeUnlocks = delegate { };

        private PlayerController _playerController;

        private void OnValidate()
        {
            UpdateUnlocks();
        }

        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();
            if (_playerController == null) {
                _playerController = FindObjectOfType<PlayerController>();
                if (_playerController == null) {
                    _playerController = gameObject.AddComponent<PlayerController>();
                }
            }
        }

        private void Start()
        {
            UpdateUnlocks();
        }

        public void OnKill()
        {
            _onPlayerDeath.Invoke();

            _playerController.flag_cantAct = true;

            StartCoroutine(RespawnCoroutine());
        }

        // TODO: Actual respawn method, this is a temporary timer for testing only
        private IEnumerator RespawnCoroutine()
        {
            yield return new WaitForSecondsRealtime(_respawnTime);
            OnRespawn();
        }

        public void OnRespawn()
        {
            _onPlayerRespawn.Invoke();

            _playerController.TeleportToPosition(_lastCheckpoint);
            _playerController.flag_cantAct = false;
        }

        private void UpdateUnlocks()
        {
            OnChangeUnlocks.Invoke(_unlockedWarp, _unlockedResidue);
        }
    }
}