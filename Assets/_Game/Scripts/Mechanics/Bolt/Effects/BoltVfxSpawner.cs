using System.Collections;
using UnityEngine;

namespace Mechanics.Bolt
{
    /// Summary:
    /// A helper script to instantiate an object on awake.
    /// Used to instantiate visual effect prefabs primarily
    public class BoltVfxSpawner : MonoBehaviour
    {
        [SerializeField] private BoltVfxController _boltPrefab = null;
        [SerializeField] private Transform _transformOverride = null;
        [SerializeField] private Light _lightToDim = null;

        private BoltVfxController _instantiatedObject;
        private float _lightIntensity = -1;

        private void OnEnable()
        {
            if (_instantiatedObject == null) {
                InstantiateVisualEffect();
            }
            if (_instantiatedObject != null) {
                _instantiatedObject.OnReset();
                if (_lightToDim != null) {
                    if (_lightIntensity < 0) {
                        _lightIntensity = _lightToDim.intensity;
                    }
                    _lightToDim.intensity = _lightIntensity;
                }
            }
        }

        public void SetBoltCastDelta(float delta)
        {
            if (_boltPrefab == null) return;
            //_boltPrefab.SetRate(delta);
        }

        public void Dissipate()
        {
            if (_instantiatedObject == null) return;
            _instantiatedObject.Dissipate();
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
            if (_boltPrefab == null) return;
            if (_boltPrefab.gameObject.activeInHierarchy) {
                _instantiatedObject = _boltPrefab;
            }
            Transform location = _transformOverride != null ? _transformOverride : transform;
            _instantiatedObject = Instantiate(_boltPrefab, location);
        }
    }
}