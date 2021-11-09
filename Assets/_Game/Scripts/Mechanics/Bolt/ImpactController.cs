using System.Collections;
using Mechanics.Bolt.Effects;
using UnityEngine;

namespace Mechanics.Bolt
{
    public class ImpactController : MonoBehaviour
    {
        [SerializeField] private VfxController _impactVfxPrefab = null;
        [SerializeField] private LowQualityCollisionParticles _lowQualityImpactPrefab = null;

        private VfxController _impactVfx;
        private LowQualityCollisionParticles _lowQualityImpact;

        private bool _useVfx;
        private bool _missingVfx;
        private BoltManager _boltManager;

        private void OnEnable()
        {
            _useVfx = PlayerPrefs.GetFloat("SimplifiedVisuals") == 0;
            if (_useVfx) {
                if (_impactVfx == null && _impactVfxPrefab != null) {
                    _impactVfx = Instantiate(_impactVfxPrefab);
                }
                _missingVfx = _impactVfx == null;
            } else {
                if (_lowQualityImpact == null && _lowQualityImpactPrefab != null) {
                    _lowQualityImpact = Instantiate(_lowQualityImpactPrefab);
                }
                _missingVfx = _lowQualityImpact == null;
            }
        }

        public void SetManager(BoltManager manager)
        {
            _boltManager = manager;
        }

        public void PlayImpact(Vector3 position, Vector3 normal, bool hitInteractable)
        {
            if (_missingVfx) return;
            if (_useVfx) {
                _impactVfx.transform.position = position;
                _impactVfx.transform.forward = normal;

                _impactVfx.Play(hitInteractable);
            } else {
                _lowQualityImpact.transform.position = position;
                _lowQualityImpact.transform.forward = normal;

                _lowQualityImpact.Play(hitInteractable);
            }
            StartCoroutine(AutoKill(2));
        }

        private IEnumerator AutoKill(float timer)
        {
            for (float t = 0; t < timer; t += Time.deltaTime) {
                yield return null;
            }
            if (!_missingVfx) {
                if (_useVfx) {
                    _impactVfx.Stop();
                } else {
                    _lowQualityImpact.Stop();
                }
            }
            _boltManager.AddController(this);
        }
    }
}