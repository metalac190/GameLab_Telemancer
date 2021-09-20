using UnityEngine;

// The sound and visual feedback script for the Bolt Controller
namespace Mechanics.WarpBolt
{
    public class BoltFeedback : MonoBehaviour
    {
        [Header("Audio")]
        [SerializeField] private AudioClip _playerWarpSound = null;
        [Header("Visual")]
        [SerializeField] private ParticleSystem _dissipationParticles = null;

        public void OnBoltDissipate()
        {
            if (_dissipationParticles != null) {
                _dissipationParticles.Play();
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