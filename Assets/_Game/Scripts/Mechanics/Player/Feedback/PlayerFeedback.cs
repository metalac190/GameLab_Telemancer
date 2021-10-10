using System.Collections;
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

        [SerializeField] private AbilityStateEnum _boltState;
        [SerializeField] private AbilityStateEnum _warpState;
        [SerializeField] private AbilityStateEnum _residueState;

        private bool _boltCooldown;
        private bool _warpCooldown;
        private bool _residueCooldown;

        /* Player Crosshair
         * - Looking at nothing (empty air)
         * - Looking at object (not interactable)
         * - Looking at interactable (can be shot at with bolt)
         * - Looking at ted (can talk to)
         */

        #region Crosshair

        public void OnCrosshairColorChange(InteractableEnums type)
        {
            _playerToHud.OnHudColorChange(type);
        }

        // Updates what abilities are currently unlocked for the player. Used for visuals / hud
        public void OnUpdateUnlockedAbilities(bool boltAbility, bool warpAbility, bool residueAbility)
        {
            _playerToHud.OnUpdateUnlockedAbilities(warpAbility, residueAbility);

            if (!boltAbility) {
                _boltState = AbilityStateEnum.Disabled;
            } else if (_boltState == AbilityStateEnum.Disabled) {
                _boltState = AbilityStateEnum.Ready;
            }
            if (!warpAbility) {
                _warpState = AbilityStateEnum.Disabled;
            } else if (_warpState == AbilityStateEnum.Disabled) {
                _warpState = AbilityStateEnum.Idle;
            }
            if (!residueAbility) {
                _residueState = AbilityStateEnum.Disabled;
            } else if (_residueState == AbilityStateEnum.Disabled) {
                _residueState = AbilityStateEnum.Idle;
            }
        }

        #endregion

        // Ability States:
        // - Disabled (Not unlocked, cannot be used)
        // - Idle (Ability unlocked but cant be used yet)
        // - Ready (Ability active and ready to be used)
        // - Locked (Ability used, waiting for cooldown)

        // Ability Actions:
        // - InputDetected (Ability input detected)
        // - AttemptedUnsuccessful (After input, ability was not successful)
        // - AttemptedSuccessful (After input, ability was successful)
        // - Acted (Acted On / Ability used)

        #region Bolt

        public void SetBoltState(AbilityStateEnum state)
        {
            if (_boltState == AbilityStateEnum.Disabled) return;

            _boltState = state;
            if (_boltCooldown) return;

            _playerToHud.UpdateBoltState(_boltState);

            if (_boltState == AbilityStateEnum.Ready) {
                _playerSfx.OnBoltReady();
                _playerVfx.OnBoltReady();
            }
        }

        public void OnBoltAction(AbilityActionEnum action)
        {
            if (_boltCooldown || _boltState == AbilityStateEnum.Disabled) return;

            _playerToHud.OnBoltAction(action);

            if (action == AbilityActionEnum.Acted) {
                _playerSfx.OnBoltUsed();
                _playerVfx.OnBoltUsed();
            }
        }

        public void SetBoltCooldown(float cooldownDuration)
        {
            StartCoroutine(BoltCooldown(cooldownDuration));
        }

        private IEnumerator BoltCooldown(float duration)
        {
            SetBoltState(AbilityStateEnum.Idle);
            _boltCooldown = true;
            for (float t = 0; t < duration; t += Time.deltaTime) {
                float delta = 1 - t / duration;
                _playerToHud.SetBoltCooldown(delta);
                yield return null;
            }
            _playerToHud.SetBoltCooldown(0);
            _boltCooldown = false;
            SetBoltState(AbilityStateEnum.Ready);
        }

        #endregion

        #region Warp

        public void SetWarpState(AbilityStateEnum state)
        {
            if (_warpState == AbilityStateEnum.Disabled) return;

            _warpState = state;
            if (_warpCooldown) return;

            _playerToHud.UpdateWarpState(_warpState);

            if (_warpState == AbilityStateEnum.Ready) {
                _playerSfx.OnWarpReady();
                _playerVfx.OnWarpReady();
            }
        }

        public void OnWarpAction(AbilityActionEnum action)
        {
            if (_warpCooldown || _warpState == AbilityStateEnum.Disabled) return;

            _playerToHud.OnWarpAction(action);

            if (action == AbilityActionEnum.Acted) {
                _playerSfx.OnWarpUsed();
                _playerVfx.OnWarpUsed();
            }
        }

        public void SetWarpCooldown(float cooldownDuration)
        {
            StartCoroutine(WarpCooldown(cooldownDuration));
        }

        private IEnumerator WarpCooldown(float duration)
        {
            _warpCooldown = true;
            for (float t = 0; t < duration; t += Time.deltaTime) {
                float delta = 1 - t / duration;
                _playerToHud.SetWarpCooldown(delta);
                yield return null;
            }
            _playerToHud.SetWarpCooldown(0);
            _warpCooldown = false;
            _playerToHud.UpdateWarpState(_warpState);
        }

        #endregion

        #region Residue

        public void SetResidueState(AbilityStateEnum state)
        {
            if (_residueState == AbilityStateEnum.Disabled) return;

            _residueState = state;
            if (_residueCooldown) return;

            _playerToHud.UpdateResidueState(_residueState);

            if (_residueState == AbilityStateEnum.Ready) {
                _playerSfx.OnResidueReady();
                _playerVfx.OnResidueReady();
            }
        }

        public void OnResidueAction(AbilityActionEnum action)
        {
            if (_residueCooldown || _residueState == AbilityStateEnum.Disabled) return;

            _playerToHud.OnResidueAction(action);

            if (action == AbilityActionEnum.Acted) {
                _playerSfx.OnResidueUsed();
                _playerVfx.OnResidueUsed();
            }
        }

        public void SetResidueCooldown(float cooldownDuration)
        {
            StartCoroutine(ResidueCooldown(cooldownDuration));
        }

        private IEnumerator ResidueCooldown(float duration)
        {
            _residueCooldown = true;
            for (float t = 0; t < duration; t += Time.deltaTime) {
                float delta = 1 - t / duration;
                _playerToHud.SetResidueCooldown(delta);
                yield return null;
            }
            _playerToHud.SetResidueCooldown(0);
            _residueCooldown = false;
            _playerToHud.UpdateResidueState(_residueState);
        }

        #endregion
    }
}