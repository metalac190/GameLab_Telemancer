using System.Collections;
using UnityEngine;

namespace Mechanics.Player
{
    // The main Player Animator script that controls all animation for the player
    // Mostly used by the Player Casting Script and the 'Player Interactions' Script
    public class PlayerAnimator : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private string _castBoltTrigger = "pointTrigger";
        [SerializeField] private string _warpTrigger = "snapTrigger";
        [SerializeField] private string _useResidueTrigger = "interactTrigger";
        [SerializeField] private string _boltDissipateTrigger = "dissipateTrigger";

        private float _fallTime;

        private void Awake()
        {
            if (_animator == null) {
                _animator = GetComponentInChildren<Animator>();
            }
        }

        private void OnEnable()
        {
            ResetToIdle();
        }

        public void OnPetToad()
        {
        }

        public void OnStartle()
        {
        }

        public void OnJump()
        {
            _fallTime = Time.time;
        }

        public void OnFall()
        {
        }

        public void OnLand()
        {
            float time = Time.time - _fallTime;
            if (time < 1) {
                OnSimpleLand();
            } else if (time < 2) {
                OnAnimatedLand();
            } else {
                OnDeathLand();
            }
        }

        private void OnSimpleLand()
        {
        }

        private void OnAnimatedLand()
        {
        }

        private void OnDeathLand()
        {
        }

        public void OnKill()
        {
        }

        public void OnCastBolt()
        {
            _animator.SetTrigger(_castBoltTrigger);
            _animator.ResetTrigger(_warpTrigger);
            _animator.ResetTrigger(_useResidueTrigger);
            _animator.ResetTrigger(_boltDissipateTrigger);
        }

        public void OnUseResidue()
        {
            _animator.SetTrigger(_useResidueTrigger);
        }

        public void OnInstantWarp()
        {
            _animator.SetTrigger(_warpTrigger);
        }

        public void OnInteractableWarp()
        {
            _animator.SetTrigger(_useResidueTrigger);
        }

        public void OnNoAction()
        {
            _animator.SetTrigger(_boltDissipateTrigger);
        }

        public void ResetToIdle()
        {
            // Resets all triggers and returns animation to idle
            // For instance, after warping or after landing a fall
            _animator.ResetTrigger(_castBoltTrigger);
            _animator.ResetTrigger(_warpTrigger);
            _animator.ResetTrigger(_useResidueTrigger);
            _animator.ResetTrigger(_boltDissipateTrigger);
            _animator.Rebind();
            _animator.Update(0);
        }
    }
}