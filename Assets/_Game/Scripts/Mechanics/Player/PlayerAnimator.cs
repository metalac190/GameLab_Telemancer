using AudioSystem;
using UnityEngine;

namespace Mechanics.Player
{
    // The main Player Animator script that controls all animation for the player
    // Mostly used by the Player Casting Script and the 'Player Interactions' Script
    public class PlayerAnimator : MonoBehaviour
    {
        [Header("Animator")]
        [SerializeField] private Animator _animator;
        [SerializeField] private float _idleFlairMinDelay = 5f;
        [SerializeField] private float _idleFlairMaxDelay = 20f;
        [Header("Idle Parameters")]
        [SerializeField] private string _petToadTrigger = "pet_toad";
        [SerializeField] private string _startledTrigger = "startled";
        [SerializeField] private string _idleTwitchTrigger = "idle_twitch";
        [Header("Jumping / Landing Parameters")]
        [SerializeField] private string _jumpTrigger = "jump";
        [SerializeField] private string _fallTrigger = "fall";
        [SerializeField] private string _airTimeFloat = "air_time";
        [SerializeField] private string _landTrigger = "land";
        [Header("Casting Parameters")]
        [SerializeField] private string _castBoltTrigger = "point_cast";
        [SerializeField] private string _warpTrigger = "snap_teleport";
        [SerializeField] private string _interactableTrigger = "interact_hitInteractable";
        [SerializeField] private string _residueIdleTrigger = "residue_idle";
        [SerializeField] private string _relayInteract = "relay_interact";
        [SerializeField] private string _boltDissipateTrigger = "dissipate";

        private bool _missingAnimator;
        private float _jumpTime;
        
        private float _idleTime;
        private float _idleFlairTime;
        private bool _wasIdleFlairBefore;

        private void Awake()
        {
            if (_animator == null) {
                _animator = GetComponentInChildren<Animator>();
                if (_animator == null) {
                    _missingAnimator = true;
                }
            }
            _idleTime = 0;
        }

        private void OnEnable()
        {
            ResetToIdle();
        }

        private void Update()
        {
            // TODO: Better way of doing idle flair timer
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) {
                if (_idleTime == 0) {
                    _idleFlairTime = Random.Range(_idleFlairMinDelay, _idleFlairMaxDelay);
                    if (_wasIdleFlairBefore) {
                        _idleFlairTime *= 2;
                    }
                }
                _idleTime += Time.deltaTime;
                if (_idleTime >= _idleFlairTime) {
                    OnIdleFlair();
                }
            } else {
                _idleTime = 0;
            }
        }

        public void TriggerAnim(string anim)
        {
            _animator.SetTrigger(anim);
        }

        #region Basic Actions

        public void OnPetToad()
        {
            if (_missingAnimator) return;
            TriggerAnim(_petToadTrigger);
        }

        public void OnStartle()
        {
            if (_missingAnimator) return;
            TriggerAnim(_startledTrigger);
        }

        public void OnIdleFlair()
        {
            if (_missingAnimator) return;
            _wasIdleFlairBefore = true;
            _idleTime = 0;
            TriggerAnim(_idleTwitchTrigger);
        }

        public void OnJump()
        {
            if (_missingAnimator) return;
            _jumpTime = Time.time;
            ResetActionTriggers();
            TriggerAnim(_jumpTrigger);
        }

        public void OnFall()
        {
            if (_missingAnimator) return;
            _jumpTime = Time.time;
            TriggerAnim(_fallTrigger);
        }

        public void OnLand()
        {
            if (_missingAnimator) return;
            ResetActionTriggers();
            _animator.SetFloat(_airTimeFloat, Time.time - _jumpTime);
            TriggerAnim(_landTrigger);
        }

        public void OnKill()
        {
        }

        #endregion

        #region Casting Actions

        public void OnCastBolt()
        {
            if (_missingAnimator) return;
            ResetCastingTriggers();
            TriggerAnim(_castBoltTrigger);
        }

        // Player warped (right click) with warp unlocked. Play anim and return to idle.
        public void OnInstantWarp()
        {
            if (_missingAnimator) return;
            ResetActionTriggers();
            ResetCastingTriggers();
            TriggerAnim(_warpTrigger);
        }

        // Player hit interactable with bolt. Play anim and return to idle.
        public void OnInteractableWarp()
        {
            if (_missingAnimator) return;
            ResetActionTriggers();
            ResetCastingTriggers();
            TriggerAnim(_interactableTrigger);
        }

        // Player used residue. Play hit interactable and return to idle.
        // TODO: Should this be a different animation?
        public void OnUseResidue()
        {
            OnInteractableWarp();
        }

        public void ReturnToHold()
        {
            if (_missingAnimator) return;
            ResetCastingTriggers();
            TriggerAnim(_relayInteract);
        }

        // Player was holding magic, but the bolt dissipated. Return to Idle
        public void OnBoltDissipate(bool residueReady)
        {
            if (_missingAnimator) return;
            if (residueReady) {
                TriggerAnim(_residueIdleTrigger);
            } else {
                ResetActionTriggers();
                ResetCastingTriggers();
                TriggerAnim(_boltDissipateTrigger);
            }
        }

        #endregion

        #region Helper Functions

        // Resets all triggers and returns animation to idle
        // For instance, after warping or after landing a fall
        public void ResetToIdle()
        {
            if (_missingAnimator) return;
            ResetActionTriggers();
            ResetCastingTriggers();
            _animator.Rebind();
            _animator.Update(0);
        }

        private void ResetActionTriggers()
        {
            _animator.ResetTrigger(_petToadTrigger);
            _animator.ResetTrigger(_startledTrigger);
            _animator.ResetTrigger(_idleTwitchTrigger);
            _animator.ResetTrigger(_jumpTrigger);
            _animator.ResetTrigger(_fallTrigger);
            _animator.ResetTrigger(_landTrigger);
        }

        private void ResetCastingTriggers()
        {
            _animator.ResetTrigger(_castBoltTrigger);
            _animator.ResetTrigger(_warpTrigger);
            _animator.ResetTrigger(_interactableTrigger);
            _animator.ResetTrigger(_boltDissipateTrigger);
            _animator.ResetTrigger(_relayInteract);
        }

        #endregion
    }
}