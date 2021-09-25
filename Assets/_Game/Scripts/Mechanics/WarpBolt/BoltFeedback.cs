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
        [SerializeField] private VfxController _boltImpactVfx;

        public void OnBoltDissipate(Vector3 position, Vector3 forward)
        {
            // Play dissipation particles
        }

        public void OnBoltImpact(Vector3 position, Vector3 normal, bool interactable = true)
        {
            // TODO: Remove nasty instantiation -- reuse objects in some way

            if (_boltImpactVfx != null) {
                VfxController controller = Instantiate(_boltImpactVfx);
                // Play particles at collision normal
                controller.transform.position = position;
                controller.transform.forward = normal;

                controller.Play(interactable);

                controller.AutoKill(2);
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