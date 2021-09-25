using AudioSystem;
using UnityEngine;
using UnityEngine.VFX;

namespace Mechanics.WarpBolt
{
    /// Summary:
    /// The sound and visual feedback script for the Bolt Controller
    public class BoltFeedback : MonoBehaviour
    {
        // @Brett should probably take over audio and hud Feedback

        [Header("Audio")]
        [SerializeField] private SFXOneShot _playerWarpSound = null;
        [SerializeField] private SFXOneShot _warpInteractSound = null;

        [Header("VFX on Impact")]
        [SerializeField] private VfxController _successfulImpactVfx;
        [SerializeField] private VfxController _failedImpactVfx;

        private void Awake()
        {
            // Ensure that particles are in scene (allows for prefab reference)
            if (_successfulImpactVfx != null && !_successfulImpactVfx.gameObject.activeInHierarchy) {
                _successfulImpactVfx = Instantiate(_successfulImpactVfx, transform);
            }
            if (_failedImpactVfx != null && !_failedImpactVfx.gameObject.activeInHierarchy) {
                _failedImpactVfx = Instantiate(_failedImpactVfx, transform);
            }
        }

        public void OnBoltDissipate(Vector3 position, Vector3 forward)
        {
            // Play dissipation particles
        }

        public void OnBoltImpact(Vector3 position, Vector3 normal, bool interactable = true)
        {
            VfxController effectToPlay = interactable ? _successfulImpactVfx : _failedImpactVfx;
            if (effectToPlay != null) {
                // Play particles at collision normal
                effectToPlay.transform.position = position;
                effectToPlay.transform.forward = normal;

                effectToPlay.Play();
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