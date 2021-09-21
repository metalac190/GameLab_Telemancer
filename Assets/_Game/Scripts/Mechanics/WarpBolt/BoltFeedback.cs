using UnityEngine;
using UnityEngine.VFX;

namespace Mechanics.WarpBolt
{
    // The sound and visual feedback script for the Bolt Controller
    public class BoltFeedback : MonoBehaviour
    {
        [Header("Audio")]
        [SerializeField] private AudioClip _playerWarpSound = null;
        [SerializeField] private AudioClip _warpInteractSound = null;
        [Header("Visual")]
        [SerializeField] private VisualEffect _impactParticles = null;

        public void OnBoltDissipate()
        {
            if (_impactParticles != null) {
                _impactParticles.Play();
            }
        }

        public void OnWarpInteract()
        {
            if (_warpInteractSound != null) {
                // Play Warp Interact Sound
            }
        }

        public void OnPlayerWarp()
        {
            if (_playerWarpSound != null) {
                // Play player warp sound
                // Maybe be on Player Feedback Script?
            }
        }
    }
}