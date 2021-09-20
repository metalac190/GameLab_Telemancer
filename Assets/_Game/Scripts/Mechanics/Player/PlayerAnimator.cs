using UnityEngine;

namespace Mechanics.Player
{
    // The main Player Animator script that controls all animation for the player
    // Mostly used by the Player Casting Script and the 'Player Interactions' Script
    public class PlayerAnimator : MonoBehaviour
    {
        private float _fallTime;

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
        }

        public void OnResidueActive()
        {
        }

        public void OnInstantWarp()
        {
        }

        public void OnInteractableWarp()
        {
        }

        private void ResetToIdle()
        {
            // Resets all triggers and returns animation to idle
            // For instance, after warping or after landing a fall
        }
    }
}