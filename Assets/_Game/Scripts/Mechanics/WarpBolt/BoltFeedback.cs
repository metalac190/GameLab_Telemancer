using UnityEngine;

namespace Mechanics.WarpBolt
{
    // The sound and visual feedback script for the Bolt Controller
    public class BoltFeedback : MonoBehaviour
    {
        [Header("Audio")]
        [SerializeField] private AudioClip _playerWarpSound = null;
        [SerializeField] private AudioClip _warpInteractSound = null;
        [Header("Visual")]
        [SerializeField] private ParticleSystem _dissipationParticles = null;

        public void OnBoltDissipate()
        {
            if (_dissipationParticles != null) {
                _dissipationParticles.Play();
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