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
        private BoltVfxController _instantiatedObject;

        private void OnEnable()
        {
            if (_instantiatedObject == null) {
                InstantiateVisualEffect();
            }
            if (_instantiatedObject != null) {
                _instantiatedObject.Reset();
            }
        }

        public float Dissipate()
        {
            if (_instantiatedObject == null) return 0;
            return _instantiatedObject.Dissipate();
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