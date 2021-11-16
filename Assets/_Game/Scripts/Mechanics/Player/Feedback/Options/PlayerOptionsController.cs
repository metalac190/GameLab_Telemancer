using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Mechanics.Player.Feedback.Options
{
    public class PlayerOptionsController : MonoBehaviour
    {
        [SerializeField] private PlayerLevelSelector _levelSelector = null;
        [SerializeField] private PlayerOptionSelector _invincibility = null;
        [SerializeField] private PlayerOptionSelector _infiniteJumps = null;
        [SerializeField] private PlayerOptionSelector _noBoltCooldown = null;
        [SerializeField] private PlayerOptionSelector _noWarpCooldown = null;
        [SerializeField] private PlayerOptionSelector _noResidueCooldown = null;
        [SerializeField] private PlayerOptionSelector _infiniteBoltDistance = null;
        [SerializeField] private PlayerOptionsSlider _boltMoveSpeed = null;

        public PlayerLevelSelector LevelSelector => _levelSelector;
        public PlayerOptionSelector Invincibility => _invincibility;
        public PlayerOptionSelector InfiniteJumps => _infiniteJumps;
        public PlayerOptionSelector NoBoltCooldown => _noBoltCooldown;
        public PlayerOptionSelector NoWarpCooldown => _noWarpCooldown;
        public PlayerOptionSelector NoResidueCooldown => _noResidueCooldown;
        public PlayerOptionSelector InfiniteBoltDistance => _infiniteBoltDistance;
        public PlayerOptionsSlider BoltMoveSpeed => _boltMoveSpeed;

        private void OnEnable()
        {
            UIEvents.current.DisableGamePausing();
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void OnDisable()
        {
            UIEvents.current.EnableGamePausing();
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame) {
                gameObject.SetActive(false);
            }
        }

        public void SetData(PlayerOptionsData data)
        {
            _invincibility.SetCurrentItem(data.Invincibility);
            _infiniteJumps.SetCurrentItem(data.InfiniteJumps);
            _noBoltCooldown.SetCurrentItem(data.NoBoltCooldown);
            _noWarpCooldown.SetCurrentItem(data.NoWarpCooldown);
            _noResidueCooldown.SetCurrentItem(data.NoResidueCooldown);
            _infiniteBoltDistance.SetCurrentItem(data.InfiniteBoltDistance);
            _boltMoveSpeed.SetCurrentValue(data.BoltMoveSpeed);
        }
    }
}