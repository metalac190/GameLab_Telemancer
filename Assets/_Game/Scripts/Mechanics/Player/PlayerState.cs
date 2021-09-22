using System;
using UnityEngine;

namespace Mechanics.Player
{
    // The State of the Player
    // This should link to PlayerPrefs State (Henry)
    public class PlayerState : MonoBehaviour
    {
        [SerializeField] private bool _unlockedWarp = false;
        [SerializeField] private bool _unlockedResidue = false;
        // Temporary Checkpoint holder -- TODO: Make actual check points and a respawn script
        [SerializeField] private Vector3 _lastCheckpoint;

        public event Action<bool, bool> OnChangeUnlocks = delegate { };

        private void OnValidate()
        {
            UpdateUnlocks();
        }

        private void Start()
        {
            UpdateUnlocks();
        }

        private void UpdateUnlocks()
        {
            OnChangeUnlocks.Invoke(_unlockedWarp, _unlockedResidue);
        }
    }
}