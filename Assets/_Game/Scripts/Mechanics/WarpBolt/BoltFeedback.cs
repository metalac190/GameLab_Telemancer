using AudioSystem;
using UnityEngine;
using UnityEngine.VFX;

namespace Mechanics.WarpBolt
{
    /// Summary:
    /// The sound and visual feedback script for the Bolt Controller
    public class BoltFeedback : MonoBehaviour
    {
        [Header("Audio")]
        [SerializeField] private SFXOneShot _playerWarpSound = null;
        [SerializeField] private SFXOneShot _warpInteractSound = null;
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
                _warpInteractSound.PlayOneShot(transform.position);
            }
        }

        public void OnPlayerWarp()
        {
            if (_playerWarpSound != null) {
                // Maybe should be on Player Feedback Script?
                // Play player warp sound
                _playerWarpSound.PlayOneShot(transform.position);
            }
        }
    }
}