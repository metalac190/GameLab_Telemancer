using Mechanics.Player;
using UnityEngine;

namespace Mechanics.Bolt
{
    public class BoltAnimator : MonoBehaviour
    {
        [SerializeField] private Transform _groundReference = null;
        [SerializeField] private bool _maintainLastOffset = false;
        [SerializeField] private Vector3 _defaultOffset = Vector3.down;
        [SerializeField] private float _smoothStep = 5;

        private Vector3 _currentOffset;

        private void OnEnable()
        {
            _groundReference.position = transform.position;
            _currentOffset = _defaultOffset;
        }

        private void Update()
        {
            GetGround();
        }

        private void GetGround()
        {
            if (_groundReference == null) return;

            Physics.Raycast(transform.position, Vector3.down, out var hit, PlayerState.settings.boltLightDownDist);

            if (hit.collider == null) {
                if (!_maintainLastOffset) {
                    _currentOffset = Vector3.Lerp(_currentOffset, _defaultOffset, _smoothStep * Time.deltaTime);
                }
                _groundReference.position = transform.position + _currentOffset;
                return;
            }

            _currentOffset = Vector3.Lerp(_currentOffset, Vector3.down * hit.distance, _smoothStep * Time.deltaTime);
            _groundReference.position = transform.position + _currentOffset;
        }
    }
}