using UnityEngine;

namespace Mechanics.Player
{
    /// Summary:
    /// The Player Feedback Script, contains all sound or visual effects that the player directly causes
    /// Used by anything connected to the player
    [RequireComponent(typeof(PlayerSfx), typeof(PlayerVfx), typeof(PlayerToHud))]
    public class PlayerFeedback : MonoBehaviour
    {
        [SerializeField] private PlayerVfx _playerVfx;
        [SerializeField] private PlayerToHud _playerToHud;
        [SerializeField] private PlayerSfx _playerSfx;

        public void OnUpdateUnlockedAbilities(bool warpAbility, bool residueAbility)
        {
            _playerToHud.OnUpdateUnlockedAbilities(warpAbility, residueAbility);
        }

        public void OnPrepareToCast(bool wasSuccessful = true)
        {
            _playerToHud.OnPrepareToCast(wasSuccessful);
        }

        public void OnCastBolt()
        {
            _playerVfx.OnCastBolt();
            _playerSfx.CastBolt();
        }

        public void OnWarpReady(bool ready = true)
        {
            _playerToHud.OnWarpReady(ready);
        }

        public void OnActivateWarp(bool wasSuccessful = true)
        {
            _playerToHud.OnActivateWarp(wasSuccessful);
            if (wasSuccessful) {
                _playerSfx.ActivateWarp();
            }
        }

        public void OnResidueReady(bool ready = true)
        {
            _playerToHud.OnResidueReady(ready);
            if (ready) {
                _playerSfx.ResidueReady();
            }
        }

        public void OnActivateResidue(bool wasSuccessful = true)
        {
            _playerToHud.OnActivateResidue(wasSuccessful);
            if (wasSuccessful) {
                _playerSfx.ActivateResidue();
            }
        }

        public void OnCrosshairColorChange(InteractableEnums type)
        {
            _playerToHud.OnHudColorChange(type);
        }
    }
}