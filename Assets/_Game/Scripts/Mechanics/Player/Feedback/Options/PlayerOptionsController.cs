using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Mechanics.Player.Feedback.Options
{
    public class PlayerOptionsController : MonoBehaviour
    {
        public static PlayerOptionsController Instance;

        [SerializeField] private PlayerLevelSelector _levelSelector = null;
        [SerializeField] private PlayerOptionSelector _invincibility = null;
        [SerializeField] private PlayerOptionSelector _infiniteJumps = null;
        [SerializeField] private PlayerOptionSelectorAdvanced _boltUnlocked = null;
        [SerializeField] private PlayerOptionSelectorAdvanced _warpUnlocked = null;
        [SerializeField] private PlayerOptionSelectorAdvanced _residueUnlocked = null;
        [SerializeField] private PlayerOptionSelector _noBoltCooldown = null;
        [SerializeField] private PlayerOptionSelector _noWarpCooldown = null;
        [SerializeField] private PlayerOptionSelector _noResidueCooldown = null;
        [SerializeField] private PlayerOptionSelector _infiniteBoltDistance = null;
        [SerializeField] private PlayerOptionsSlider _boltMoveSpeed = null;
        [SerializeField] private PlayerOptionSelector _mortalTed = null;
        [SerializeField] private PlayerOptionSelector _improvedWaterfalls = null;

        public PlayerLevelSelector LevelSelector => _levelSelector;
        public PlayerOptionSelector Invincibility => _invincibility;
        public PlayerOptionSelector InfiniteJumps => _infiniteJumps;
        public PlayerOptionSelectorAdvanced BoltUnlocked => _boltUnlocked;
        public PlayerOptionSelectorAdvanced WarpUnlocked => _warpUnlocked;
        public PlayerOptionSelectorAdvanced ResidueUnlocked => _residueUnlocked;
        public PlayerOptionSelector NoBoltCooldown => _noBoltCooldown;
        public PlayerOptionSelector NoWarpCooldown => _noWarpCooldown;
        public PlayerOptionSelector NoResidueCooldown => _noResidueCooldown;
        public PlayerOptionSelector InfiniteBoltDistance => _infiniteBoltDistance;
        public PlayerOptionsSlider BoltMoveSpeed => _boltMoveSpeed;
        public PlayerOptionSelector MortalTed => _mortalTed;
        public PlayerOptionSelector ImprovedWaterfalls => _improvedWaterfalls;

        public event Action OnDisable = delegate { };

        private void Start()
        {
            if (Instance != null) {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void Refresh()
        {
            _invincibility.Refresh();
            _infiniteJumps.Refresh();
            _boltUnlocked.Refresh();
            _warpUnlocked.Refresh();
            _residueUnlocked.Refresh();
            _noBoltCooldown.Refresh();
            _noWarpCooldown.Refresh();
            _noResidueCooldown.Refresh();
            _infiniteBoltDistance.Refresh();
            _boltMoveSpeed.Refresh();
        }

        public void Disable()
        {
            OnDisable?.Invoke();
        }
    }
}