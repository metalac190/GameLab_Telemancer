using System.Collections;
using UnityEngine;

namespace Mechanics.Bolt.Effects
{
    /// Summary:
    /// A helper script to instantiate an object on awake.
    /// Used to instantiate visual effect prefabs primarily
    public class BoltVfxSpawner : MonoBehaviour
    {
        [SerializeField] private BoltVfxController _boltPrefab = null;
        [SerializeField] private BoltLowQualityEffect _boltLowQuality = null;
        [SerializeField] private Transform _transformOverride = null;
        [SerializeField] private Light _lightToDim = null;

        private BoltVfxController _instantiatedObject;
        private BoltLowQualityEffect _lowQualityObject;

        private float _lightIntensity = -1;
        private bool _useVfx;
        private bool _missingVfx;

        private void OnEnable()
        {
            _useVfx = PlayerPrefs.GetFloat("SimplifiedVisuals") == 0;
            if (_useVfx) {
                if (_instantiatedObject == null && _boltPrefab != null) {
                    InstantiateVisualEffect();
                }
                _missingVfx = _instantiatedObject == null;
            } else {
                if (_lowQualityObject == null && _boltLowQuality != null) {
                    InstantiateParticles();
                }
                _missingVfx = _lowQualityObject == null;
            }
        }

        public void OnReset()
        {
            if (_useVfx) {
                _instantiatedObject.OnReset();
            } else {
                _lowQualityObject.OnReset();
            }
            if (_lightToDim != null) {
                if (_lightIntensity < 0) {
                    _lightIntensity = _lightToDim.intensity;
                }
                _lightToDim.intensity = _lightIntensity;
            }
        }

        public void SetBoltCastDelta(float delta)
        {
            if (_boltPrefab == null) return;
            //_boltPrefab.SetRate(delta);
        }

        public void SetBoltLifetime(float timeAlive, float lifeSpan)
        {
            if (_useVfx) {
                _instantiatedObject.SetLifetime(timeAlive, lifeSpan);
            } else {
                _lowQualityObject.SetLifetime(timeAlive, lifeSpan);
            }
        }

        public void Dissipate(float dissipateTime)
        {
            if (_useVfx) {
                _instantiatedObject.Dissipate(dissipateTime);
            } else {
                _lowQualityObject.Dissipate(dissipateTime);
            }
        }

        public void DimLight(float dimLightTime)
        {
            if (_lightToDim != null) {
                StartCoroutine(DimLightRoutine(dimLightTime));
            }
        }

        private IEnumerator DimLightRoutine(float timer)
        {
            for (float t = 0; t < timer; t += Time.deltaTime) {
                float delta = 1 - t / timer;
                _lightToDim.intensity = delta * _lightIntensity;
                yield return null;
            }
            _lightToDim.intensity = 0;
        }

        private void InstantiateVisualEffect()
        {
            _useVfx = true;
            if (_boltPrefab.gameObject.activeInHierarchy) {
                _instantiatedObject = _boltPrefab;
                return;
            }
            Transform location = _transformOverride != null ? _transformOverride : transform;
            _instantiatedObject = Instantiate(_boltPrefab, location);
        }

        private void InstantiateParticles()
        {
            _useVfx = false;
            if (_boltLowQuality == null) return;
            if (_boltLowQuality.gameObject.activeInHierarchy) {
                _lowQualityObject = _boltLowQuality;
                return;
            }
            Transform location = _transformOverride != null ? _transformOverride : transform;
            _lowQualityObject = Instantiate(_boltLowQuality, location);
        }
    }
}