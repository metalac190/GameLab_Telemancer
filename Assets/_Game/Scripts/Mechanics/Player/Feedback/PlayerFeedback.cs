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
        [SerializeField] private PlayerAnimator _playerAnimator;

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

        private void OnEnable()
        {
            AnimatorNullCheck();
        }

        #endregion

        private AbilityStateEnum _boltState;
        private AbilityStateEnum _warpState;
        private AbilityStateEnum _residueState;

        private bool _boltCooldown;
        private bool _warpCooldown;
        private bool _residueCooldown;

        private bool _isJumping;
        private bool _isPlayerGrounded;

        #region Crosshair

        /* Player Crosshair
         * - Looking at nothing (empty air)
         * - Looking at object (not interactable)
         * - Looking at interactable (can be shot at with bolt)
         * - Looking at ted (can talk to)
         */

        public void OnCrosshairColorChange(InteractableEnums type)
        {
            _playerToHud.OnHudColorChange(type);
        }

        #endregion

        #region Player Movement

        // Player jumped
        public void OnPlayerJump()
        {
            _isJumping = true;
            _playerSfx.OnPlayerJump();
            if (!_missingAnimator) {
                _playerAnimator.OnJump();
            }
        }

        // Player walked off a platform. No longer grounded
        public void OnPlayerFall()
        {
            if (_isJumping) return;
            if (!_missingAnimator) {
                _playerAnimator.OnFall();
            }
        }

        // Player was falling and hit ground
        public void OnPlayerLand()
        {
            _isJumping = false;
            _playerSfx.OnPlayerLand();
            if (!_missingAnimator) {
                _playerAnimator.OnLand();
            }
        }

        public void SetPlayerVelocity(Vector3 velocity, bool isGrounded, bool walking)
        {
            _playerSfx.SetPlayerMovementSpeed(velocity, isGrounded, walking);

            if (_isPlayerGrounded != isGrounded) {
                _isPlayerGrounded = isGrounded;
                if (isGrounded) {
                    OnPlayerLand();
                } else {
                    OnPlayerFall();
                }
            }
        }

        #endregion

        #region Player Animation Events

        public void OnGamePaused()
        {
            _playerAnimator.ResetToIdle();
        }

        public void OnAnimationPoint()
        {
        }

        public void OnAnimationSnap()
        {
            _playerSfx.OnAnimationSnap();
        }

        #endregion

        #region Abilities

        public void SetWatcherLock(bool locked)
        {
            _playerToHud.InWatcherRange(locked);
            _playerSfx.InWatcherRange(locked);
        }

        // Updates what abilities are currently unlocked for the player. Used for visuals / hud
        public void OnUpdateUnlockedAbilities(bool boltAbility, bool warpAbility, bool residueAbility)
        {
            _playerToHud.OnUpdateUnlockedAbilities(boltAbility, warpAbility, residueAbility);

            if (!boltAbility) {
                _boltState = AbilityStateEnum.Disabled;
            } else if (_boltState == AbilityStateEnum.Disabled) {
                SetBoltState(AbilityStateEnum.Ready, true);
            }
            if (!warpAbility) {
                _warpState = AbilityStateEnum.Disabled;
            } else if (_warpState == AbilityStateEnum.Disabled) {
                SetWarpState(AbilityStateEnum.Idle, true);
            }
            if (!residueAbility) {
                _residueState = AbilityStateEnum.Disabled;
            } else if (_residueState == AbilityStateEnum.Disabled) {
                SetResidueState(AbilityStateEnum.Idle, true);
            }
        }

        #endregion

        // Ability States:
        // - Disabled (Not unlocked, cannot be used)
        // - Idle (Ability unlocked but cant be used yet)
        // - Ready (Ability active and ready to be used)

        // Ability Actions:
        // - InputDetected (Ability input detected)
        // - AttemptedUnsuccessful (After input, ability was not successful)
        // - AttemptedSuccessful (After input, ability was successful)
        // - Acted (Acted On / Ability used)

        #region Bolt

        public void SetBoltState(AbilityStateEnum state, bool setDisabled = false)
        {
            if (_boltState == AbilityStateEnum.Disabled && !setDisabled) return;

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
            if (_boltCooldown) {
                if (action == AbilityActionEnum.AttemptedUnsuccessful) {
                    _playerToHud.OnBoltAction(action);
                }
                return;
            }
            if (_boltState == AbilityStateEnum.Disabled) return;

            _playerToHud.OnBoltAction(action);

            // TODO: Attempt can still fail, but animation will play anyways
            if (action == AbilityActionEnum.InputDetected && !_missingAnimator) {
                _playerAnimator.OnCastBolt();
            }

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
                float delta = t / duration;
                _playerToHud.SetBoltCooldown(delta);
                yield return null;
            }
            _playerToHud.SetBoltCooldown(1);
            _boltCooldown = false;
            SetBoltState(AbilityStateEnum.Ready);
        }

        public void OnBoltDissipate(bool residueReady)
        {
            if (_missingAnimator) return;
            _playerAnimator.OnBoltDissipate(residueReady);
        }

        #endregion

        #region Warp

        public void SetWarpState(AbilityStateEnum state, bool setDisabled = false)
        {
            if (_warpState == AbilityStateEnum.Disabled && !setDisabled) return;

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

            // TODO: Attempt can still fail, but animation will play anyways
            if (action == AbilityActionEnum.InputDetected && !_missingAnimator) {
                _playerAnimator.OnInstantWarp();
            }

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
                float delta = t / duration;
                _playerToHud.SetWarpCooldown(delta);
                yield return null;
            }
            _playerToHud.SetWarpCooldown(1);
            _warpCooldown = false;
            _playerToHud.UpdateWarpState(_warpState);
        }

        #endregion

        #region Residue

        public void SetResidueState(AbilityStateEnum state, bool setDisabled = false)
        {
            if (_residueState == AbilityStateEnum.Disabled && !setDisabled) return;

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

            // TODO: Attempt can still fail, but animation will play anyways
            if (action == AbilityActionEnum.InputDetected && !_missingAnimator) {
                _playerAnimator.OnUseResidue();
            }

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
                float delta = t / duration;
                _playerToHud.SetResidueCooldown(delta);
                yield return null;
            }
            _playerToHud.SetResidueCooldown(0);
            _residueCooldown = false;
            _playerToHud.UpdateResidueState(_residueState);
        }

        #endregion

        #region Null Checks

        private bool _missingAnimator;

        private void AnimatorNullCheck()
        {
            if (_playerAnimator != null) return;

            Transform parent = transform.parent;
            _playerAnimator = parent != null ? parent.GetComponentInChildren<PlayerAnimator>() : GetComponentInChildren<PlayerAnimator>();
            if (_playerAnimator == null) {
                Debug.LogWarning("PlayerFeedback missing Player Animator");
                _missingAnimator = true;
            }
        }

        #endregion
    }
}