using UnityEngine;

namespace Mechanics.Player
{
    /// Summary:
    /// The Player Feedback Script, is the middle man for the player and the various types of feedback
    /// 
    /// Requires each of the feedback component types. This can be switched to make them exposed, msg Brandon if we need to do that
    [RequireComponent(typeof(PlayerSfx), typeof(PlayerVfx), typeof(PlayerToHud))]
    public class PlayerFeedback : MonoBehaviour
    {
        [SerializeField] private PlayerAnimator _playerAnimator = null;

        #region Feedback Script References

        private PlayerToHud _playerToHud;
        private PlayerSfx _playerSfx;
        private PlayerVfx _playerVfx;

        private void Awake()
        {
            _playerToHud = GetComponent<PlayerToHud>();
            if (_playerToHud == null) {
                _playerToHud = gameObject.AddComponent<PlayerToHud>();
            }
            _playerSfx = GetComponent<PlayerSfx>();
            if (_playerSfx == null) {
                _playerSfx = gameObject.AddComponent<PlayerSfx>();
            }
            _playerVfx = GetComponent<PlayerVfx>();
            if (_playerVfx == null) {
                _playerVfx = gameObject.AddComponent<PlayerVfx>();
            }
        }

        #endregion

        // Updates what abilities are currently unlocked for the player. Used for visuals / hud
        public void OnUpdateUnlockedAbilities(bool warpAbility, bool residueAbility)
        {
            _playerToHud.OnUpdateUnlockedAbilities(warpAbility, residueAbility);
        }

        // The player made a "Cast" input. Can be successful (OnCastBolt() is called) or failed. Used for hud feedback
        public void OnPrepareToCast(bool wasSuccessful = true)
        {
            _playerToHud.OnPrepareToCast(wasSuccessful);
            if (wasSuccessful && _playerAnimator != null) {
                _playerAnimator.OnCastBolt();
            }
        }

        // The player successfully casted a bolt
        public void OnCastBolt()
        {
            _playerVfx.OnCastBolt();
            _playerSfx.CastBolt();
        }

        // Either a warp is ready to be used (currently midair -- ready = true) or not (cannot use warp -- ready = false)
        public void OnWarpReady(bool ready = true)
        {
            _playerToHud.OnWarpReady(ready);
        }

        public void PrepareToWarp()
        {
            if (_playerAnimator != null) {
                _playerAnimator.OnInstantWarp();
            }
        }

        // The player made a "Warp" input. Can be successful or failed
        public void OnActivateWarp(bool wasSuccessful = true)
        {
            _playerToHud.OnActivateWarp(wasSuccessful);
            if (wasSuccessful) {
                _playerSfx.ActivateWarp();
            }
        }

        public void PrepareForResidue()
        {
            if (_playerAnimator != null) {
                _playerAnimator.OnUseResidue();
            }
        }

        // Either the residue is active in scene (ready = true) or residue is no longer active (ready = false)
        public void OnResidueReady(bool ready = true)
        {
            _playerToHud.OnResidueReady(ready);
            if (ready) {
                _playerSfx.ResidueReady();
            }
        }

        // The player made a "Activate Residue" input. Can be successful or failed
        public void OnActivateResidue(bool wasSuccessful = true)
        {
            _playerToHud.OnActivateResidue(wasSuccessful);
            if (wasSuccessful) {
                _playerSfx.ActivateResidue();
                if (_playerAnimator != null) {
                    _playerAnimator.OnUseResidue();
                }
            }
        }

        // Changes depending on what the player is looking at
        public void OnCrosshairColorChange(InteractableEnums type)
        {
            _playerToHud.OnHudColorChange(type);
        }
    }
}