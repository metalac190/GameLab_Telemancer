using System.Collections;
using AudioSystem;
using Mechanics.Bolt.Effects;
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
        [SerializeField] private LowQualityCollisionParticles _lowQualityImpact = null;

        private bool _overrideBoltLife;
        private bool _useVfx;

        private void OnEnable()
        {
            _useVfx = PlayerPrefs.GetFloat("SimplifiedVisuals") == 0;
            if (_useVfx && _boltImpactVfx == null) {
                _useVfx = false;
            }
        }

        public void SetBoltCastDelta(float delta)
        {
            if (_boltVfxSpawner == null) return;
            _boltVfxSpawner.SetBoltCastDelta(delta);
        }

        public void SetBoltLifetime(float timeAlive, float lifeSpan)
        {
            if (_overrideBoltLife || _boltVfxSpawner == null) return;
            _boltVfxSpawner.SetBoltLifetime(timeAlive, lifeSpan);
        }

        public void OverrideBoltLifetime(float timeAlive, float lifeSpan, float extraLifeSpan, float timeLeftAlive)
        {
            _overrideBoltLife = true;
            StartCoroutine(BoltLifetime(timeAlive, lifeSpan, extraLifeSpan, timeLeftAlive));
        }

        private IEnumerator BoltLifetime(float startTime, float lifeSpan, float extraLifeSpan, float timeLeftAlive)
        {
            for (float t = 0; t < timeLeftAlive; t += Time.deltaTime) {
                float delta = t / timeLeftAlive;
                float timeAlive = Mathf.Lerp(startTime, lifeSpan + extraLifeSpan, delta);
                _boltVfxSpawner.SetBoltLifetime(timeAlive, lifeSpan);
                yield return null;
            }
        }

        public void OnBoltDissipate(Vector3 position, Vector3 forward, float dissipateTime, float dimLightTime)
        {
            if (_boltVfxSpawner == null) return;
            _boltVfxSpawner.Dissipate(dissipateTime);
            _boltVfxSpawner.DimLight(dimLightTime);
        }

        public void OnBoltImpact(Vector3 position, Vector3 normal, bool interactable = true)
        {
            // TODO: Remove nasty instantiation -- reuse objects in some way

            if (_useVfx) {
                var controller = Instantiate(_boltImpactVfx);
                // Play particles at collision normal
                controller.transform.position = position;
                controller.transform.forward = normal;

                controller.Play(interactable);

                controller.AutoKill(2);
            } else if (_lowQualityImpact != null) {
                var controller = Instantiate(_lowQualityImpact);
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

        public void OnReset()
        {
            _overrideBoltLife = false;
            if (_boltVfxSpawner == null) return;
            _boltVfxSpawner.OnReset();
        }
    }
}