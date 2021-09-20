using System;
using UnityEngine;

namespace Mechanics.Player
{
    public class PlayerState : MonoBehaviour
    {
        [SerializeField] private Vector3 _lastCheckpoint;
        [SerializeField] private bool _unlockedWarp = false;
        [SerializeField] private bool _unlockedResidue = false;

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