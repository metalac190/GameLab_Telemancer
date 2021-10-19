using AudioSystem;
using UnityEngine;

namespace Mechanics.Bolt
{
    /// Summary:
    /// The sound and visual feedback script for the Bolt Controller
    public class BoltFeedback : MonoBehaviour
    {
        [SerializeField] private BoltVfxSpawner _boltVfxSpawner = null;

        // @Brett should probably take over audio and hud Feedback

        [Header("Audio")]
        [SerializeField] private SFXOneShot _warpInteractSound = null;
        [SerializeField] private SFXOneShot _objectImpactSound = null;

        [Header("VFX on Impact")]
        [SerializeField] private VfxController _boltImpactVfx = null;

        public void SetBoltCastDelta(float delta)
        {
            if (_boltVfxSpawner == null) return;
            _boltVfxSpawner.SetBoltCastDelta(delta);
        }

        public float OnBoltDissipate(Vector3 position, Vector3 forward)
        {
            if (_boltVfxSpawner != null) {
                return _boltVfxSpawner.Dissipate();
            }
            return 0;
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

            if (_objectImpactSound != null) {
                // Play Object Impact Sound
                _objectImpactSound.PlayOneShot(transform.position);
            }
        }

        public void OnWarpInteract()
        {
            if (_warpInteractSound != null) {
                // Play Warp Interact Sound
                // Used in place of manual residue for big rocks in lvl 1 and 2
                _warpInteractSound.PlayOneShot(transform.position);
            }
        }
    }
}