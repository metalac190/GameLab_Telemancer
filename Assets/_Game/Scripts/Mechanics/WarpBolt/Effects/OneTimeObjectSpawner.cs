using UnityEngine;

namespace Mechanics.WarpBolt.Effects
{
    /// Summary:
    /// A helper script to instantiate an object on awake.
    /// Used to instantiate visual effect prefabs primarily
    public class OneTimeObjectSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _objectToSpawn = null;
        [SerializeField] private Transform _transformOverride = null;
        private GameObject _instantiatedObject;

        private void Awake()
        {
            InstantiateVisualEffect();
        }

        private void InstantiateVisualEffect()
        {
            if (_objectToSpawn == null) return;
            if (_instantiatedObject != null) {
                Destroy(_instantiatedObject.gameObject);
            }
            Transform location = _transformOverride != null ? _transformOverride : transform;
            _instantiatedObject = Instantiate(_objectToSpawn, location);
        }
    }
}